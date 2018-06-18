using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwipeControl : MonoBehaviour
{
    [SerializeField]
    private Button UpArrow;

    [SerializeField]
    private Button DownArrow;

    [SerializeField]
    private Button LeftArrow;

    [SerializeField]
    private Button RightArrow;



    //First/Last finger position
    private Vector3 mFirstPosition;
    private Vector3 mLastPosition;
    private Animator mAnimator;
    private int OffState;


    //Distance needed for a swipe to take some Action
    private float DragDistance;


    private void Start()
    {
        mAnimator = this.GetComponent<Animator>();
        DragDistance = 0.5F;
        OffState = Animator.StringToHash("Base.OFF");
    }

    public void Test()
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    void Update()
    {


        if (mAnimator.GetCurrentAnimatorStateInfo(0).nameHash != OffState)
        {
            //Check the touch inputs
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    mFirstPosition = touch.position;
                    mLastPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    mLastPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    //First check if it’s actually a drag
                    if (Mathf.Abs(mLastPosition.x - mFirstPosition.x) > DragDistance || Mathf.Abs(mLastPosition.y - mFirstPosition.y) > DragDistance)
                    {
                        //It’s a drag
                        //Now check what direction the drag was
                        //First check which axis
                        if (Mathf.Abs(mLastPosition.x - mFirstPosition.x) > Mathf.Abs(mLastPosition.y - mFirstPosition.y))
                        {
                            //If the horizontal movement is greater than the vertical movement…
                            if (mLastPosition.x > mFirstPosition.x)
                            {
                                if (LeftArrow.interactable)
                                    mAnimator.SetTrigger("Previous");
                            }
                            else
                            {
                                if (RightArrow.interactable)
                                    mAnimator.SetTrigger("Next");
                            }
                        }
                        else
                        {
                            //the vertical movement is greater than the horizontal movement
                            if (mLastPosition.y > mFirstPosition.y)
                            {
                                if (UpArrow.interactable)
                                    mAnimator.SetTrigger("NextPage");
                            }
                            else
                            {
                                if (DownArrow.interactable)
                                    mAnimator.SetTrigger("PreviousPage");
                            }
                        }
                    }

                }
            }
        }
    }
}