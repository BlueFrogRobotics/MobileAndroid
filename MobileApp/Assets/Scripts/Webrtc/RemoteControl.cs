using UnityEngine;
using UnityEngine.UI;

public class RemoteControl : MonoBehaviour {

    [SerializeField]
    private GameObject joystick;
    
    [SerializeField]
    private Webrtc webRTC;

    [SerializeField]
    private ToggleController toggleController;

    [SerializeField]
    private GameObject connectedView;

    [SerializeField]
    private GameObject disconnectedView;

    //public Slider mSlider;

    private int mClickCount = 0;
    private float mLastTime = 0.0f;
    private float mTime = 0.0f;

    void Update()
    {
        //if (webRTC.connectionState == Webrtc.CONNECTION.CONNECTING) {
        //    disconnectedView.SetActive(false);
        //    connectedView.SetActive(true);
        //} else if (webRTC.connectionState == Webrtc.CONNECTION.DISCONNECTING) {
        //    connectedView.SetActive(false);
        //    disconnectedView.SetActive(true);
        //}

        mTime += Time.deltaTime;
        if (mTime >= 0.2f)
        {
            mTime = 0.0f;
            //Debug.Log("X : " + BasePosition.transform.localPosition.x + " Y : " + BasePosition.transform.localPosition.y + " Z : " + BasePosition.transform.localPosition.z);
            if (toggleController.IsBodyActive) {
                if (Mathf.Abs(joystick.transform.localPosition.x) > 50.0f)
                {
                    if (Mathf.Sign(joystick.transform.localPosition.x) > 0)
                        webRTC.SendWithDataChannel("r");
                    else if (Mathf.Sign(joystick.transform.localPosition.x) < 0)
                        webRTC.SendWithDataChannel("l");
                }
                if (Mathf.Abs(joystick.transform.localPosition.y) > 50.0f)
                {
                    if (Mathf.Sign(joystick.transform.localPosition.y) > 0)
                        webRTC.SendWithDataChannel("f");
                    else if (Mathf.Sign(joystick.transform.localPosition.y) < 0)
                        webRTC.SendWithDataChannel("b");
                }
            } else {
                if (Mathf.Abs(joystick.transform.localPosition.x) > 50.0f)
                {
                    if (Mathf.Sign(joystick.transform.localPosition.x) > 0)
                        webRTC.SendWithDataChannel("d");
                    else if (Mathf.Sign(joystick.transform.localPosition.x) < 0)
                        webRTC.SendWithDataChannel("s");
                }
                if (Mathf.Abs(joystick.transform.localPosition.y) > 50.0f)
                {
                    if (Mathf.Sign(joystick.transform.localPosition.y) > 0)
                        webRTC.SendWithDataChannel("u");
                    else if (Mathf.Sign(joystick.transform.localPosition.y) < 0)
                        webRTC.SendWithDataChannel("t");
                }
            }
        }
    }

    public void DoubleClick()
    {
        float lDiffTime = Time.time - mLastTime;
        mLastTime = Time.time;

        if (lDiffTime > 0.500f) { 
            mClickCount = 0;
            return;
        }
        if (mClickCount > 1) {
            webRTC.SendWithDataChannel("z");
            mClickCount = 0;
        }
       
        mClickCount++;
    }

    // This function is called when a webrtc data is received
    public void OnMessage(string iMessage)
    {
        Debug.Log("receive postion : "+iMessage);
        //mSlider.value =  - float.Parse(iMessage);
    }
}
