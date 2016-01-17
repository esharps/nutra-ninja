using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;

/// <summary>
/// Tracks the Right Hand, Left Hand, Right Foot, and Left Foot player joints with
/// the Kinect. Determines if the player is currently performing a valid SLASH gesture,
/// and if so, renders a line.
/// @author Emily Sharp
/// </summary>
public class JointGestureManager : MonoBehaviour {

	private KinectSensor mySensor;
	private MultiSourceFrameReader msFrameReader;

	// Coordinate Mapper identifies whether a 3D space corresponds to 
	// a point in the color or depth 2D space.
	CoordinateMapper myCoordinateMapper = null;

	BodyFrame bodyFrame = null;

	// Current users Body as mapped by the Kinect
	Body ninjaBody;

	StreamWriter sw_v;
	StreamWriter sw_t;
	StreamWriter sw_x;
	StreamWriter sw_y;

	// IList of Bodies detected 
	IList<Body> bodies;

	// Foreground texture of ninja (player)  
	private Texture2D ninjaTex;

	public float minGestureVelocity;

	// IDictionary mapping out which joints are connected in forming a skeleton
	private IDictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> jointLib;

	// Rectangle taken by the foreground texture (in pixels)
	private Rect foregroundGuiRect;
	private Rect foregroundImgRect;

	private const int colorWidth = 1920;
	private const int colorHeight = 1080;

	// Queues that dynamically track the X, Y coordinates and time stamp for each joint
	Queue<float> rightHandQueue_X, rightHandQueue_Y, rightHandQueue_T;
	Queue<float> leftHandQueue_X, leftHandQueue_Y, leftHandQueue_T;
	Queue<float> rightFootQueue_X, rightFootQueue_Y, rightFootQueue_T;
	Queue<float> leftFootQueue_X, leftFootQueue_Y, leftFootQueue_T;

	// The current X, Y coordinate values to render as lines
	public float currRightHand_X, currRightHand_Y, currLeftHand_X, currLeftHand_Y;
	public float currRightFoot_X, currRightFoot_Y, currLeftFoot_X, currLeftFoot_Y;

	// Variables for rendering the slash lines as player performs valid gesture
	public Color c1 = Color.yellow;
	public Color c2 = Color.red;
	private GameObject lineGameObj;
	private LineRenderer lineRenderer;
	private int i = 0;


	// Determines the gesture state of each Joint 
	// 0 = no gesture detected, 1 = gesture detected
	public int RH_GESTURE, LH_GESTURE, RF_GESTURE, LF_GESTURE = 0;

	// Where the LineRender renders a line
	private const float Z_RENDER = 10; 

	// Use this for initialization
	void Start()
	{
		mySensor = KinectSensor.GetDefault();

		if (mySensor != null)
		{
			if (!mySensor.IsOpen)
			{
				mySensor.Open();
			}

			ninjaTex = new Texture2D(colorWidth, colorHeight, TextureFormat.BGRA32, false);

			msFrameReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth |
				FrameSourceTypes.BodyIndex);

			// There has to be a more efficient way of tracking these (i.e. using OOP)
			rightHandQueue_X = new Queue<float>();
			rightHandQueue_Y = new Queue<float>();
			rightHandQueue_T = new Queue<float>();

			leftHandQueue_X = new Queue<float>();
			leftHandQueue_Y = new Queue<float>();
			leftHandQueue_T = new Queue<float>();

			rightFootQueue_X = new Queue<float>();
			rightFootQueue_Y = new Queue<float>();
			rightFootQueue_T = new Queue<float>();

			leftFootQueue_X = new Queue<float>();
			leftFootQueue_Y = new Queue<float>();
			leftFootQueue_T = new Queue<float>();

			/** Construct StreamWriter object for collecting user data **/
			sw_v = new StreamWriter("EMILY_V.txt");
			sw_t = new StreamWriter("EMILY_T.txt");
			sw_x = new StreamWriter("EMILY_X.txt");
			sw_y = new StreamWriter("EMILY_Y.txt");


			InitializeSlashRenderer();

			//Rendering user as part of the Unity Scene background via Main Camera
			Rect cameraRect = Camera.main.pixelRect;
			float rectHeight = cameraRect.height;
			float rectWidth = cameraRect.width;

			if (rectWidth > rectHeight)
				rectWidth = rectHeight * colorWidth / colorHeight;
			else
				rectHeight = rectWidth * colorHeight / colorWidth;

			float foregroundOfsX = (cameraRect.width - rectWidth) / 2;
			float foregroundOfsY = (cameraRect.height - rectHeight) / 2;
			foregroundImgRect = new Rect(foregroundOfsX, foregroundOfsY, rectWidth, rectHeight);
			foregroundGuiRect = new Rect(foregroundOfsX, cameraRect.height - foregroundOfsY, rectWidth, -rectHeight);
		}

	}

	// Update is called once per frame
	void Update()
	{

		if (msFrameReader == null)
		{
			return;
		}

		MultiSourceFrame msFrame = msFrameReader.AcquireLatestFrame();

		if (msFrame != null)
		{
			using (var _bodyFrame = msFrame.BodyFrameReference.AcquireFrame())
			{
				if (_bodyFrame.BodyFrameSource.BodyCount != null) { 
					bodies = new Body[_bodyFrame.BodyFrameSource.BodyCount];

					_bodyFrame.GetAndRefreshBodyData(bodies);

					foreach (var ninjaBody in bodies)
					{
						if (ninjaBody.IsTracked)
						{
							// Coordinate mapping the joints
							foreach (Windows.Kinect.Joint joint in ninjaBody.Joints.Values)
							{
								if (joint.TrackingState == TrackingState.Tracked)
								{
									if (joint.JointType.Equals(JointType.HandRight))
									{         

										//Debug.Log("RH Z-COORDINATE: " + joint.Position.Z);
										// 3D location in camera space
										CameraSpacePoint jointPosition = joint.Position;

										// 2D space point
										PointF _point = new PointF();

										ColorSpacePoint colorPoint = mySensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

										_point.X = colorPoint.X;
										_point.Y = colorPoint.Y;

										// Ensure we have a dataset of at least 5 points
										if (rightHandQueue_X.Count != 5 && rightHandQueue_Y.Count != 5)
										{
											rightHandQueue_X.Enqueue(_point.X);
											rightHandQueue_Y.Enqueue(_point.Y);
											rightHandQueue_T.Enqueue(Time.time);
											// Debug.Log(Time.fixedTime);
										}
										else
										{
											//Debug.Log("RIGHT HAND (" + _point.X + ", " + _point.Y + ")");
											HandGestureDetection(rightHandQueue_X, rightHandQueue_Y, rightHandQueue_T, "right");
											UpdateSlashRender(rightHandQueue_X, rightHandQueue_Y);

											//Debug.Log("Gesture State: " + RH_GESTURE);

											rightHandQueue_X.Dequeue();
											rightHandQueue_Y.Dequeue();
											rightHandQueue_T.Dequeue();

											//Debug.Log(rightHandQueue_X.Count);

											rightHandQueue_X.Enqueue(_point.X);
											rightHandQueue_Y.Enqueue(_point.Y);
											rightHandQueue_T.Enqueue(Time.time);
											// Debug.Log(Time.fixedTime);

											sw_x.WriteLine(_point.X);
											sw_y.WriteLine(_point.Y);
											sw_t.WriteLine(Time.time);
										}

									} //End of RightHand tracking 

									if (joint.JointType.Equals(JointType.HandLeft) && ninjaBody.HandLeftState.Equals("2"))
									{
										// 3D location in camera space
										CameraSpacePoint jointPosition = joint.Position;

										// 2D space point
										PointF _point = new PointF();

										ColorSpacePoint colorPoint = mySensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

										_point.X = colorPoint.X;
										_point.Y = colorPoint.Y;
										//Debug.Log("LEFT HAND (" + _point.X + ", " + _point.Y + ")");


										// 
										if (leftHandQueue_X.Count != 5 && leftHandQueue_Y.Count != 5)
										{
											leftHandQueue_X.Enqueue(_point.X);
											leftHandQueue_Y.Enqueue(_point.Y);
											leftHandQueue_T.Enqueue(Time.time);
										}
										else
										{
											HandGestureDetection(leftHandQueue_X, leftHandQueue_Y, leftHandQueue_T, "left");

											UpdateSlashRender(leftHandQueue_X, leftHandQueue_Y);



											//Debug.Log("Gesture State: " + RH_GESTURE);

											leftHandQueue_X.Dequeue();
											leftHandQueue_Y.Dequeue();
											leftHandQueue_T.Dequeue();

											//Debug.Log(rightHandQueue_X.Count);

											leftHandQueue_X.Enqueue(_point.X);
											leftHandQueue_Y.Enqueue(_point.Y);
											leftHandQueue_T.Enqueue(Time.time);

											sw_x.WriteLine(_point.X);
											sw_y.WriteLine(_point.Y);
											sw_t.WriteLine(Time.time);
										}

									} //End of LeftHand tracking 

									if (joint.JointType.Equals(JointType.FootRight))
									{
										// 3D location in camera space
										CameraSpacePoint jointPosition = joint.Position;

										// 2D space point
										PointF _point = new PointF();

										ColorSpacePoint colorPoint = mySensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

										_point.X = colorPoint.X;
										_point.Y = colorPoint.Y;
										// Debug.Log("RIGHT FOOT (" + _point.X + ", " + _point.Y +")");


										if (leftFootQueue_X.Count != 5 && leftFootQueue_Y.Count != 5)
										{
											leftFootQueue_X.Enqueue(_point.X);
											leftFootQueue_Y.Enqueue(_point.Y);
											leftFootQueue_T.Enqueue(Time.time);
										}
										else
										{
											//if(joint.Position.Y > ninjaBody.Joints.)

										}

									}// End of FootRight tracking

									if (joint.JointType.Equals(JointType.FootLeft))
									{
										// 3D location in camera space
										CameraSpacePoint jointPosition = joint.Position;

										// 2D space point
										PointF _point = new PointF();

										ColorSpacePoint colorPoint = mySensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

										_point.X = colorPoint.X;
										_point.Y = colorPoint.Y;
										//Debug.Log("LEFT FOOT (" + _point.X + ", " + _point.Y + ")");


										if (leftFootQueue_X.Count != 5 && leftFootQueue_Y.Count != 5)
										{
											leftFootQueue_X.Enqueue(_point.X);
											leftFootQueue_Y.Enqueue(_point.Y);
											leftFootQueue_T.Enqueue(Time.time);
											// Debug.Log(Time.fixedTime);
										}
										else
										{

										}
									}// End of FootLeft tracking

								}

							} // End of foreach Windows.Kinect.Joint dictiona
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Used to determine whether the user is performing a valid hand gesture,
	/// and adjusts the global GESTURE variable accordingly
	/// </summary>
	/// <param name="x"></param> Queue of the last 5 x-coordinates
	/// <param name="y"></param> Queue of the last 5 y-coordinates
	/// <param name="t"></param> Queue of the timestamps associated with the (x, y) coords
	/// <param name="hand"></param> RIGHT or LEFT
	void HandGestureDetection(Queue<float> x, Queue<float> y, Queue<float> t, string hand)
	{
		float[] xArray = new float[x.Count];
		xArray = x.ToArray();
		float[] yArray = new float[y.Count];
		yArray = y.ToArray();
		float[] tArray = new float[t.Count];
		tArray = t.ToArray();


		float xInitial = xArray[0];
		float xFinal = xArray[xArray.Length - 1];

		//Debug.Log("X_i " + xInitial);
		//Debug.Log("X_f " + xFinal);

		float yInitial = yArray[0];
		float yFinal = yArray[yArray.Length - 1];

		float tInitial = tArray[0];
		//Debug.Log(tInitial);
		tInitial = Mathf.Round(tInitial * 1000f) / 1000f;

		float tFinal = tArray[tArray.Length - 1];
		//Debug.Log(tFinal);
		tFinal = Mathf.Round(tFinal * 1000f) / 1000f;

		float vX = ((xFinal - xInitial) / (tFinal - tInitial));
		float vY = ((yFinal - yInitial) / (tFinal - tInitial));


		// Cheat way of taking the absolute value of float
		if (vX < 0)
			vX = vX * (-1);
		if (vY < 0)
			vY = vY * (-1);

		// Debug.Log("( " + vX + ", " + vY + ")");
		if((vX > minGestureVelocity || vY > minGestureVelocity) && hand.Equals("right")) {
			RH_GESTURE = 1;

			for (int i = 0; i < xArray.Length - 1; i++)
			{
				Vector3 start = new Vector3(xArray[i], yArray[i], 0);
				Vector3 end = new Vector3(xArray[i + 1], yArray[i + 1], 0);

				//Debug.DrawLine(start, end, Color.white, 0.5f, false);
			}
		} 
		else 
		{
			RH_GESTURE = 0;
		}

		if ((vX > minGestureVelocity || vY > minGestureVelocity) && hand.Equals("left"))
		{
			LH_GESTURE = 1;
		} else {
			LH_GESTURE = 0;
		}
	}

	void InitializeSlashRenderer()  
	{
		// Initialization of LineRenderer resources
		lineGameObj = new GameObject("Slash");
		lineGameObj.AddComponent<LineRenderer>();
		lineRenderer = lineGameObj.GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Mobile/Particles/Additive"));
		lineRenderer.SetColors(c1, c2);
		lineRenderer.SetWidth(0.3f, 0);
		lineRenderer.SetVertexCount(0);
	}


	void UpdateSlashRender (Queue <float> xQueue, Queue <float> yQueue) {

		float[] xArray = xQueue.ToArray();
		float[] yArray = yQueue.ToArray();

		if (RH_GESTURE == 1)
		{
			//Debug.Log("HIT SLASH RENDER");
			lineRenderer.SetVertexCount(i + 1);

			//Rendering line segments b/w coordinates 5 at a time
			for (int j = 0; j < xArray.Length; j++ )
			{
				//Debug.Log("(" + xArray[j] + ", " + yArray[j] + ", " + Z_RENDER + ")");

				Vector3 handPosition = new Vector3(xArray[j] - 200, 1080 - 400 - yArray[j], Z_RENDER);
				lineRenderer.SetPosition(i, Camera.main.ScreenToWorldPoint(handPosition));
				Debug.Log("(" + handPosition.x + ", " + handPosition.y + ", " + handPosition.z + ")");
			}

			i++;
		}
		else
		{
			lineRenderer.SetVertexCount(0);
			i = 0;
		}
	}


	bool GestureDetection(Queue <float> qX, Queue <float> qY)
	{
		if (CheckForIncreaseTrend(qX))
			return true;
		else if (CheckForDecreaseTrend(qX))
			return true;
		else if (CheckForIncreaseTrend(qY))
			return true;
		else if (CheckForDecreaseTrend(qY))
			return true;
		return false;
	}

	bool CheckForIncreaseTrend(Queue<float> q)
	{
		float[] gestureArray = new float[q.Count];
		gestureArray = q.ToArray();

		for (int i = 0; i < gestureArray.Length - 1; i++)
		{
			if (gestureArray[i] > gestureArray[i + 1])
			{
				if (i == 3)
					return true;
				continue;
			}
			else
				return false;
		}
		return false;
	}

	bool CheckForDecreaseTrend(Queue<float> q)
	{
		float[] gestureArray = new float[q.Count];
		gestureArray = q.ToArray();

		for (int i = 0; i < gestureArray.Length - 1; i++)
		{
			if (gestureArray[i] < gestureArray[i + 1])
			{
				if (i == 3)
					return true;
				continue;
			}
			else
				return false;
		}
		return false;
	}



	void OnApplicationQuit()
	{
		sw_x.Close();
		sw_y.Close();
		sw_v.Close();
		sw_t.Close();

		if (msFrameReader != null)
		{
			msFrameReader.Dispose();
			msFrameReader = null;
		}
		if (mySensor != null)
		{
			if (mySensor.IsOpen)
				mySensor.Close();
			mySensor = null;
		}
	}
}
