using UnityEngine;
using System;
using System.Collections;
using Windows.Kinect;

public class NinjaBodyMaskManager : MonoBehaviour {

	private KinectSensor mySensor;
	private MultiSourceFrameReader msFrameReader;

	// Depth data values
	private ushort[] depthFrameData = null;

	// RGB pixel values
	private byte[] colorFrameData = null;

	// Body index values 
	private byte[] bodyIndexFrameData = null;

	// BGRA values used for background removal effect
	private byte[] displayPixels = null;

	// Color points used for background removal 
	ColorSpacePoint[] colorPoints = null;

	// Coordinate mapper for background removal
	CoordinateMapper myCoordinateMapper = null;

	// Foreground texture of ninja (player)
	private Texture2D ninjaTex;

	// Rectangle taken by the foreground texture (in pixels)
	private Rect foregroundGuiRect;
	private Rect foregroundImgRect;

	private const int bodyCount = 1;
	private const int bytes_per_pixel = 4;

	// Resolutions of   depth, bodyIndex, Color stream inputs (bodyIndex res = depth res)
	private const int depthWidth = 512;
	private const int depthHeight = 424;
	private const int colorWidth = 1920;
	private const int colorHeight = 1080;

	public Texture2D GetBodyMaskTexture()
	{
		return ninjaTex;
	}


	// Use this for initialization
	void Start () {
	
		mySensor = KinectSensor.GetDefault();

		if (mySensor != null)
		{
			depthFrameData = new ushort[depthWidth * depthHeight];
			bodyIndexFrameData = new byte[depthWidth * depthHeight];
			colorFrameData = new byte[colorWidth * colorHeight * bytes_per_pixel];
			displayPixels = new byte[depthWidth * depthHeight * bytes_per_pixel];
			colorPoints = new ColorSpacePoint[depthWidth * depthHeight];

			ninjaTex = new Texture2D(depthWidth, depthHeight, TextureFormat.BGRA32, false);

			if (!mySensor.IsOpen)
			{
				mySensor.Open();
			}

			myCoordinateMapper = mySensor.CoordinateMapper;

			msFrameReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.BodyIndex |
				FrameSourceTypes.Color | FrameSourceTypes.Depth);

			//Rendering user as part of the Unity Scene background via Main Camera
			Rect cameraRect = Camera.main.pixelRect;
			float rectHeight = cameraRect.height;
			float rectWidth = cameraRect.width;

			if (rectWidth > rectHeight)
				rectWidth = rectHeight * depthWidth / depthHeight;
			else
				rectHeight = rectWidth * depthHeight / depthWidth;

			float foregroundOfsX = (cameraRect.width - rectWidth) / 2;
			float foregroundOfsY = (cameraRect.height - rectHeight) / 2;
			foregroundImgRect = new Rect(foregroundOfsX, foregroundOfsY, rectWidth, rectHeight);
			foregroundGuiRect = new Rect(foregroundOfsX, cameraRect.height - foregroundOfsY, rectWidth, -rectHeight);
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
			using (var _depthFrame = msFrame.DepthFrameReference.AcquireFrame())
			{
				using (var _colorFrame = msFrame.ColorFrameReference.AcquireFrame())
				{
					using (var _bodyIndexFrame = msFrame.BodyIndexFrameReference.AcquireFrame())
					{

						// Update depth data
						if (_depthFrame != null)
						{
							_depthFrame.CopyFrameDataToArray(depthFrameData);
							_depthFrame.Dispose();
						}

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

						// Update body index data
						if (_bodyIndexFrame != null)
						{
							_bodyIndexFrame.CopyFrameDataToArray(bodyIndexFrameData);
							_bodyIndexFrame.Dispose();
						}

						// Now do coordinate mapping = mapping depth values to colorPoints array
						myCoordinateMapper.MapDepthFrameToColorSpace(depthFrameData, colorPoints);

						// Clear the array of pixels to be displayed
						Array.Clear(displayPixels, 0, displayPixels.Length);

						for (int y = 0; y < depthHeight; ++y)
						{
							for (int x = 0; x < depthWidth; ++x)
							{

								int depthIndex = (y * depthWidth) + x;

								byte player = bodyIndexFrameData[depthIndex];

								// Determine whether the current pixel belongs to a player
								if (player != 0xff)
								{
									ColorSpacePoint currColorPoint = colorPoints[depthIndex];

									int colorX = (int)Math.Floor(currColorPoint.X + 0.5);
									int colorY = (int)Math.Floor(currColorPoint.Y + 0.5);

									if ((colorX >= 0) && (colorX < colorWidth) && (colorY >= 0) &&
										(colorY < colorHeight))
									{
										int colorIndex = ((colorY * colorWidth) + colorX) * bytes_per_pixel;
										int displayIndex = depthIndex * bytes_per_pixel;

										displayPixels[displayIndex + 0] = 0xe2; // BLUE colorFrameData[colorIndex]
										displayPixels[displayIndex + 1] = 0xe2; // GREEN colorFrameData[colorIndex + 1];
										displayPixels[displayIndex + 2] = 0xe5; // RED colorFrameData[colorIndex + 2];
										displayPixels[displayIndex + 3] = 0x80; // ALPHA

										/** Enable this code to display color video image of player **/
										//displayPixels[displayIndex + 0] = colorFrameData[colorIndex];
										//displayPixels[displayIndex + 1] = colorFrameData[colorIndex + 1];
										//displayPixels[displayIndex + 2] = colorFrameData[colorIndex + 2];
										//displayPixels[displayIndex + 3] = 0xff;
									}
								}
							}
						} // End of for-loops


						/** Code to draw boundaries of the Kinect Depth camera **/
						int offset = 5; 

						// Processing down 424 rows
						for (int i = 0; i < displayPixels.Length; i += 4 * 512)
						{
							for (int j = 0; j < 4 * offset; j += 4)
							{
								displayPixels[i + j] = 0x00;    //Blue
								displayPixels[i + j + 1] = 0x00;//Green
								displayPixels[i + j + 2] = 0x00;//Red
								displayPixels[i + j + 3] = 0x80;//Alpha
							}

							for (int k = 4 * (depthWidth - 1); k > 4 * (depthWidth - 1 - offset); k -= 4)
							{
								displayPixels[i + k] = 0x00;    //Blue
								displayPixels[i + k + 1] = 0x00;//Green
								displayPixels[i + k + 2] = 0x00;//Red
								displayPixels[i + k + 3] = 0x80;//Alpha
							}
						}

						// Render to Unity Scene background
						ninjaTex.LoadRawTextureData(displayPixels);
						ninjaTex.Apply();
					}
				}
			}
		}
	}

	void OnGUI()
	{
		if (ninjaTex)
		{
			GUI.DrawTexture(foregroundGuiRect, ninjaTex);
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

}
