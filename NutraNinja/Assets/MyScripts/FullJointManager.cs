using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;

/// <summary>
/// Keeps track of all 25 Kinect Joints for purposes
/// of collision detection and gesture recognition
/// </summary>
public class FullJointManager : MonoBehaviour {

    private KinectSensor mySensor;
    private MultiSourceFrameReader msFrameReader;

    // Coordinate Mapper identifies whether a 3D space corresponds to 
    // a point in the color or depth 2D space 
    CoordinateMapper myCoordinateMapper = null;

    // IDictionary mapping out which joints are connected in forming a skeleton
    private IDictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> jointLib;

    // IList of Bodies detected by Kinect sensor
    IList<Body> bodies;

    // Current player's Body as mapped by the Kinect
    Body ninjaBody;

    // List of (x, y) coordinate positions for all 25 Joints
    public static float spineBaseX, spineBaseY;
    public static float spineMidX, spineMidY;
    public static float neckX, neckY;
    public static float headX, headY;
    public static float shoulderLeftX, shoulderLeftY;
    public static float elbowLeftX, elbowLeftY;
    public static float wristLeftX, wristLeftY;
    public static float handLeftX, handLeftY;
    public static float shoulderRightX, shoulderRightY;
    public static float elbowRightX, elbowRightY;
    public static float wristRightX, wristRightY;
    public static float handRightX, handRightY;
    public static float hipLeftX, hipLeftY;
    public static float kneeLeftX, kneeLeftY;
    public static float ankleLeftX, ankleLeftY;
    public static float footLeftX, footLeftY;
    public static float hipRightX, hipRightY;
    public static float kneeRightX, kneeRightY;
    public static float ankleRightX, ankleRightY;
    public static float footRightX, footRightY;
    public static float spineShoulderX, spineShoulderY;
    public static float handTipLeftX, handTipLeftY;
    public static float thumbLeftX, thumbLeftY;
    public static float handTipRightX, handTipRightY;
    public static float thumbRightX, thumbRightY;

	// Use this for initialization
	void Start () {
	    mySensor = KinectSensor.GetDefault();

        if (mySensor != null)
        {
            if (!mySensor.IsOpen)
            {
                mySensor.Open();
            }

            // Acquiring data from Body, Depth and BodyIndex for Joint tracking
            msFrameReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth |
                FrameSourceTypes.BodyIndex);
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

                                    if (joint.JointType.Equals(JointType.SpineBase))
                                    {
                                        spineBaseX = colorPoint.X;
                                        spineBaseY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.SpineMid))
                                    {
                                        spineMidX = colorPoint.X;
                                        spineMidY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.Neck))
                                    {
                                        neckX = colorPoint.X;
                                        neckY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.Head))
                                    {
                                        headX = colorPoint.X;
                                        headY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.ShoulderLeft))
                                    {
                                        shoulderLeftX = colorPoint.X;
                                        shoulderLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.ElbowLeft))
                                    {
                                        elbowLeftX = colorPoint.X;
                                        elbowLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.WristLeft))
                                    {
                                        wristLeftX = colorPoint.X;
                                        wristLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.HandLeft))
                                    {
                                        handLeftX = colorPoint.X;
                                        handLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.ShoulderRight))
                                    {
                                        shoulderRightX = colorPoint.X;
                                        shoulderRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.ElbowRight))
                                    {
                                        elbowRightX = colorPoint.X;
                                        elbowRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.WristRight))
                                    {
                                        wristRightX = colorPoint.X;
                                        wristRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.HandRight))
                                    {
                                        handRightX = colorPoint.X;
                                        handRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.HipLeft))
                                    {
                                        hipLeftX = colorPoint.X;
                                        hipLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.KneeLeft))
                                    {
                                        kneeLeftX = colorPoint.X;
                                        kneeLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.AnkleLeft))
                                    {
                                        ankleLeftX = colorPoint.X;
                                        ankleLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.FootLeft))
                                    {
                                        footLeftX = colorPoint.X;
                                        footLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.HipRight))
                                    {
                                        hipRightX = colorPoint.X;
                                        hipRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.KneeRight))
                                    {
                                        kneeRightX = colorPoint.X;
                                        kneeLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.AnkleRight))
                                    {
                                        ankleRightX = colorPoint.X;
                                        ankleRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.FootRight))
                                    {
                                        footRightX = colorPoint.X;
                                        footRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.SpineShoulder))
                                    {
                                        spineShoulderX = colorPoint.X;
                                        spineShoulderY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.HandTipLeft))
                                    {
                                        handTipLeftX = colorPoint.X;
                                        handTipLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.ThumbLeft))
                                    {
                                        thumbLeftX = colorPoint.X;
                                        thumbLeftY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.HandTipRight))
                                    {
                                        handTipRightX = colorPoint.X;
                                        handTipRightY = colorPoint.Y;
                                    }
                                    if (joint.JointType.Equals(JointType.ThumbRight))
                                    {
                                        thumbRightX = colorPoint.X;
                                        thumbRightY = colorPoint.Y;
                                    }
                                }
                            } //End of cycling through Dictionary of all 25 Joints

                        } //End of processing individual ninjaBody

                    } //End of processing ALL tracked bodies

                }//End of body tracking

            }//End of BodyFrame analysis

        }//End of msFrame processing
	}
}
