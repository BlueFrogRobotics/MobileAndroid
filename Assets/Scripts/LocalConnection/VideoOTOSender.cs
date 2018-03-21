using UnityEngine;
using OpenCVUnity;

/// <summary>
/// Sender for Video streaming between remote and phone
/// </summary>
public class VideoOTOSender : OTONetSender
{
    [Range(1, 100)]
    public int mCompressQuality;

    [Range(5, 30)]
    public int mRequestedFPS;

    [SerializeField]
    private OTONetwork OTO;

    [SerializeField]
    private PhoneWebcamManager mPhoneWebcam;

    private float mLastTime;
    private MatOfByte mBuffer;

    void Start()
    {
        mPhoneWebcam.FPS = mRequestedFPS;
        mPhoneWebcam.CompressQuality = mCompressQuality;
        mLastTime = Time.time;
    }

    void OnEnable()
    {
        //InvokeRepeating("SendPhoneVideo", 0.05f, 0.05f);
    }

    void Update()
    {
        //Check if connection is established and if a new frame is available to send
        if (!OTO.HasAPeer || !isActiveAndEnabled || !mPhoneWebcam.DidUpdateThisFrame())
            return;

        if (Time.time - mLastTime < 1 / mRequestedFPS)
            return;

        //Debug.Log("Video OTO has peer " + OTO.HasAPeer + " and sending");
        SendPhoneVideo();
    }

    void OnDisable()
    {
        //CancelInvoke("SendPhoneVideo");
    }

    void SendPhoneVideo()
    {
        //Compress the frame to set JPG quality and send the buffer
        mLastTime = Time.time;
        mPhoneWebcam.CompressQuality = mCompressQuality;
        byte[] lData = mPhoneWebcam.GetBuffer();

        if (lData == null)
            return;

        SendData(lData, lData.Length);
    }
}
