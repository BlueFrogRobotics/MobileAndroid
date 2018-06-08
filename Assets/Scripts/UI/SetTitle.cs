using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTitle : MonoBehaviour
{

    [SerializeField]
    private Text Previous;
    [SerializeField]
    private Text Current;
    [SerializeField]
    private Text Next;

    private Animator mAnimator;


    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (mAnimator.GetBool("IsActive"))
        {
            if (mAnimator.GetBool("Emoji"))
            {
                Current.text = "SET MOOD";
                if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("EmojiSpeach"))
                {
                    Next.text = "SET SPEECH";
                }
            }
            else if (mAnimator.GetBool("Speech"))
            {
                Current.text = "SET SPEECH";
                if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("SpeachEmoji"))
                {
                    Next.text = "SET SPEECH";
                    Current.text = "SET MOOD";
                }
                if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("SpeachBML"))
                {
                    Previous.text = "SET SPEECH";
                    Current.text = "PLAY BML";
                }
            }
            else if (mAnimator.GetBool("BML"))
            {
                Current.text = "PLAY BML";
                if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("BMLMovement"))
                {
                    Next.text = "SET MOVE";
                }
            }
            else if (mAnimator.GetBool("Movement"))
            {
                Current.text = "SET MOVE";
                if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("MovementBML"))
                {
                    Next.text = "SET MOVE";
                    Current.text = "PLAY BML";
                };
                if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("MovementEmoji"))
                {
                    Previous.text = "SET MOVE";
                    Current.text = "SET MOOD";
                }
            }
        }
    }
}
