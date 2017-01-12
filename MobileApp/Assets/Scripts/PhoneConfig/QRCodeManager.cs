using UnityEngine;
using ZXing;

public class QRCodeManager : MonoBehaviour
{
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
}
