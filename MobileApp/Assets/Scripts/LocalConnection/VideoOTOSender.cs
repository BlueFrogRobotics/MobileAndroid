using UnityEngine;
using OpenCVUnity;

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
        mLastTime = Time.time;
        mPhoneWebcam.CompressQuality = mCompressQuality;
        byte[] lData = mPhoneWebcam.GetBuffer();

        if (lData == null)
            return;

        SendData(lData, lData.Length);

        //if (!mOTO.HasAPeer || !mPhoneWebcam.DidUpdateThisFrame())
        //    return;

        //mPhoneWebcam.CompressQuality = mCompressQuality;
        //byte[] lData = mPhoneWebcam.GetBuffer();

        //if (lData == null)
        //    return;

        //SendData(lData, lData.Length);
    }
}
