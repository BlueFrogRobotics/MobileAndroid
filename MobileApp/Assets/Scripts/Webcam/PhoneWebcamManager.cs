using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;

/// <summary>
/// Takes care of getting the camera frame and set it to the correct format
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
    private bool mIsWebRTCConnection = false;


    void Start()
    {
        if (mWebRTC.activeSelf)
        {
            mIsWebRTCConnection = true;
        }
        else {
            mTime = Time.time;
            WebCamDevice[] lDevices = WebCamTexture.devices;

            //Get front facing camera
            for (int i = 0; i < lDevices.Length; i++)
            {
                if (lDevices[i].isFrontFacing)
                {
                    mWebcamTexture = new WebCamTexture(lDevices[i].name, mRequestedWidth, mRequestedHeight, mFPS);
                    break;
                }
            }

            //Initialize compression matrix
            mTempMat = new Mat(mRequestedHeight, mRequestedWidth, CvType.CV_8UC3);
            mCompression = new MatOfInt(Highgui.CV_IMWRITE_JPEG_QUALITY, mCompressQuality);
        }
    }

    void OnDisable()
    {
       /* if (!mIsWebRTCConnection)
        {
            mWebcamTexture.Stop();
            mPhoneWebcamStream.gameObject.SetActive(false);
        }*/
    }

    void Update()
    {
       /* if (!mIsWebRTCConnection)
        {
            if (!mWebcamTexture.didUpdateThisFrame || Time.time - mTime < 1 / mFPS)
                return;

            mTime = Time.time;
            mPhoneWebcamStream.texture = mWebcamTexture;
        }*/
    }

    public byte[] GetBuffer()
    {
      //Get camera frame, rotate and convert it to a byte array
        MatOfByte lBuffer = new MatOfByte();
        BuddyTools.Utils.WebCamTextureToMat(mWebcamTexture, mTempMat);

        if (mWebcamTexture.videoRotationAngle == 0)
            Core.flip(mTempMat, mTempMat, 1);
        else if (mWebcamTexture.videoRotationAngle == 90)
            Core.flip(mTempMat, mTempMat, 0);
        else if (mWebcamTexture.videoRotationAngle == 270)
            Core.flip(mTempMat, mTempMat, 1);

        Highgui.imencode(".jpg", mTempMat, lBuffer, mCompression);

        return lBuffer.toArray();
    }

    public bool DidUpdateThisFrame()
    {
        return mWebcamTexture.didUpdateThisFrame;
    }

    //Switch the activity of camera
    public void ActivatePhoneWebcam()
    {
        if (!mIsWebRTCConnection)
        {
            Debug.Log("webrtc is false;");
            if (!mPhoneWebcamStream.IsActive())
            {
                mPhoneWebcamStream.gameObject.SetActive(true);
                // mPhoneWebcamStream.texture = mWebcamTexture;
                // Debug.Log("Webcam height " + mRequestedHeight + ", width " + mRequestedWidth);
                // mWebcamTexture.Play();
            }
            else {
                mPhoneWebcamStream.gameObject.SetActive(false);
                // mWebcamTexture.Stop();
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
