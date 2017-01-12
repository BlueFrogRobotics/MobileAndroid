using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;

public class PhoneWebcamManager : MonoBehaviour
{
    public int CompressQuality { get { return mCompressQuality; } set { mCompressQuality = value; } }
    public int FPS { get { return mFPS; } set { mFPS = value; } }

    [SerializeField]
    private RawImage mPhoneWebcamStream;

    private int mCompressQuality = 30;
    private int mRequestedHeight = 240;
    private int mRequestedWidth = 320;
    private int mFPS = 20;
    private float mTime;
    //private Quaternion mPhoneBaseRotation;
    private Mat mTempMat;
    private MatOfInt mCompression;
    private WebCamTexture mWebcamTexture;

    void Start()
    {
        mTime = Time.time;
        WebCamDevice[] lDevices = WebCamTexture.devices;

        for (int i = 0; i < lDevices.Length; i++)
        {
            if (lDevices[i].isFrontFacing)
            {
                mWebcamTexture = new WebCamTexture(lDevices[i].name, mRequestedWidth, mRequestedHeight, mFPS);
                //mPhoneBaseRotation = mPhoneWebcamStream.transform.rotation;
                //mPhoneBaseRotation = mWebcamTexture.;
                break;
            }
        }

        mTempMat = new Mat(mRequestedHeight, mRequestedWidth, CvType.CV_8UC3);
        mCompression = new MatOfInt(Highgui.CV_IMWRITE_JPEG_QUALITY, mCompressQuality);
    }

    void OnDisable()
    {
        mWebcamTexture.Stop();
        mPhoneWebcamStream.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!mWebcamTexture.didUpdateThisFrame || Time.time - mTime < 1 / mFPS)
            return;

        mTime = Time.time;
        mPhoneWebcamStream.texture = mWebcamTexture;
    }

    public byte[] GetBuffer()
    {
        MatOfByte lBuffer = new MatOfByte();
        BuddyTools.Utils.WebCamTextureToMat(mWebcamTexture, mTempMat);

        if (mWebcamTexture.videoRotationAngle == 0)
        {
            Core.flip(mTempMat, mTempMat, 1);
        }
        else if (mWebcamTexture.videoRotationAngle == 90)
        {
            Core.flip(mTempMat, mTempMat, 0);
        }
        else if (mWebcamTexture.videoRotationAngle == 270)
        {
            Core.flip(mTempMat, mTempMat, 1);
        }

        Highgui.imencode(".jpg", mTempMat, lBuffer, mCompression);

        return lBuffer.toArray();
    }

    public bool DidUpdateThisFrame()
    {
        return mWebcamTexture.didUpdateThisFrame;
    }

    public void ActivatePhoneWebcam()
    {
        if (!mPhoneWebcamStream.IsActive())
        {
            mPhoneWebcamStream.gameObject.SetActive(true);
            mPhoneWebcamStream.texture = mWebcamTexture;
            Debug.Log("Webcam height " + mRequestedHeight + ", width " + mRequestedWidth);
            mWebcamTexture.Play();
        }
        else
        {
            mPhoneWebcamStream.gameObject.SetActive(false);
            mWebcamTexture.Stop();
        }
    }
}
