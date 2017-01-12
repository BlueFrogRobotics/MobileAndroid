using UnityEngine;
using UnityEngine.UI;
using BuddyOS.Command;

public class BuddyJoystickOTOSender : OTONetSender
{
    [SerializeField]
    private Transform joystickHead;

    [SerializeField]
    private Transform joystickHeadLandscape;

    [SerializeField]
    private Transform joystickBody;

    [SerializeField]
    private Transform joystickBodyLandscape;

    [SerializeField]
    private OTONetwork oto;

    [SerializeField]
    private Slider mHeadNoBar;

    //Network
    private byte[] mSentData;
    private bool mIsSent;
    private bool mIsInitialized;
    private float mTime;
    
    //Joysticks' var
    private float mXPositionHead;
    private float mYPositionHead;
    private float mXPositionBody;
    private float mYPositionBody;

    private float X_DELTA_JOYSTICK_HEAD;
    private float Y_DELTA_JOYSTICK_HEAD;
    private float X_DELTA_JOYSTICK_BODY;
    private float Y_DELTA_JOYSTICK_BODY;
    private float X_DELTA_JOYSTICK_HEAD_LANDSCAPE;
    private float Y_DELTA_JOYSTICK_HEAD_LANDSCAPE;
    private float X_DELTA_JOYSTICK_BODY_LANDSCAPE;
    private float Y_DELTA_JOYSTICK_BODY_LANDSCAPE;

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
        mIsInitialized = false;
        mIsSent = false;

        X_DELTA_JOYSTICK_HEAD = joystickHead.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK_HEAD = joystickHead.parent.GetComponent<RectTransform>().sizeDelta.y;
        X_DELTA_JOYSTICK_BODY = joystickBody.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK_BODY = joystickBody.parent.GetComponent<RectTransform>().sizeDelta.y;

        X_DELTA_JOYSTICK_HEAD_LANDSCAPE = joystickHeadLandscape.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK_HEAD_LANDSCAPE = joystickHeadLandscape.parent.GetComponent<RectTransform>().sizeDelta.y;
        X_DELTA_JOYSTICK_BODY_LANDSCAPE = joystickBodyLandscape.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK_BODY_LANDSCAPE = joystickBodyLandscape.parent.GetComponent<RectTransform>().sizeDelta.y;
    }

    void Update()
    {
        if (!isActiveAndEnabled || !oto.HasAPeer || Time.time - mTime < 0.1) return;

        if (Screen.orientation == ScreenOrientation.LandscapeLeft
            || Screen.orientation == ScreenOrientation.LandscapeRight) {
            mXPositionHead = joystickHeadLandscape.localPosition.x / X_DELTA_JOYSTICK_HEAD_LANDSCAPE;
            mYPositionHead = joystickHeadLandscape.localPosition.y / Y_DELTA_JOYSTICK_HEAD_LANDSCAPE;

            mXPositionBody = joystickBodyLandscape.localPosition.x / X_DELTA_JOYSTICK_BODY_LANDSCAPE;
            mYPositionBody = joystickBodyLandscape.localPosition.y / Y_DELTA_JOYSTICK_BODY_LANDSCAPE;
        }
        else {
            mXPositionHead = joystickHead.localPosition.x / X_DELTA_JOYSTICK_HEAD;
            mYPositionHead = joystickHead.localPosition.y / Y_DELTA_JOYSTICK_HEAD;

            mXPositionBody = joystickBody.localPosition.x / X_DELTA_JOYSTICK_BODY;
            mYPositionBody = joystickBody.localPosition.y / Y_DELTA_JOYSTICK_BODY;
        }

        if (mXPositionHead != 0 && mYPositionHead != 0) {
            Debug.Log("Computing Head movement");
            ComputeNoAxis();
            ComputeYesAxis();
            
            byte[] lNoCmd = new SetPosNoCmd(mAngleNo, mNoSpeed).Serialize();
            byte[] lYesCmd = new SetPosYesCmd(mAngleYes, mYesSpeed).Serialize();

            SendData(lNoCmd, lNoCmd.Length);
            SendData(lYesCmd, lYesCmd.Length);
        }

        if (mXPositionBody != 0 && mYPositionBody != 0) {
            Debug.Log("Computing Body movement");
            ComputeMobileBase();
            byte[] lMobileCmd = new SetWheelsSpeedCmd(mLeftSpeed, mRightSpeed, 100).Serialize();
            SendData(lMobileCmd, lMobileCmd.Length);
        }

        mTime = Time.time;
    }

    private void ComputeNoAxis()
    {
        Debug.Log("X Head position : " + mXPositionHead);
        mAngleNo -= mXPositionHead * 5f;

        if (Mathf.Abs(mAngleNo) > 45)
            mAngleNo = Mathf.Sign(mAngleNo) * 45;

        mHeadNoBar.value = -mAngleNo;
        mNoSpeed = (mXPositionHead * mXPositionHead) * mSpeedHead * 3f;
    }

    private void ComputeYesAxis()
    {
        Debug.Log("Y Head position : " + mYPositionBody);
        mAngleYes -= mYPositionHead * 5f;

        if (mAngleYes < -30)
            mAngleYes = -30;

        if (mAngleYes > 60)
            mAngleYes = 60;

        mYesSpeed = (mYPositionHead * mYPositionHead) * mSpeedHead * 3f;
    }

    private void ComputeMobileBase()
    {
        float lRadius = Mathf.Sqrt(mXPositionBody * mXPositionBody + mYPositionBody * mYPositionBody);
        float lAngle = (Mathf.Atan2(mYPositionBody, mXPositionBody));
        Debug.Log("Body position radius : " + lRadius + " / body position angle : " + lAngle);
        mLeftSpeed = mSpeedBody * (Mathf.Sin(lAngle) + Mathf.Cos(lAngle)/3) * lRadius;
        mRightSpeed = mSpeedBody * (Mathf.Sin(lAngle) - Mathf.Cos(lAngle)/3) * lRadius;
    }
}
