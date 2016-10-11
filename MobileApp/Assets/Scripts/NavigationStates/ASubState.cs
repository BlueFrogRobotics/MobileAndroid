using UnityEngine;
using System.Collections;

public abstract class ASubState : StateMachineBehaviour {

	[SerializeField]
	protected int triggerBackground;

	[SerializeField]
	protected int triggerTop;

	[SerializeField]
	protected int triggerBottom;

	[SerializeField]
	protected bool triggerEndScene;

	[SerializeField]
	protected int indexState;

	public GameObject ContentBottom { get; set; }
	public GameObject ContentTop { get; set; }

	public Animator BackgroundAnimator { get; set; }
	public Animator ContentTopAnimator { get; set; }
	public Animator ContentBottomAnimator { get; set; }

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (indexState == 0) {
			if (ContentTop != null)
				ContentTop.SetActive (true);
			if (ContentBottom != null)
				ContentBottom.SetActive (true);
		}
		BackgroundAnimator.SetInteger ("Background_Blue_State", triggerBackground);
		ContentBottomAnimator.SetInteger ("Content_Bottom_State", triggerBottom);
		ContentTopAnimator.SetInteger ("Content_Top_State", triggerTop);
		animator.SetBool ("EndScene", triggerEndScene);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (indexState == 3) {
			if (ContentTop != null)
				ContentTop.SetActive (false);
			if (ContentBottom != null)
				ContentBottom.SetActive (false);
		}
	}
}
