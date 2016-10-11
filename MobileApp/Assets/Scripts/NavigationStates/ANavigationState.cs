using UnityEngine;
using System.Collections;

public abstract class ANavigationState : StateMachineBehaviour {

	public Animator NavigationAnimator { get; set; }
	public Animator BackgroundAnimator { get; set; }
	public Animator ContentTopAnimator { get; set; }
	public Animator ContentBottomAnimator { get; set; }

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		//OnEnter (animator, stateInfo, layerIndex);
	}

	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//OnUpdate (animator, stateInfo, layerIndex);
	}

	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//OnExit (animator, stateInfo, layerIndex);
	}

//	public abstract void Init();
//	protected abstract void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
//	protected abstract void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
//	protected abstract void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
//	protected abstract void OnStartLeavingTransition(Animator iAnimator, AnimatorStateInfo stateInfo, int layerIndex);
//	protected abstract void OnEndLeavingTransition(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);


}
