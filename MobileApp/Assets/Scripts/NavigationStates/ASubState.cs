using UnityEngine;
using System.Collections;

public abstract class ASubState : StateMachineBehaviour {

	[SerializeField]
	protected int State_Background;

	[SerializeField]
	protected int State_Top;

	[SerializeField]
	protected int OnOff_Bottom;

    [SerializeField]
    protected int State_Bottom;

    [SerializeField]
	protected bool EndScene;

	[SerializeField]
	protected int indexState;

	public GameObject ContentBottom { get; set; }
	public GameObject ContentTop { get; set; }
	public Animator BackgroundAnimator { get; set; }
	public Animator ContentTopAnimator { get; set; }
	public Animator ContentBottomAnimator { get; set; }

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Poney_Enter");
		if (indexState == 0) {
			if (ContentTop != null)
				ContentTop.SetActive (true);
			if (ContentBottom != null)
				ContentBottom.SetActive (true);
		}
		BackgroundAnimator.SetInteger ("Background_Blue_State", State_Background);
        ContentBottomAnimator.SetInteger("Content_Bottom_OnOff", OnOff_Bottom);
        ContentBottomAnimator.SetInteger ("Content_Bottom_State", State_Bottom);
		ContentTopAnimator.SetInteger ("Content_Top_State", State_Top);
		animator.SetBool ("EndScene", EndScene);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        Debug.Log("Poney_Exit");
        if (indexState == 3) {
			if (ContentTop != null)
				ContentTop.SetActive (false);
			if (ContentBottom != null)
				ContentBottom.SetActive (false);
		}
	}
}
