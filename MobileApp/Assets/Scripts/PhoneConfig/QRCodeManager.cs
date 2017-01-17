using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using ZXing;
using ZXing.Common;

public class QRCodeManager : MonoBehaviour
{
    [SerializeField]
    private RawImage QRImage;

    private bool mCameraStarted;
    private WebCamTexture mCamera;
    
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
    }

    void Update()
    {
        if(mCamera.didUpdateThisFrame) {
            Debug.Log("Trying to read QRCode");
            BarcodeReader lReader = new BarcodeReader { AutoRotate = true, TryInverted = true };
            Result[] lResults = lReader.DecodeMultiple(mCamera.GetPixels32(), mCamera.width, mCamera.height);

            if (lResults != null && lResults.Length != 0) {
                string lNameQrCode = lResults[0].Text;
                Debug.Log("QR Code Message : " + lNameQrCode);
            }
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
        //According to tests, this works fine.
        //In reality, nothing is shown in the RawImage
        BarcodeWriter lQRCode = new BarcodeWriter() {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions() {
                Height = 450,
                Width = 450
            },
            Renderer = new Color32Renderer(),
            Encoder = new MultiFormatWriter()
        };
        Texture2D lTempText = new Texture2D(450, 450);
        BitMatrix lMatrix = lQRCode.Encode(iData);
        //Color32[] lTemp = lQRCode.Write(iData);
        Color32[] lTemp = lQRCode.Write(lMatrix);
        lTempText.SetPixels32(lTemp);

        QRImage.texture = lTempText;
        QRImage.gameObject.SetActive(true);

        BuddyTools.Utils.SaveTextureToFile(lTempText, Application.streamingAssetsPath + "/image.png");
    }
}
