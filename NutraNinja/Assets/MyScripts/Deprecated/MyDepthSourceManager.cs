using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class MyDepthSourceManager : MonoBehaviour {

	private KinectSensor mySensor;
	private DepthFrameReader depthFrameReader;
	private ushort[] depthFrameData = null;
	private byte[] depthRawData = null;

	// Foreground texture
	private Texture2D depthTex;

	private CoordinateMapper coordMapper = null;

	private DepthSpacePoint[] colorMappedToDepthPoints = null;

	// Rectangle taken by the foreground texture (in pixels)
	private Rect foregroundGuiRect;
	private Rect foregroundImgRect;

	private int depthImageWidth;
	private int depthImageHeight;

	// Size of the RGB pixels in the bitmap
	private const int BYTES_PER_PIXEL = 4;

	public Texture2D GetDepthTexture()
	{
		return depthTex;
	}

	// Use this for initialization
	void Start () {

		mySensor = KinectSensor.GetDefault();

		if (mySensor != null)
		{
			depthFrameReader = mySensor.DepthFrameSource.OpenReader();
			var depthFrameDescrip = mySensor.DepthFrameSource.FrameDescription;

			depthFrameData = new ushort[depthFrameDescrip.LengthInPixels];

			// Raw data contains B, G, R, A values
			depthRawData = new byte[depthFrameDescrip.LengthInPixels * BYTES_PER_PIXEL];

			depthTex = new Texture2D(depthFrameDescrip.Width, depthFrameDescrip.Height, TextureFormat.BGRA32, false);

			if (!mySensor.IsOpen)
			{
				mySensor.Open();
			}

			//Get the IR image size
			depthImageHeight = depthFrameDescrip.Height;
			depthImageWidth = depthFrameDescrip.Width;

			//Calculate the foreground rectangles
			Rect cameraRect = Camera.main.pixelRect;
			float rectHeight = cameraRect.height;
			float rectWidth = cameraRect.width;

			if (rectWidth > rectHeight)
				rectWidth = rectHeight * depthImageWidth / depthImageHeight;
			else
				rectHeight = rectWidth * depthImageHeight / depthImageWidth;

			float foregroundOfsX = (cameraRect.width - rectWidth) / 2;
			float foregroundOfsY = (cameraRect.height - rectHeight) / 2;
			foregroundImgRect = new Rect(foregroundOfsX, foregroundOfsY, rectWidth, rectHeight);
			foregroundGuiRect = new Rect(foregroundOfsX, cameraRect.height - foregroundOfsY, rectWidth, -rectHeight);
		}
	}

	// Update is called once per frame
	void Update () {

		ushort minDepth = 0;
		ushort maxDepth = 0;

		if (depthFrameReader != null)
		{
			var frame = depthFrameReader.AcquireLatestFrame();
			if (frame != null)
			{
				frame.CopyFrameDataToArray(depthFrameData);

				minDepth = frame.DepthMinReliableDistance;
				maxDepth = frame.DepthMaxReliableDistance;

				int index = 0;
				int mapDepthToByte = maxDepth / 256;

				for (int i = 0; i < depthFrameData.Length; i++)
				{
					ushort depth = depthFrameData[i];

					byte intensity = (byte)(depth >= minDepth &&
						depth <= maxDepth ? (depth / mapDepthToByte) : 0);
					depthRawData[index++] = intensity; //Blue
					depthRawData[index++] = intensity; //Green
					depthRawData[index++] = intensity; //Red
					depthRawData[index++] = 100; //Alpha (opacity)

				}

				depthTex.LoadRawTextureData(depthRawData);
				depthTex.Apply();

				frame.Dispose();
				frame = null;
			}
		}
	}

	void OnGUI()
	{
		if (depthTex)
		{
			GUI.DrawTexture(foregroundGuiRect, depthTex);
		}
	}

	void OnApplicationQuit()
	{
		if (depthFrameReader != null)
		{
			depthFrameReader.Dispose();
			depthFrameReader = null;
		}
		if (mySensor != null)
		{
			if (mySensor.IsOpen)
				mySensor.Close();
			mySensor = null;
		}
	}

}
