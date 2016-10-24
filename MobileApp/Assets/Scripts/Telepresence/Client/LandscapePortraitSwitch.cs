using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LandscapePortraitSwitch : MonoBehaviour {

    private bool  mOnSwitch = false;
    [SerializeField]
    private GameObject mToggle;
    [SerializeField]
    private GameObject mJoystickHead;
    [SerializeField]
    private GameObject mJoystickBody;
    [SerializeField]
    private GameObject mJoystickHeadLandscape;
    [SerializeField]
    private GameObject mJoystickBodyLandscape;

    public bool mIsInitialized=false;

    void Start()
    {
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            mToggle.SetActive(true);
            mJoystickBody.SetActive(true);
            mJoystickHead.SetActive(false);
            mJoystickBodyLandscape.SetActive(false);
            mJoystickHeadLandscape.SetActive(false);
        }
        else
        {
            mToggle.SetActive(false);
            mJoystickBody.SetActive(false);
            mJoystickHead.SetActive(false);
            mJoystickBodyLandscape.SetActive(true);
            mJoystickHeadLandscape.SetActive(true);
        }
    }
    void Update()
    {
        //if (mIsInitialized)
        //{
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                mToggle.SetActive(false);
                mJoystickBody.SetActive(false);
                mJoystickHead.SetActive(false);

                mJoystickHeadLandscape.SetActive(true);
                mJoystickBodyLandscape.SetActive(true);
                mOnSwitch = true;
            }
            else
            {
                mJoystickHeadLandscape.SetActive(false);
                mJoystickBodyLandscape.SetActive(false);
                mToggle.SetActive(true);
                if (mOnSwitch)
                {
                    mJoystickBody.SetActive(true);
                    mJoystickHead.SetActive(false);
                    if (mToggle.GetComponent<ToggleController>().state)
                    {
                        mToggle.GetComponent<ToggleController>().toggle();
                    }
                    mOnSwitch = false;
                }
            }

        //}
    }
}
