using UnityEngine;
using System.Collections;

/// <summary>
/// Updates the x-y position of the 2D collider object so that it maps
/// directly to the current position of a specific Kinect Joint. Performs
/// the y-coordinate conversion from the Kinect coordinate system to the 
/// Unity coordinate system in pixels. 
/// NOTE: Kinect origin (0,0) located in upper left corner of camera screen space
/// NOTE: Unity origin (0,0) located in lower left corner of Unity scene space
/// @author Emily Sharp
/// </summary>
public class KinectJointColliders : MonoBehaviour {


	const float Y_MID = 539.5f;
	Vector2 newPos;

	private int rightHand = -1;
	private int leftHand = -1;
    private int rightFoot = -1;
    private int leftFoot = -1;

	// Use this for initialization
	void Start () {
		
		if (gameObject.tag.Equals("RightHand"))
		{
			rightHand = 1;
			leftHand = 0;
		}
		if (gameObject.tag.Equals("LeftHand"))
		{
			rightHand = 0;
			leftHand = 1;
		}
        if(gameObject.tag.Equals("RightFoot"))
        {
            rightFoot = 1;
        }
        if(gameObject.tag.Equals("LeftFoot"))
        {
            leftFoot = 1;
        }

	}
	
	// Update is called once per frame
	void Update () {
	
		if (rightHand == 1)
		{
			float unityWristRightY = ConvertKinectYToUnityY(NinjaJointManager.wristRightY);
			newPos = new Vector2(NinjaJointManager.wristRightX, unityWristRightY);
			transform.position = newPos;
		}
		if(leftHand == 1)
		{
			float unityWristLeftY = ConvertKinectYToUnityY(NinjaJointManager.wristLeftY);
			newPos = new Vector2(NinjaJointManager.wristLeftX, unityWristLeftY);
			transform.position = newPos;
		}
        if(rightFoot == 1)
        {
            float unityFootLeftY = ConvertKinectYToUnityY(NinjaJointManager.ankleLeftY);
            newPos = new Vector2(NinjaJointManager.ankleLeftX, unityFootLeftY);
            transform.position = newPos;
        }
        if (rightFoot == 1)
        {
            float unityFootRightY = ConvertKinectYToUnityY(NinjaJointManager.ankleRightY);
            newPos = new Vector2(NinjaJointManager.ankleLeftX, unityFootRightY);
            transform.position = newPos;
        }
	}

	/// <summary>
	/// Performs the conversion from Kinect coordinate system (UL origin, BR max x-y) to
	/// the Unity coordinate system (BL origin, UR max x-y) so positions of user joints 
	/// can be rendered correctly in Unity space, where it interacts with other objects
	/// </summary>
	/// <param name="kinectCoordY"></param> Kinect y-coordinate to convert to Unity coord system
	/// <returns></returns>
	public static float ConvertKinectYToUnityY(float kinectCoordY)
	{
		//Default to y-coord screen center 
		float unityCoordY = Y_MID;

		if (kinectCoordY < Y_MID)
		{
			unityCoordY = kinectCoordY + 2 * (Y_MID - kinectCoordY);
			return unityCoordY;
		}
		if (kinectCoordY > Y_MID)
		{
			unityCoordY = kinectCoordY - 2 * (kinectCoordY - Y_MID);
			return unityCoordY;
		}
		return unityCoordY;
	}
}
