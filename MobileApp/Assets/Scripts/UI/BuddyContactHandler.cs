using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Buddy Contact prefab for color display depending on a Buddy being selected or not
/// </summary>
public class BuddyContactHandler : MonoBehaviour
{
    private bool mIsOn;
    private Text mBuddyName;
    private Color mSelectedColor;
    private Color mUnselectedColor;

    void Start()
    {
        mIsOn = false;
        mBuddyName = GetComponent<Text>();
        mSelectedColor = new Color(0, 0.83f, 0.81f);
        mUnselectedColor = Color.white;
    }

    public void ToggleChanged()
    {
        mIsOn = !mIsOn;
        if (mIsOn)
            mBuddyName.color = mSelectedColor;
        else
            mBuddyName.color = mUnselectedColor;

        Transform lParentTransform = gameObject.transform.parent;
        int lSiblingIndex = lParentTransform.GetSiblingIndex();
        int lCount = 0;

        Transform lScrollTransf = lParentTransform.parent;

        foreach(Transform lSibling in lScrollTransf) {
            if(lSiblingIndex == lCount 
                || lSibling.gameObject.name == "LocalSeparator" 
                || lSibling.gameObject.name == "DistantSeparator" 
                || lSibling.gameObject.name == "SearchingContact") {
                lCount++;
                continue;
            }
            lSibling.gameObject.GetComponentInChildren<BuddyContactHandler>().UnToggle();
            lCount++;
        }
    }

    public void UnToggle()
    {
        mIsOn = false;
        mBuddyName.color = mUnselectedColor;
    }
}
