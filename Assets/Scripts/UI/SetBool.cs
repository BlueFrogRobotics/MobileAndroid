using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBool : StateMachineBehaviour {

    [SerializeField]
    private string[] NameOfBool;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(NameOfBool[0], true);
        animator.SetBool(NameOfBool[4], true);
        for (int i = 1; i <= 3; ++i)
        {
            //Debug.Log(NameOfBool[i]);
            animator.SetBool(NameOfBool[i], false);
        }
    }
}
