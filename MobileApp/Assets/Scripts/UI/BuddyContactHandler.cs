using UnityEngine;
using UnityEngine.UI;

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
        if(mTogglePreviousState != mParentToggle.isOn) {
            mTogglePreviousState = mParentToggle.isOn;

            if (mTogglePreviousState == true)
                mBuddyName.color = mSelectedColor;
            else
                mBuddyName.color = mUnselectedColor;
        }
    }
}
