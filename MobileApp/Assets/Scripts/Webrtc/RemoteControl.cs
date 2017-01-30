using UnityEngine;
using UnityEngine.UI;
using BuddyOS.Command;

public class RemoteControl : MonoBehaviour {

    [SerializeField]
    private Transform joystick;
    
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

    //Network
    private byte[] mSentData;
    private float mTime;
    private float mLastTime = 0.0f;

    //Joysticks' var
    private float mXPosition;
    private float mYPosition;

    private float X_DELTA_JOYSTICK;
    private float Y_DELTA_JOYSTICK;

    //Motion's var
    private float mNoSpeed = 0f;
    private float mYesSpeed = 0f;
    private float mAngleYes = 0f;
    private float mAngleNo = 0f;
    private float mSpeedHead = 100f;
    private float mSpeedBody = 500f;
    private float mLeftSpeed = 0f;
    private float mRightSpeed = 0f;

    void OnEnable()
    {
        mTime = Time.time;

        X_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.y;
    }

    void Update()
    {
        //The goal here is to get the position of the cursor in the relative plane of the joystick
        if (!isActiveAndEnabled || Time.time - mTime < 0.1) return;
        
        mXPosition = joystick.localPosition.x / X_DELTA_JOYSTICK;
        mYPosition = joystick.localPosition.y / Y_DELTA_JOYSTICK;

        if (mXPosition != 0 && mYPosition != 0) {
            //The cursor of the joystick is being moved
            //We are controlling the body movement
            if (toggleController.IsBodyActive) {
                //Compute the desired body movement and send the serialized command to remote
                ComputeMobileBase();
                byte[] lMobileCmd = new SetWheelsSpeedCmd(mLeftSpeed, mRightSpeed, 100).Serialize();

                webRTC.SendWithDataChannel(GetString(lMobileCmd));
            }
            //We are controlling the head movement
            else {
                //Compute the desired head movement and send the serialized command to remote
                ComputeNoAxis();
                ComputeYesAxis();

                byte[] lNoCmd = new SetPosNoCmd(mAngleNo, mNoSpeed).Serialize();
                byte[] lYesCmd = new SetPosYesCmd(mAngleYes, mYesSpeed).Serialize();

                webRTC.SendWithDataChannel(GetString(lNoCmd));
                webRTC.SendWithDataChannel(GetString(lYesCmd));
            }
        }

        mTime = Time.time;
    }

    /*void Update()
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
    }*/

    private void ComputeNoAxis()
    {
        //Add an increment to No axis position corresponding to the cursor's
        mAngleNo -= mXPosition * 5f;

        if (Mathf.Abs(mAngleNo) > 45)
            mAngleNo = Mathf.Sign(mAngleNo) * 45;

        //headNoAnim.SetFloat("HeadPosition_H", -mAngleNo * 100 / 45);
        mNoSpeed = (mXPosition * mXPosition) * mSpeedHead * 3f;
    }

    private void ComputeYesAxis()
    {
        //Add an increment to Yes axis position corresponding to the cursor's
        mAngleYes -= mYPosition * 5f;

        if (mAngleYes < -30)
            mAngleYes = -30;

        if (mAngleYes > 60)
            mAngleYes = 60;

        //headYesAnim.SetFloat("HeadPosition_V", mAngleYes);
        mYesSpeed = (mYPosition * mYPosition) * mSpeedHead * 3f;
    }

    private void ComputeMobileBase()
    {
        //Compute the speed of both wheels according to the cursors position;
        //The direction is thus influences by the speed of both wheels
        float lRadius = Mathf.Sqrt(mXPosition * mXPosition + mYPosition * mYPosition);
        float lAngle = (Mathf.Atan2(mYPosition, mXPosition));
        Debug.Log("Body position radius : " + lRadius + " / body position angle : " + lAngle);
        mLeftSpeed = mSpeedBody * (Mathf.Sin(lAngle) + Mathf.Cos(lAngle) / 3) * lRadius;
        mRightSpeed = mSpeedBody * (Mathf.Sin(lAngle) - Mathf.Cos(lAngle) / 3) * lRadius;
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

    private string GetString(byte[] iBytes)
    {
        return System.Convert.ToBase64String(iBytes);
    }
}
