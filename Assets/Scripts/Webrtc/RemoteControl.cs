using UnityEngine;
using UnityEngine.UI;

using Buddy.Command;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// Manager for the remote control session.
/// Sends the movement commands to the robot and displays data from Buddy's sensors.
/// All commands are sent via the data channel.
/// From the device to Buddy: Movement (body and head), TTS, Mood change commands.
/// From Buddy to the device: Sensor data.
/// </summary>
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
    private InputField inputText;

    public bool ControlsDisabled { get { return mControlsDisabled; } set { mControlsDisabled = value; } }

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

    private bool mControlsDisabled = false;
    private bool mAlreadyDisabled = false;

    private TouchScreenKeyboard keyboard;

    void OnEnable()
    {
        mTime = Time.time;

        // Get the X and Y delta of the joystick's movement.
        X_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.x;
        Y_DELTA_JOYSTICK = joystick.parent.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Send movement commands at every update.
    void Update()
    {
        // If the keyboard text input is finished, send a TTS command to Buddy.
        if (keyboard != null && keyboard.done)
        {
            byte[] lCmd = null;
            lCmd = new SayTTSCmd(keyboard.text).Serialize();
            webRTC.SendWithDataChannel(GetString(lCmd));
            keyboard = null;
        }

        // Get the position of the cursor in the relative plane of the joystick.
        if (!isActiveAndEnabled || Time.time - mTime < 0.2F)
            return;

        mXPosition = joystick.localPosition.x / X_DELTA_JOYSTICK;
        mYPosition = joystick.localPosition.y / Y_DELTA_JOYSTICK;

        // If the controls are disabled, stop moving Buddy.
        if (mControlsDisabled) {
            if (!mAlreadyDisabled) {
                Debug.Log("BAD CONNECTION, stopping Buddy...");
                webRTC.SendWithDataChannel(GetString(new SetWheelsSpeedCmd(0, 0, 200).Serialize()));
                mAlreadyDisabled = true;
            }
        }
        else {
            mAlreadyDisabled = false;

            if (mXPosition != 0 && mYPosition != 0) {
                // The cursor of the joystick is being moved.
                // Controlling the body movement.
                if (toggleController.IsBodyActive) {
                    //Compute the desired body movement and send the serialized command to remote.
                    ComputeMobileBase();
                    byte[] lMobileCmd = new SetWheelsSpeedCmd(mLeftSpeed, mRightSpeed, 200).Serialize();

                    webRTC.SendWithDataChannel(GetString(lMobileCmd));
                }
                // Controlling the head movement.
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
        }

        mTime = Time.time;
    }

    /// <summary>
    /// Called when a button is pressed in Wizard of Oz mode.
    /// Can either launch an application, play a sound, change mood or launch the execution of a BML.
    /// </summary>
    /// <param name="iButtonName">Name of the button pressed.</param>
    public void ButtonPushed(string iButtonName)
    {
        // Parse the input and launch the corresponding command.
        string[] words = iButtonName.Split(' ');
        byte[] lCmd = null;
        switch (words[0]) {
            case "App":
                lCmd = new StartAppCmd(words[1]).Serialize();
                break;

            case "Sound":
                var text = EventSystem.current.currentSelectedGameObject.transform.Find("Text");
                if (text != null)
                    lCmd = new SayTTSCmd(text.GetComponent<Text>().text).Serialize();
                break;

            case "Mood":
                Debug.Log(words[1].ToUpper());
                lCmd = new SetMoodCmd((Buddy.MoodType)Enum.Parse(typeof(Buddy.MoodType), words[1].ToUpper())).Serialize();
                break;

            case "BML":
                lCmd = new LaunchRandomBMLCmd((Buddy.MoodType)Enum.Parse(typeof(Buddy.MoodType), words[1].ToUpper())).Serialize();
                break;

            default:
                break;
        }
        webRTC.SendWithDataChannel(GetString(lCmd));
    }

    /// <summary>
    /// Test method I guess ?
    /// </summary>
    /// <param name="text">The input to be spoken by Buddy.</param>
    public void SayInputText(string text)
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

        Debug.Log(keyboard.text);

        //Debug.Log(text);
        //byte[] lCmd = null;
        //lCmd = new SayTTSCmd(/*inputText.text*/text).Serialize();
        //webRTC.SendWithDataChannel(GetString(lCmd));
    }

    /// <summary>
    /// Compute the movement of the no head axis.
    /// </summary>
    private void ComputeNoAxis()
    {
        //Add an increment to the No axis position corresponding to the cursor's position.
        mAngleNo -= mXPosition * 5f;

        if (Mathf.Abs(mAngleNo) > 45)
            mAngleNo = Mathf.Sign(mAngleNo) * 45;

        // Set the head position indicator on the animator.
        headNoAnim.SetFloat("HeadPosition_H", -mAngleNo * 100 / 45);
        mNoSpeed = (mXPosition * mXPosition) * mSpeedHead * 3f;
    }

    /// <summary>
    /// Compute the movement of the yes head axis.
    /// </summary>
    private void ComputeYesAxis()
    {
        //Add an increment to the Yes axis position corresponding to the cursor's position.
        mAngleYes -= mYPosition * 5f;

        if (mAngleYes < -30)
            mAngleYes = -30;

        if (mAngleYes > 60)
            mAngleYes = 60;

        // Set the head position indicator on the animator.
        headYesAnim.SetFloat("HeadPosition_V", mAngleYes);
        mYesSpeed = (mYPosition * mYPosition) * mSpeedHead * 3f;
    }

    /// <summary>
    /// Compute the body movement.
    /// </summary>
    private void ComputeMobileBase()
    {
        //Compute the speed of both wheels according to the cursor's position.
        //The direction is thus influenced by the speed of both wheels.
        float lRadius = Mathf.Sqrt(mXPosition * mXPosition + mYPosition * mYPosition);
        float lAngle = (Mathf.Atan2(mYPosition, mXPosition));
        Debug.Log("Body position radius : " + lRadius + " / body position angle : " + lAngle);
        // Oval representation of the speed to make smoother turns.
        mLeftSpeed = mSpeedBody * (Mathf.Sin(lAngle) + Mathf.Cos(lAngle) / 3) * lRadius;
        mRightSpeed = mSpeedBody * (Mathf.Sin(lAngle) - Mathf.Cos(lAngle) / 3) * lRadius;
    }

    /// <summary>
    /// Called by the WebRTC Java plugin when data was received through the data channel from the remote peer.
    /// </summary>
    /// <param name="iMessage">Message received on the data channel.</param>
    public void onMessage(string iMessage)
    {
        //We use this function to receive sensor data and display it with the obstacle sprites.
        byte[] lData = System.Convert.FromBase64String(iMessage);

        if (lData.Length != 4)
            return;

        leftSensor.GetComponent<ObstacleManager>().lvl = lData[0];
        middleSensor.GetComponent<ObstacleManager>().lvl = lData[1];
        rightSensor.GetComponent<ObstacleManager>().lvl = lData[2];
        backSensor.GetComponent<ObstacleManager>().lvl = lData[3];
    }

    /// <summary>
    /// Get the string described by an array of byte.
    /// </summary>
    /// <param name="iBytes">The byte array to convert to a string.</param>
    /// <returns>The string described by the byte array.</returns>
    private string GetString(byte[] iBytes)
    {
        return System.Convert.ToBase64String(iBytes);
    }
}
