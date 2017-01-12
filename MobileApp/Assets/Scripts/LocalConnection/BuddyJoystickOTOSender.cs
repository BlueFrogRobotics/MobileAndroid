using UnityEngine;
using UnityEngine.UI;
using BuddyOS.Command;

public class BuddyJoystickOTOSender : OTONetSender
{
    [SerializeField]
    private Transform joystick;

    [SerializeField]
    private ToggleController toggleController;

    [SerializeField]
    private OTONetwork oto;

    [SerializeField]
    private Animator headNoAnim;

    [SerializeField]
    private Animator headYesAnim;

    //Network
    private byte[] mSentData;
    private bool mIsSent;
    private bool mIsInitialized;
    private float mTime;
    
    //Joysticks' var
    private float mXPosition;
    private float mYPosition;

    private float X_DELTA_JOYSTICK;
    private float Y_DELTA_JOYSTICK;
    //private float X_DELTA_JOYSTICK_HEAD_LANDSCAPE;
    //private float Y_DELTA_JOYSTICK_HEAD_LANDSCAPE;
    //private float X_DELTA_JOYSTICK_BODY_LANDSCAPE;
    //private float Y_DELTA_JOYSTICK_BODY_LANDSCAPE;

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

        X_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.y;
        //X_DELTA_JOYSTICK_BODY = joystickBody.parent.GetComponent<RectTransform>().sizeDelta.x;
        //Y_DELTA_JOYSTICK_BODY = joystickBody.parent.GetComponent<RectTransform>().sizeDelta.y;

        //X_DELTA_JOYSTICK_HEAD_LANDSCAPE = joystickHeadLandscape.parent.GetComponent<RectTransform>().sizeDelta.x;
        //Y_DELTA_JOYSTICK_HEAD_LANDSCAPE = joystickHeadLandscape.parent.GetComponent<RectTransform>().sizeDelta.y;
        //X_DELTA_JOYSTICK_BODY_LANDSCAPE = joystickBodyLandscape.parent.GetComponent<RectTransform>().sizeDelta.x;
        //Y_DELTA_JOYSTICK_BODY_LANDSCAPE = joystickBodyLandscape.parent.GetComponent<RectTransform>().sizeDelta.y;
    }

    void Update()
    {
        if (!isActiveAndEnabled ||  Time.time - mTime < 0.1) return;

        if (Screen.orientation == ScreenOrientation.LandscapeLeft
            || Screen.orientation == ScreenOrientation.LandscapeRight) {
            //mXPositionHead = joystickHeadLandscape.localPosition.x / X_DELTA_JOYSTICK_HEAD_LANDSCAPE;
            //mYPositionHead = joystickHeadLandscape.localPosition.y / Y_DELTA_JOYSTICK_HEAD_LANDSCAPE;

            //mXPositionBody = joystickBodyLandscape.localPosition.x / X_DELTA_JOYSTICK_BODY_LANDSCAPE;
            //mYPositionBody = joystickBodyLandscape.localPosition.y / Y_DELTA_JOYSTICK_BODY_LANDSCAPE;
        }
        else {
            mXPosition = joystick.localPosition.x / X_DELTA_JOYSTICK;
            mYPosition = joystick.localPosition.y / Y_DELTA_JOYSTICK;
        }

        if (mXPosition != 0 && mYPosition != 0) {
            Debug.Log("Joystick moved");
            if(toggleController.IsBodyActive) {
                ComputeMobileBase();
                byte[] lMobileCmd = new SetWheelsSpeedCmd(mLeftSpeed, mRightSpeed, 100).Serialize();
                SendData(lMobileCmd, lMobileCmd.Length);
            } else {
                //Debug.Log("Computing Head movement");
                ComputeNoAxis();
                ComputeYesAxis();

                byte[] lNoCmd = new SetPosNoCmd(mAngleNo, mNoSpeed).Serialize();
                byte[] lYesCmd = new SetPosYesCmd(mAngleYes, mYesSpeed).Serialize();

                SendData(lNoCmd, lNoCmd.Length);
                SendData(lYesCmd, lYesCmd.Length);
            }            
        }

        mTime = Time.time;
    }

    private void ComputeNoAxis()
    {
        //Debug.Log("X Head position : " + mXPosition);
        mAngleNo -= mXPosition * 5f;

        if (Mathf.Abs(mAngleNo) > 45)
            mAngleNo = Mathf.Sign(mAngleNo) * 45;
        
        headNoAnim.SetFloat("HeadPosition_H", -mAngleNo*100/45);
        mNoSpeed = (mXPosition * mXPosition) * mSpeedHead * 3f;
    }

    private void ComputeYesAxis()
    {
        //Debug.Log("Y Head position : " + mYPosition);
        mAngleYes -= mYPosition * 5f;

        if (mAngleYes < -30)
            mAngleYes = -30;

        if (mAngleYes > 60)
            mAngleYes = 60;

        headYesAnim.SetFloat("HeadPosition_V", mAngleYes);
        mYesSpeed = (mYPosition * mYPosition) * mSpeedHead * 3f;
    }

    private void ComputeMobileBase()
    {
        float lRadius = Mathf.Sqrt(mXPosition * mXPosition + mYPosition * mYPosition);
        float lAngle = (Mathf.Atan2(mYPosition, mXPosition));
        Debug.Log("Body position radius : " + lRadius + " / body position angle : " + lAngle);
        mLeftSpeed = mSpeedBody * (Mathf.Sin(lAngle) + Mathf.Cos(lAngle)/3) * lRadius;
        mRightSpeed = mSpeedBody * (Mathf.Sin(lAngle) - Mathf.Cos(lAngle)/3) * lRadius;
    }
}
