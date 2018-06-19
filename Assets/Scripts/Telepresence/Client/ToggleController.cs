using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI to switch between Body and Head control.
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
    private Color mWhiteColor = new Color(1F, 1F, 1F);
    private Color mBlackColor = new Color(0.2F, 0.2F, 0.2F);

    /// <summary>
    /// Disable and enable the proper UI elements to switch from body to head movement.
    /// </summary>
    private void MoveToHead()
    {
        Debug.Log("Switch to Head");
        mIsOnBody = false;
        imageBody.SetActive(false);
        imageHead.SetActive(true);
        headText.color = mBlackColor;
        bodyText.color = mWhiteColor;
        toggleAnim.SetTrigger("Head");
    }

    /// <summary>
    /// Disable and enable the proper UI elements to switch from head to body movement.
    /// </summary>
    private void MoveToBody()
    {
        Debug.Log("Switch to Body");
        mIsOnBody = true;
        imageBody.SetActive(true);
        imageHead.SetActive(false);
        headText.color = mWhiteColor;
        bodyText.color = mBlackColor;
        toggleAnim.SetTrigger("Body");
    }

    /// <summary>
    /// Triggers the switch between the Body and Head control.
    /// </summary>
    public void ToggleActivate()
    {
        Debug.Log("Toggled");
        mIsOnBody = !mIsOnBody;
        if (mIsOnBody)
            MoveToBody();
        else
            MoveToHead();
    }
}
