using UnityEngine;

/// <summary>
/// Manages the Option Menu in the Remote Control window.
/// </summary>
public class ShowControlMenu : MonoBehaviour {

    [SerializeField]
    private GameObject listedMenu;

    [SerializeField]
    private Animator menuAnimator;

    private bool mShowingListedMenu;

    void Start()
    {
        mShowingListedMenu = false;
    }

    /// <summary>
    /// Show the list menu for the remote control.
    /// </summary>
	public void ShowListedMenu()
    {
        mShowingListedMenu = !mShowingListedMenu;
        listedMenu.SetActive(mShowingListedMenu);
        if (mShowingListedMenu)
            menuAnimator.SetTrigger("Open");
        else
            menuAnimator.SetTrigger("Close");
    }
}
