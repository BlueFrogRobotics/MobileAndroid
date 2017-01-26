using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Buddy Contact prefab for color display depending on a Buddy being selected or not
/// </summary>
public class BuddyContactHandler : MonoBehaviour
{
    private bool mTogglePreviousState;
    private Text mBuddyName;
    private Color mSelectedColor;
    private Color mUnselectedColor;
    private Toggle mParentToggle;

    void Start()
    {
        mParentToggle = GetComponentInParent<Toggle>();
        mBuddyName = GetComponent<Text>();
        mSelectedColor = new Color(0, 0.83f, 0.81f);
        mUnselectedColor = Color.white;
        mTogglePreviousState = mParentToggle.isOn;
    }

    void Update()
    {
        //Check if toggle state has changed
        if(mTogglePreviousState != mParentToggle.isOn) {
            mTogglePreviousState = mParentToggle.isOn;

            //Set the color matching the toggle state
            if (mTogglePreviousState == true)
                mBuddyName.color = mSelectedColor;
            else
                mBuddyName.color = mUnselectedColor;
        }
    }
}
