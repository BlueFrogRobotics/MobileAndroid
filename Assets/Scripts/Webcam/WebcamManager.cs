using UnityEngine;
using OpenCVUnity;

using Buddy;

public class WebcamManager : MonoBehaviour
{
    [Range(1, 100)]
    public int mCompressQuality;

    private WebCamTexture mWebcam;
    private MatOfByte mBuffer = new MatOfByte();

    private Mat mFrame;

    void Start()
    {
        WebCamDevice[] lDevices = WebCamTexture.devices;

        foreach (WebCamDevice lDevice in lDevices) {
            if (lDevice.isFrontFacing) {
                mWebcam = new WebCamTexture(lDevice.name, 320, 240);
                break;
            }
        }

        if (mWebcam == null)
            mWebcam = new WebCamTexture(320, 240);

        mFrame = new Mat(CvType.CV_8UC3, 320, 240);

        mWebcam.Play();

        Debug.Log("Opening Webcam");
    }

    void Update()
    {
        Utils.WebCamTextureToMat(mWebcam, mFrame);
        //Imgproc.resize(lFrame, lFrame, new Size(320, 240));
        MatOfInt lCompression = new MatOfInt(Highgui.CV_IMWRITE_JPEG_QUALITY, mCompressQuality);
        Highgui.imencode(".jpg", mFrame, mBuffer, lCompression);
    }

    public MatOfByte GetBuffer()
    {
        return mBuffer;
    }
}
