using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;

/// <summary>
/// Updates the x, y pixel coordinate positions of player Joints
/// and stores them as public static variables that can be 
/// easily accessed by other classes in the Unity project through
/// implicit calls to the NinjaJointManager class
/// @author Emily Sharp
/// </summary>
public class NinjaJointManager : MonoBehaviour {


	private KinectSensor mySensor;
	private MultiSourceFrameReader msFrameReader;

	// Coordinate Mapper identifies whether a 3D space corresponds to 
	// a point in the color or depth 2D space 
	CoordinateMapper myCoordinateMapper = null;

	// Foreground texture of ninja (player)
	private Texture2D ninjaTex;

	// Rectangle taken by the foreground texture (in pixels)
	private Rect foregroundGuiRect;
	private Rect foregroundImgRect;

	private const int colorWidth = 1920;
	private const int colorHeight = 1080;

	// IDictionary mapping out which joints are connected in forming a skeleton
	private IDictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> jointLib;

	// IList of Bodies detected by Kinect sensor
	IList<Body> bodies;

	// Current player's Body as mapped by the Kinect
	Body ninjaBody;

	StreamWriter sw_cm_x;
	StreamWriter sw_cm_y;
	StreamWriter sw_t;

	public static float wristRightX;
	public static float wristRightY;

	public static float wristLeftX;
	public static float wristLeftY;

	public static float handRightX;
	public static float handRightY;

	public static float handLeftX;
	public static float handLeftY;

	public static float ankleRightX;
	public static float ankleRightY;

	public static float ankleLeftX;
	public static float ankleLeftY;

	public static float headX;
	public static float headY;

	public static float spineMidX;
	public static float spineMidY;


	// Use this for initialization
	void Start () {
		mySensor = KinectSensor.GetDefault();

		if (mySensor != null)
		{
			if (!mySensor.IsOpen)
			{
				mySensor.Open();
			}

			//Writing data to an output file for graphing analysis
			sw_cm_x = new StreamWriter("PLAYER_CM_X");
			sw_cm_y = new StreamWriter("PLAYER_CM_Y");
			sw_t = new StreamWriter("PLAYER_T");

			ninjaTex = new Texture2D(colorWidth, colorHeight, TextureFormat.BGRA32, false);

			msFrameReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth |
				FrameSourceTypes.BodyIndex);

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
			//UNNECESSARY?
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if (msFrameReader == null)
		{
			return;
		}

		MultiSourceFrame msFrame = msFrameReader.AcquireLatestFrame();

		if (msFrame != null)
		{
			using (var _bodyFrame = msFrame.BodyFrameReference.AcquireFrame())
			{
				if (_bodyFrame.BodyFrameSource.BodyCount != null)
				{
					//An array of the Bodies currently being tracked by Kinect
					bodies = new Body[_bodyFrame.BodyFrameSource.BodyCount];

					//List of refreshed Body data
					_bodyFrame.GetAndRefreshBodyData(bodies);

					//Processing every detected user 
					//TODO: Implement TRACK CLOSEST PLAYER 
					foreach (var ninjaBody in bodies)
					{
						if (ninjaBody.IsTracked)
						{
							// Fully process Dictionary of Joints (25)
							foreach (Windows.Kinect.Joint joint in ninjaBody.Joints.Values)
							{

								if (joint.TrackingState == TrackingState.Tracked)
								{

									// 3D location in camera space
									CameraSpacePoint jointPosition = joint.Position;

									// Mapping camera to 2D point in color space
									ColorSpacePoint colorPoint = mySensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

									// Update x,y coordinates for Joints in which we are interested 
									if (joint.JointType.Equals(JointType.HandTipRight))
									{
										wristRightX = colorPoint.X;
										wristRightY = colorPoint.Y;
									}
									if (joint.JointType.Equals(JointType.HandTipLeft))
									{
										wristLeftX = colorPoint.X;
										wristLeftY = colorPoint.Y;
									}
									if (joint.JointType.Equals(JointType.AnkleRight))
									{
										ankleRightX = colorPoint.X;
										ankleRightY = colorPoint.Y;
									}
									if (joint.JointType.Equals(JointType.AnkleLeft))
									{
										ankleLeftX = colorPoint.X;
										ankleLeftY = colorPoint.Y;
									}
									if (joint.JointType.Equals(JointType.Head))
									{
										headX = colorPoint.X;
										headY = colorPoint.Y;
									}
									if (joint.JointType.Equals(JointType.SpineMid))
									{
										spineMidX = colorPoint.X;
										spineMidY = colorPoint.Y;

										sw_cm_x.WriteLine(spineMidX);
										sw_cm_y.WriteLine(spineMidY);
										sw_t.WriteLine(Time.time);
									}
									if(joint.JointType.Equals(JointType.HandRight))
									{
										handRightX = colorPoint.X;
										handRightY = colorPoint.Y;
									}
									if (joint.JointType.Equals(JointType.HandLeft))
									{
										handLeftX = colorPoint.X;
										handLeftY = colorPoint.Y;
									}
								}
							} //End of cycling through Dictionary of all 25 Joints

						} //End of processing individual ninjaBody

					} //End of processing ALL tracked bodies
				
				}//End of body tracking

			}//End of BodyFrame analysis

		}//End of msFrame processing

	} //End of Update() method
}
