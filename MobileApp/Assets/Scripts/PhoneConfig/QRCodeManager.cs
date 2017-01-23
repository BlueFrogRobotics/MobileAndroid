using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

public class QRCodeManager : MonoBehaviour
{
    [SerializeField]
    private Image QRImage;

    [SerializeField]
    private RawImage cameraImage;

    [SerializeField]
    private Text resultText;

    private bool mCameraStarted;
    private WebCamTexture mCamera;
    private BarcodeReader mReader;
    private Mat mTempMat;

    void Start () {
        mCameraStarted = false;
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++) {
            if (!devices[i].isFrontFacing) {
                Debug.Log("Found Camera " + devices[i].name);
                mCamera = new WebCamTexture(devices[i].name, 320, 240, 20);
                break;
            }
        }
        mReader = new BarcodeReader { AutoRotate = true, TryInverted = true };
        mTempMat = new Mat(240, 320, CvType.CV_8UC3);
    }

    void Update()
    {
        if(mCamera != null && mCamera.didUpdateThisFrame) {
            Debug.Log("Trying to read QRCode");
            Result[] lResults = mReader.DecodeMultiple(mCamera.GetPixels32(), mCamera.width, mCamera.height);

            if (lResults != null && lResults.Length != 0) {
                string lNameQrCode = lResults[0].Text;
                resultText.text = lNameQrCode;
                Debug.Log("QR Code Message : " + lNameQrCode);
            }
            cameraImage.texture = mCamera;
            Debug.Log("Camera rotation " + mCamera.videoRotationAngle);
            BuddyTools.Utils.WebCamTextureToMat(mCamera, mTempMat);

            if (mCamera.videoRotationAngle == 0)
            {
                Core.flip(mTempMat, mTempMat, 1);
            }
            else if (mCamera.videoRotationAngle == 90)
            {
                Core.flip(mTempMat, mTempMat, 0);
            }
            else if (mCamera.videoRotationAngle == 270)
            {
                Core.flip(mTempMat, mTempMat, 1);
            }

            cameraImage.texture = BuddyTools.Utils.MatToTexture2D(mTempMat);
        }
    }

    public void SwitchQRCodeReader()
    {
        Debug.Log("Switching state");

        if (mCameraStarted)
            mCamera.Stop();
        else
            mCamera.Play();

        mCameraStarted = !mCameraStarted;        
    }

    public void CreateQrCode(string iData)
    {
        //We create the QRCode writer and encode the data
        QRCodeWriter lQRCode = new QRCodeWriter();
        BitMatrix lBitMatrix = lQRCode.encode(iData, BarcodeFormat.QR_CODE, 450, 450);

        //We have to go through each pixel to setup the texture ourself, so we set everything up
        Color32[] lColors = new Color32[450 * 450];
        Color32 lWhite = new Color32(255, 255, 255, 255);
        Color32 lBlack = new Color32(0, 0, 0, 255);

        //Do the actual reading and building the texture
        for(int i=0; i<lBitMatrix.Height; i++) {
            for(int j=0; j<lBitMatrix.Width; j++) {
                lColors[j + i * lBitMatrix.Width] = lBitMatrix[j, i] ? lBlack : lWhite;
            }
        }
        Texture2D lTempTex = new Texture2D(lBitMatrix.Width, lBitMatrix.Height);
        lTempTex.SetPixels32(0, 0, lBitMatrix.Width, lBitMatrix.Height, lColors);
        BuddyTools.Utils.SaveTextureToFile(lTempTex, Application.streamingAssetsPath + "/qrcode.png");

        //We have to go by this method to be sure the QRCode is properly displayed.
        byte[] lFileData = File.ReadAllBytes(BuddyTools.Utils.GetStreamingAssetFilePath("qrcode.png"));
        Texture2D lTex = new Texture2D(2, 2);
        lTex.LoadImage(lFileData);
        QRImage.sprite = Sprite.Create(lTex, new UnityEngine.Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
        QRImage.gameObject.SetActive(true);
    }
}
