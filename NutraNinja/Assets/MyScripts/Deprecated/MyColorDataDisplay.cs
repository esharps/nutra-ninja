using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Windows.Kinect;


// <summary>
// Displays the Kinect 1920 x 1080 color-camera view in Unity by
// attaching this script to the Main Camera in a Unity scene and
// running the scene normally  
// @author Emily Sharp
// </summary>
public class MyColorDataDisplay : MonoBehaviour {

	private KinectSensor mySensor;
	private MultiSourceFrameReader msFrameReader; 

	// The BGRA pixel values
	private byte[] colorFrameData = null;

	// Foreground texture
	private Texture2D backgroundTex;

	// Rectangle taken by the foreground texture (in pixels)
	private Rect foregroundGuiRect;
	private Rect foregroundImgRect;

	// Resolution of Color stream inputs 
	private const int colorWidth = 1920;
	private const int colorHeight = 1080;

	// Each pixel is comprised of 4 bytes worth of data, where
	// the bytes represent values for BLUE, GREEN, RED, ALPHA
	private const int bytes_per_pixel = 4;


	public Texture2D GetBodyMaskTexture()
	{
		return backgroundTex;
	}


	// Use this for initialization
	void Start () {

		mySensor = KinectSensor.GetDefault();

		if (mySensor != null)
		{
			// Total array of data representing a single rendered frame
			colorFrameData = new byte[colorWidth * colorHeight * bytes_per_pixel];

			backgroundTex = new Texture2D(colorWidth, colorHeight, TextureFormat.BGRA32, false);

			if (!mySensor.IsOpen)
			{
				mySensor.Open();
			}

			msFrameReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color);

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
	} //End of Start()

	// Update is called once per frame
	void Update () {

		if (msFrameReader == null)
		{
			return;
		}

		MultiSourceFrame msFrame = msFrameReader.AcquireLatestFrame();

		if(msFrame != null)
		{

			using (var _colorFrame = msFrame.ColorFrameReference.AcquireFrame())
			{
				// Update color data
				if (_colorFrame != null && _colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
				{
					_colorFrame.CopyRawFrameDataToArray(colorFrameData);
					_colorFrame.Dispose();
				}
				else
				{
					_colorFrame.CopyConvertedFrameDataToArray(colorFrameData, ColorImageFormat.Bgra);
					_colorFrame.Dispose();
				}
			}

			backgroundTex.LoadRawTextureData(colorFrameData);
			backgroundTex.Apply();
		}
	} // End of Update()

	void OnGUI()
	{
		if (backgroundTex)
		{
			GUI.DrawTexture(foregroundGuiRect, backgroundTex);
		}
	}

	void OnApplicationQuit()
	{
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

} //End of class
