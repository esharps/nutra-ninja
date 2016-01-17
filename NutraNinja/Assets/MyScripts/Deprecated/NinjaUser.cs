using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class NinjaUser : MonoBehaviour {

	KinectSensor mySensor = null;
	InfraredFrameReader irFrameReader = null;
	private ushort[] irFrameData;
	private byte[] irRawData;
	private Texture2D irTexture;

	public Texture2D GetInfraredTexture()
	{
		return irTexture;
	}

	// Use this for initialization
	void Start()
	{
		mySensor = KinectSensor.GetDefault();

		if (mySensor != null)
		{
			irFrameReader = mySensor.InfraredFrameSource.OpenReader();
			var irFrameDescrip = mySensor.InfraredFrameSource.FrameDescription;

			irFrameData = new ushort[irFrameDescrip.LengthInPixels * irFrameDescrip.Width * irFrameDescrip.Height];

			// Raw data contains B, G, R, A values 
			irRawData = new byte[irFrameDescrip.LengthInPixels * irFrameDescrip.Width * irFrameDescrip.Height * 4];

			irTexture = new Texture2D(irFrameDescrip.Width, irFrameDescrip.Height, TextureFormat.BGRA32, false);

			if (!mySensor.IsOpen)
			{
				mySensor.Open();
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(irFrameReader != null)
		{
			var frame = irFrameReader.AcquireLatestFrame();
			if (frame != null)
			{
				frame.CopyFrameDataToArray(irFrameData);

				int index = 0;
				foreach(var ir in irRawData)
				{
					byte intensity = (byte)(ir >> 8);
					irRawData[index++] = intensity; // Blue 
					irRawData[index++] = intensity; // Green
					irRawData[index++] = intensity; // Red
					irRawData[index++] = 255; // Alpha (opacity)
				}

				irTexture.LoadRawTextureData(irRawData);
				irTexture.Apply();

				frame.Dispose();
				frame = null;
			}

		}
	}

	void OnApplicationQuit()
	{
		if (irFrameReader != null)
		{
			irFrameReader.Dispose();
			irFrameReader = null;
		}
		if (mySensor != null)
		{
			if (mySensor.IsOpen)
				mySensor.Close();
			mySensor = null;
		}
	}

}

