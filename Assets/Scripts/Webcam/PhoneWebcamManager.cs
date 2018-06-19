using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;

using Buddy;

/// <summary>
/// Takes care of getting the device's camera frame and set it to the correct format for a local remote control.
/// </summary>
public class PhoneWebcamManager : MonoBehaviour
{
    public int CompressQuality { get { return mCompressQuality; } set { mCompressQuality = value; } }
    public int FPS { get { return mFPS; } set { mFPS = value; } }

    [SerializeField]
    private RawImage mPhoneWebcamStream;
    [SerializeField]
    private GameObject mWebRTC = null;

    private int mCompressQuality = 30;
    private int mRequestedHeight = 240;
    private int mRequestedWidth = 320;
    private int mFPS = 20;
    private float mTime;
    private Mat mTempMat;
    private MatOfInt mCompression;
    private WebCamTexture mWebcamTexture;

	void OnStart()
	{
		Debug.Log ("[PhoneWebcamManager] OnStart");
	}

	void OnEnable()
	{
		Debug.Log ("[PhoneWebcamManager] OnEnable");

        // If the WebRTC is not active, we can have access to the camera.
		if(!mWebRTC.activeSelf)
		{
			Debug.Log ("[PhoneWebcamManager] not webrtc");

			mTime = Time.time;
			WebCamDevice[] lDevices = WebCamTexture.devices;

			// Get the front facing camera.
			for (int i = 0; i < lDevices.Length; i++) {
				if (lDevices[i].isFrontFacing) {
					mWebcamTexture = new WebCamTexture(lDevices[i].name, mRequestedWidth, mRequestedHeight, mFPS);
					break;
				}
			}

			// Initialize the JPEG compression matrix.
			mTempMat = new Mat(mRequestedHeight, mRequestedWidth, CvType.CV_8UC3);
			mCompression = new MatOfInt(Highgui.CV_IMWRITE_JPEG_QUALITY, mCompressQuality);
		}
	}

    void OnDisable()
    {
        // Stop the camera when disabling this element.
		Debug.Log ("[PhoneWebcamManager] OnDisable");

		if (!mWebRTC.activeSelf)
        {
            mWebcamTexture.Stop();
            mPhoneWebcamStream.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Display the camera's frame on the remote control screen.
		if (!mWebRTC.activeSelf)
        {
            if (!mWebcamTexture.didUpdateThisFrame || Time.time - mTime < 1 / mFPS)
                return;

            mTime = Time.time;
            mPhoneWebcamStream.texture = mWebcamTexture;
        }
    }

    /// <summary>
    /// Get the data of the camera's image.
    /// </summary>
    /// <returns>The content of the camera's image as a byte array.</returns>
    public byte[] GetBuffer()
    {
        //Get camera frame, rotate and convert it to a byte array
        MatOfByte lBuffer = new MatOfByte();
        Utils.WebCamTextureToMat(mWebcamTexture, mTempMat);        

        if (mWebcamTexture.videoRotationAngle == 0)
            Core.flip(mTempMat, mTempMat, 1);
        else if (mWebcamTexture.videoRotationAngle == 90)
            Core.flip(mTempMat, mTempMat, 0);
        else if (mWebcamTexture.videoRotationAngle == 270)
            Core.flip(mTempMat, mTempMat, 1);

        Highgui.imencode(".jpg", mTempMat, lBuffer, mCompression);

        return lBuffer.toArray();
    }

    /// <summary>
    /// Did the camera do an update at this frame ?
    /// </summary>
    /// <returns>True if an update was done, false otherwise.</returns>
    public bool DidUpdateThisFrame()
    {
        return mWebcamTexture.didUpdateThisFrame;
    }

    /// <summary>
    /// Open the device's camera and enable the different game objects.
    /// </summary>
    public void ActivatePhoneWebcam()
    {
		if (!mWebRTC.activeSelf)
        {
            if (!mPhoneWebcamStream.IsActive())
            {
                // Enable the display of the camera's image on the device.
                mPhoneWebcamStream.gameObject.SetActive(true);
                mPhoneWebcamStream.texture = mWebcamTexture;
                Debug.Log("Webcam height " + mRequestedHeight + ", width " + mRequestedWidth);
                mWebcamTexture.Play();
            }
            else {
                mPhoneWebcamStream.gameObject.SetActive(false);
                mWebcamTexture.Stop();
            }
        }
        else {
            if (!mPhoneWebcamStream.IsActive())
            {
                mPhoneWebcamStream.gameObject.SetActive(true);
            }
            else {
                mPhoneWebcamStream.gameObject.SetActive(false);
            }
        }
    }
}
