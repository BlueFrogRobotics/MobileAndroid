using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCurrentState : StateMachineBehaviour {

    [SerializeField]
    private Animator CurrentState;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetTrigger("ON_App"); // Recover the last state to trigger.
    }
}
