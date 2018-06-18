using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTitle : MonoBehaviour
{
    [Header("Modifications of titles")]
    [SerializeField]
    private Text Current;

    [Header("Move in the Cube")]
    [SerializeField]
    private Button UpArrow;

    [SerializeField]
    private Button UpArrowbg;

    [SerializeField]
    private Button DownArrow;

    [SerializeField]
    private Button DownArrowbg;

    [SerializeField]
    private Button LeftArrow;

    [SerializeField]
    private Button RightArrow;

    [SerializeField]
    private Button LeftArrowbg;

    [SerializeField]
    private Button RightArrowbg;

    [SerializeField]
    private Text Pages;

    private Animator mAnimatorPopups;



    void Start()
    {
        mAnimatorPopups = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("OFF"))
        {
            SetTitles();
            SetArrows();
        }
    }

    private void SetTitles()
    {
        if (mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("ON_Speech"))
            Current.text = "SET SPEECH";
        else if (mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("ON_BML"))
            Current.text = "PLAY BML";
        else if (mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("ON_Emoji"))
            Current.text = "SET EXPRESSIONS";
        else if (mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("ON_App"))
            Current.text = "LAUNCH APP";
        else if (mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("ON_Movement"))
            Current.text = "SET MOVEMENT";
    }

    private void SetArrows()
    {
        if (mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("ON_App") ||
                    mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("Previous"))
        {
            mAnimatorPopups.SetBool("HavePages", true);
            UpArrow.interactable = true;
            UpArrowbg.interactable = true;
            DownArrow.interactable = false;
            DownArrowbg.interactable = false;
            LeftArrow.interactable = true;
            RightArrow.interactable = true;
            LeftArrowbg.interactable = true;
            RightArrowbg.interactable = true;
            Pages.text = "Pages 1/2";
        }
        else if (mAnimatorPopups.GetCurrentAnimatorStateInfo(0).IsName("Next"))
        {
            mAnimatorPopups.SetBool("HavePages", true);
            UpArrow.interactable = false;
            UpArrowbg.interactable = false;
            DownArrow.interactable = true;
            DownArrowbg.interactable = true;
            LeftArrow.interactable = false;
            RightArrow.interactable = false;
            LeftArrowbg.interactable = false;
            RightArrowbg.interactable = false;
            Pages.text = "Pages 2/2";
        }
        else
        {
            mAnimatorPopups.SetBool("HavePages", false);
            UpArrow.interactable = false;
            UpArrowbg.interactable = false;
            DownArrow.interactable = false;
            DownArrowbg.interactable = false;
            LeftArrow.interactable = true;
            RightArrow.interactable = true;
            LeftArrowbg.interactable = true;
            RightArrowbg.interactable = true;
            Pages.text = "Pages 1/1";
        }
    }
}
