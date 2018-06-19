﻿using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;

using Buddy;

/// <summary>
/// Receiver for the video streaming between the remote and phone.
/// </summary>
public class VideoOTOReceiver : OTONetReceiver
{
    [SerializeField]
    private RawImage mWebcamShowStream;

    [SerializeField]
    private OTONetwork mOTO;

    private Mat mDecodedImage;
    private Texture2D mTemp2DTexture;

    void Start()
    {
        if(mTemp2DTexture == null)
            mTemp2DTexture = new Texture2D(320, 240);
    }

    void OnEnable()
    {
        mWebcamShowStream.gameObject.SetActive(false);
    }

    public override void ReceiveData(byte[] iData, int iNData)
    {
        //Decode the byte stream and create a corresponding texture
        mDecodedImage = Highgui.imdecode(new MatOfByte(iData), 3);

        if (mDecodedImage.total() != 0) {
            Utils.MatToTexture2D(mDecodedImage, mTemp2DTexture);
            mWebcamShowStream.texture = mTemp2DTexture;
        }
    }
}
