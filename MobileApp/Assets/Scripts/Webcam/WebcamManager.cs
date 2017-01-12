using UnityEngine;
using BuddyOS;
using OpenCVUnity;

public class WebcamManager : MonoBehaviour
{
    [Range(1, 100)]
    public int mCompressQuality;

    private RGBCam mWebcam;
    private MatOfByte mBuffer = new MatOfByte();

    void Start()
    {
        mWebcam = BYOS.Instance.RGBCam;
        mWebcam.Open();
        Debug.Log("Opening Webcam");
    }
    
    void Update()
    {
        if (mWebcam.FrameMat != null) {
            Mat lFrame = mWebcam.FrameMat;
            Imgproc.resize(lFrame, lFrame, new Size(320, 240));
            MatOfInt lCompression = new MatOfInt(Highgui.CV_IMWRITE_JPEG_QUALITY, mCompressQuality);
            Highgui.imencode(".jpg", mWebcam.FrameMat, mBuffer, lCompression);
        }
    }

    public MatOfByte GetBuffer()
    {
        return mBuffer;
    }
}
