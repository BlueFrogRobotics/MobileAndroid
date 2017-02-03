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
    private GameObject popupQRCode;

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
        //Find the not frontal camera to read QRCode
        mCameraStarted = false;
        //CreateCamera(true);
        //Initialize reader and texture
        mReader = new BarcodeReader { AutoRotate = true, TryInverted = true };
        mTempMat = new Mat(240, 320, CvType.CV_8UC3);
    }

    void Update()
    {
        if(mCamera != null && mCamera.didUpdateThisFrame) {
            //Read in the current frame to decode if there is a QRCode
            Result[] lResults = mReader.DecodeMultiple(mCamera.GetPixels32(), mCamera.width, mCamera.height);

            //There is a QRCode in the frame that gives a result
            if (lResults != null && lResults.Length != 0) {
                string lNameQrCode = lResults[0].Text;
                resultText.text = lNameQrCode;
                SwitchQRCodeReader();
                popupQRCode.SetActive(false);
            }
            //cameraImage.texture = mCamera;
            //Rotate the frame for display
            Debug.Log("Camera rotation " + mCamera.videoRotationAngle);
            BuddyTools.Utils.WebCamTextureToMat(mCamera, mTempMat);

            if (mCamera.videoRotationAngle == 0)
                Core.flip(mTempMat, mTempMat, 1);
            else if (mCamera.videoRotationAngle == 90)
                Core.flip(mTempMat, mTempMat, 0);
            else if (mCamera.videoRotationAngle == 270)
                Core.flip(mTempMat, mTempMat, 1);

            cameraImage.texture = BuddyTools.Utils.MatToTexture2D(mTempMat);
        }
    }

    private void CreateCamera(bool iIsFrontFacing)
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == iIsFrontFacing)
            {
                mCamera = new WebCamTexture(devices[i].name, 320, 240, 20);
                break;
            }
        }
    }

    public void SwitchQRCodeReader()
    {
        Debug.Log("Switching state");

        if (mCameraStarted) {
            mCamera.Stop();
            Destroy(mCamera);
        } else {
            CreateCamera(true);
            mCamera.Play();
        }

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
        for (int i = 0; i < lBitMatrix.Height; i++) {
            for (int j = 0; j < lBitMatrix.Width; j++)
                lColors[j + i * lBitMatrix.Width] = lBitMatrix[j, i] ? lBlack : lWhite;
        }
        Texture2D lTempTex = new Texture2D(lBitMatrix.Width, lBitMatrix.Height);
        lTempTex.SetPixels32(0, 0, lBitMatrix.Width, lBitMatrix.Height, lColors);

        //We have to go by this method to be sure the QRCode is properly displayed.
        Texture2D lTex = new Texture2D(2, 2);
        lTex.LoadImage(lTempTex.EncodeToPNG());
        QRImage.sprite = Sprite.Create(lTex, new UnityEngine.Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
        QRImage.gameObject.SetActive(true);
    }
}
