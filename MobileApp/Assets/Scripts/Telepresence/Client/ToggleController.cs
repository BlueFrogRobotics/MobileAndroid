using UnityEngine;
using UnityEngine.UI;

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

    private void MoveToHead()
    {
        mIsOnBody = false;
        imageBody.SetActive(false);
        imageHead.SetActive(true);
        headText.color = mBlackColor;
        bodyText.color = mWhiteColor;
        toggleAnim.SetTrigger("Head");
    }

    private void MoveToBody()
    {
        mIsOnBody = true;
        imageBody.SetActive(true);
        imageHead.SetActive(false);
        headText.color = mWhiteColor;
        bodyText.color = mBlackColor;
        toggleAnim.SetTrigger("Body");
    }

    public void ToggleActivate()
    {
        mIsOnBody = !mIsOnBody;
        if (mIsOnBody)
            MoveToBody();
        else
            MoveToHead();
    }
}
