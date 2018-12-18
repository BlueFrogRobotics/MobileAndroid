using UnityEngine;

using BlueQuark.Remote;

/// <summary>
/// Controls the movement of the robot on a local connection using a joystick.
/// </summary>
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
    private float mTime;
    
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
        if (!isActiveAndEnabled ||  Time.time - mTime < 0.1) return;

        if (Screen.orientation == ScreenOrientation.LandscapeLeft
            || Screen.orientation == ScreenOrientation.LandscapeRight) {
            //Landscape mode is not handled yet, so do nothing;
        }
        else {
            mXPosition = joystick.localPosition.x / X_DELTA_JOYSTICK;
            mYPosition = joystick.localPosition.y / Y_DELTA_JOYSTICK;
            Debug.LogWarning("------ JOYSTICK LOCAL:" + joystick.localPosition.ToString() + " -------");
        }

        if (mXPosition != 0 && mYPosition != 0) {
            Debug.LogWarning("----------- X/Y POSITION != 0 ------------");
            //The cursor of the joystick is being moved
            //We are controlling the body movement
            if(toggleController.IsBodyActive) {
                //Compute the desired body movement and send the serialized command to remote
                ComputeMobileBase();
                byte[] lMobileCmd = new SetWheelsVelocitiesCmd(mLeftSpeed, mRightSpeed).Serialize();
                SendData(lMobileCmd, lMobileCmd.Length);
            }
            //We are controlling the head movement
            else {
                //Compute the desired head movement and send the serialized command to remote
                ComputeNoAxis();
                ComputeYesAxis();

                byte[] lNoCmd = new SetNoHingePositionCmd(mAngleNo, mNoSpeed).Serialize();
                byte[] lYesCmd = new SetYesHingePositionCmd(mAngleYes, mYesSpeed).Serialize();

                SendData(lNoCmd, lNoCmd.Length);
                SendData(lYesCmd, lYesCmd.Length);
            }            
        }
        else if (mXPosition == 0 && mYPosition == 0) {
            Debug.LogWarning("------- POSITION SET TO ZERO IN SENDER --------");
        }

        mTime = Time.time;
    }

    private void ComputeNoAxis()
    {
        //Add an increment to No axis position corresponding to the cursor's
        mAngleNo -= mXPosition * 5f;

        //Block the no axis to -45° or 45°.
        if (Mathf.Abs(mAngleNo) > 45)
            mAngleNo = Mathf.Sign(mAngleNo) * 45;
        
        Debug.LogWarning("---- NO AXIS: " + mAngleNo.ToString() + " ----");
        Debug.LogWarning("---- NO AXIS CALCUL: " + (-mAngleNo * 100 / 45).ToString() + " ----");

        headNoAnim.SetFloat("HeadPosition_H", -mAngleNo*100/45);
        mNoSpeed = (mXPosition * mXPosition) * mSpeedHead * 3f;
    }

    private void ComputeYesAxis()
    {
        //Add an increment to Yes axis position corresponding to the cursor's
        mAngleYes -= mYPosition * 5f;

        //Block the yes axis to -30° minimum and 60° maximum.
        if (mAngleYes < -30)
            mAngleYes = -30;

        if (mAngleYes > 60)
            mAngleYes = 60;

        Debug.LogWarning("---- YES AXIS: " + mAngleYes.ToString() + " ----");

        headYesAnim.SetFloat("HeadPosition_V", mAngleYes);
        mYesSpeed = (mYPosition * mYPosition) * mSpeedHead * 3f;
    }

    private void ComputeMobileBase()
    {
        //Compute the speed of both wheels according to the cursors position;
        //The direction is thus influences by the speed of both wheels
        float lRadius = Mathf.Sqrt(mXPosition * mXPosition + mYPosition * mYPosition);
        float lAngle = (Mathf.Atan2(mYPosition, mXPosition));
        Debug.Log("Body position radius : " + lRadius + " / body position angle : " + lAngle);
        //A little oval representation of the speed, as we don't want the robot to turn too quickly.
        mLeftSpeed = mSpeedBody * (Mathf.Sin(lAngle) + Mathf.Cos(lAngle)/3) * lRadius;
        mRightSpeed = mSpeedBody * (Mathf.Sin(lAngle) - Mathf.Cos(lAngle)/3) * lRadius;
    }
}
