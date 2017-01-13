using UnityEngine;

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
