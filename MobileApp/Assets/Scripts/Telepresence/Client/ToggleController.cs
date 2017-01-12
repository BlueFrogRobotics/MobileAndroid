using UnityEngine;

public class ToggleController : MonoBehaviour {
    
    public bool IsBodyActive { get { return mIsOnBody; } }

    [SerializeField]
    private GameObject imageHead;

    [SerializeField]
    private GameObject imageBody;

    [SerializeField]
    private Animator toggleAnim;
    
    private bool mIsOnBody = true;

    public void MoveToHead()
    {
        mIsOnBody = false;
        imageBody.SetActive(false);
        imageHead.SetActive(true);
        toggleAnim.SetTrigger("Head");
    }

    public void MoveToBody()
    {
        mIsOnBody = true;
        imageBody.SetActive(true);
        imageHead.SetActive(false);
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
