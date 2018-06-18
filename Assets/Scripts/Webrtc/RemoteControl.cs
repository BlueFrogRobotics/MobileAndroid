using UnityEngine;
using UnityEngine.UI;

using Buddy.Command;
using System;
using UnityEngine.EventSystems;

public class RemoteControl : MonoBehaviour
{

    [SerializeField]
    private Transform joystick;

    [SerializeField]
    private Webrtc webRTC;

    [SerializeField]
    private ToggleController toggleController;

    [SerializeField]
    private Animator headNoAnim;

    [SerializeField]
    private Animator headYesAnim;

    [SerializeField]
    private GameObject leftSensor;

    [SerializeField]
    private GameObject middleSensor;

    [SerializeField]
    private GameObject rightSensor;

    [SerializeField]
    private GameObject backSensor;

    [SerializeField]
    private Animator animatorWOZ;

    [SerializeField]
    private Animator animatorWOZTitle;

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

    private Text mTextToEdit;

    private bool mControlsDisabled = false;
    private bool mAlreadyDisabled = false;
    private bool mEdit = false;

    private TouchScreenKeyboard keyboard;

    public bool ControlsDisabled { get { return mControlsDisabled; } set { mControlsDisabled = value; } }

    void OnEnable()
    {
        mTime = Time.time;

        X_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.y;
    }

    void Update()
    {
        if (keyboard != null && keyboard.done)
        {
            Debug.Log("EDIT = " + mEdit);
            if (mEdit)
            {
                byte[] lCmd = null;
                lCmd = new SayTTSCmd(keyboard.text).Serialize();
                webRTC.SendWithDataChannel(GetString(lCmd));
                keyboard = null;
                mEdit = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(keyboard.text))
                    mTextToEdit.text = keyboard.text;
                Debug.Log("Update " + mTextToEdit);
            }
        }
        //The goal here is to get the position of the cursor in the relative plane of the joystick
        if (!isActiveAndEnabled || Time.time - mTime < 0.2F) return;

        mXPosition = joystick.localPosition.x / X_DELTA_JOYSTICK;
        mYPosition = joystick.localPosition.y / Y_DELTA_JOYSTICK;

        if (mControlsDisabled)
        {
            if (!mAlreadyDisabled)
            {
                Debug.Log("BAD CONNECTION, stopping Buddy...");
                webRTC.SendWithDataChannel(GetString(new SetWheelsSpeedCmd(0, 0, 200).Serialize()));
                mAlreadyDisabled = true;
            }
        }
        else
        {
            mAlreadyDisabled = false;

            if (mXPosition != 0 && mYPosition != 0)
            {
                //The cursor of the joystick is being moved
                //We are controlling the body movement
                if (toggleController.IsBodyActive)
                {
                    //Compute the desired body movement and send the serialized command to remote
                    ComputeMobileBase();
                    byte[] lMobileCmd = new SetWheelsSpeedCmd(mLeftSpeed, mRightSpeed, 200).Serialize();

                    webRTC.SendWithDataChannel(GetString(lMobileCmd));
                }
                //We are controlling the head movement
                else
                {
                    //Compute the desired head movement and send the serialized command to remote
                    ComputeNoAxis();
                    ComputeYesAxis();

                    byte[] lNoCmd = new SetPosNoCmd(mAngleNo, mNoSpeed).Serialize();
                    byte[] lYesCmd = new SetPosYesCmd(mAngleYes, mYesSpeed).Serialize();

                    webRTC.SendWithDataChannel(GetString(lNoCmd));
                    webRTC.SendWithDataChannel(GetString(lYesCmd));
                }
            }
        }

        mTime = Time.time;
    }

    public void ButtonPushed(string iButtonName)
    {
        string[] words = iButtonName.Split(' ');
        byte[] lCmd = null;
        switch (words[0])
        {
            case "App":
                Debug.Log(words[1]);
                lCmd = new StartAppCmd(words[1]).Serialize();

                animatorWOZ.SetTrigger("OFF");
                animatorWOZTitle.SetTrigger("OFF");
                break;

            case "Sound":
                var text = EventSystem.current.currentSelectedGameObject.transform.Find("Text");
                Debug.Log(text.GetComponent<Text>().text);
                if (text != null)
                    lCmd = new SayTTSCmd(text.GetComponent<Text>().text).Serialize();

                break;

            case "Mood":
                Debug.Log(words[1].ToUpper());
                lCmd = new SetMoodCmd((Buddy.MoodType)Enum.Parse(typeof(Buddy.MoodType), words[1].ToUpper())).Serialize();
                break;

            case "BML":
                Debug.Log(words[1].ToUpper());
                lCmd = new LaunchRandomBMLCmd((Buddy.MoodType)Enum.Parse(typeof(Buddy.MoodType), words[1].ToUpper())).Serialize();
                break;

            default:
                break;

        }
        webRTC.SendWithDataChannel(GetString(lCmd));
    }

    public void SayInputText()
    {

        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        mEdit = true;
    }

    public void EditText()
    {
        mEdit = false;
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        mTextToEdit = EventSystem.current.currentSelectedGameObject.transform.Find("Text").GetComponent<Text>();
        Debug.Log(mTextToEdit.text);
    }

    private void ComputeNoAxis()
    {
        //Add an increment to No axis position corresponding to the cursor's
        mAngleNo -= mXPosition * 5f;

        if (Mathf.Abs(mAngleNo) > 45)
            mAngleNo = Mathf.Sign(mAngleNo) * 45;

        headNoAnim.SetFloat("HeadPosition_H", -mAngleNo * 100 / 45);
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

        headYesAnim.SetFloat("HeadPosition_V", mAngleYes);
        mYesSpeed = (mYPosition * mYPosition) * mSpeedHead * 3f;
    }

    private void ComputeMobileBase()
    {
        //Compute the speed of both wheels according to the cursors position;
        //The direction is thus influenced by the speed of both wheels
        float lRadius = Mathf.Sqrt(mXPosition * mXPosition + mYPosition * mYPosition);
        float lAngle = (Mathf.Atan2(mYPosition, mXPosition));
        Debug.Log("Body position radius : " + lRadius + " / body position angle : " + lAngle);
        mLeftSpeed = mSpeedBody * (Mathf.Sin(lAngle) + Mathf.Cos(lAngle) / 3) * lRadius;
        mRightSpeed = mSpeedBody * (Mathf.Sin(lAngle) - Mathf.Cos(lAngle) / 3) * lRadius;
    }

    // This function is called when a webrtc data is received
    public void onMessage(string iMessage)
    {
        //We use this function to receive sensor data and display it
        byte[] lData = System.Convert.FromBase64String(iMessage);

        if (lData.Length != 4)
            return;

        leftSensor.GetComponent<ObstacleManager>().lvl = lData[0];
        middleSensor.GetComponent<ObstacleManager>().lvl = lData[1];
        rightSensor.GetComponent<ObstacleManager>().lvl = lData[2];
        backSensor.GetComponent<ObstacleManager>().lvl = lData[3];
    }

    private string GetString(byte[] iBytes)
    {
        return System.Convert.ToBase64String(iBytes);
    }
}
