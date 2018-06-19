using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Buddy Contact prefab for color display depending on a Buddy being selected or not.
/// This script is attached to each "Buddy contact" game object.
/// </summary>
public class BuddyContactHandler : MonoBehaviour
{
    public bool IsOn { get; private set; }
    private Text mBuddyName;
    private Color mSelectedColor;
    private Color mUnselectedColor;

    void Start()
    {
        IsOn = false;
        mBuddyName = GetComponent<Text>();
        mSelectedColor = new Color(0, 0.83f, 0.81f);
        mUnselectedColor = Color.white;
    }

    /// <summary>
    /// Called when a buddy contact is being pressed.
    /// </summary>
    public void ToggleChanged()
    {
        // We change the color of Buddy's name to indicate which Buddy has been selected.
        IsOn = !IsOn;
        if (IsOn)
            mBuddyName.color = mSelectedColor;
        else
            mBuddyName.color = mUnselectedColor;

        Transform lParentTransform = gameObject.transform.parent;
        int lSiblingIndex = lParentTransform.GetSiblingIndex();
        int lCount = 0;

        Transform lScrollTransf = lParentTransform.parent;

        // Untoggle the other Buddies.
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
        IsOn = false;
        mBuddyName.color = mUnselectedColor;
    }
}
