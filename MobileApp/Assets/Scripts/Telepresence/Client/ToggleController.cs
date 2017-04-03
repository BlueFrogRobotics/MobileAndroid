using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI to switch between Body and Head control
/// </summary>
public class ToggleController : MonoBehaviour {
    
    public bool IsBodyActive { get { return mIsOnBody; } }

    [SerializeField]
    private GameObject imageHead;

    [SerializeField]
    private GameObject imageBody;

    [SerializeField]
    private Text headText;

    [SerializeField]
    private Text bodyText;

    [SerializeField]
    private Animator toggleAnim;
    
    private bool mIsOnBody = true;
    private Color mWhiteColor = new Color(255F, 255F, 255F, 255F);
    private Color mBlackColor = new Color(50F, 50F, 50F, 255F);

    //Disable and enable the proper UI elements
    private void MoveToHead()
    {
        mIsOnBody = false;
        imageBody.SetActive(false);
        imageHead.SetActive(true);
        headText.color = mWhiteColor;
        bodyText.color = mBlackColor;
        toggleAnim.SetTrigger("Head");
    }

    //Disable and enable the proper UI elements
    private void MoveToBody()
    {
        mIsOnBody = true;
        imageBody.SetActive(true);
        imageHead.SetActive(false);
        headText.color = mBlackColor;
        bodyText.color = mWhiteColor;
        toggleAnim.SetTrigger("Body");
    }

    //Triggers the switch between the Body and Head control
    public void ToggleActivate()
    {
        mIsOnBody = !mIsOnBody;
        if (mIsOnBody)
            MoveToBody();
        else
            MoveToHead();
    }
}
