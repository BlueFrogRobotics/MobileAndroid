using UnityEngine;
using System.Collections;

public class LinkManager : MonoBehaviour {

	[SerializeField]
	private Animator backgroundAnimator;

	[SerializeField]
	private Animator contentBottomAnimator;

	[SerializeField]
	private Animator contentTopAnimator;

	[SerializeField]
	private Animator navigationAnimator;
  
	void Awake() {
        // Assign the animators to each state.
		ASubState[] lStates = navigationAnimator.GetBehaviours<ASubState>();

		foreach (ASubState lState in lStates) {
			lState.BackgroundAnimator = backgroundAnimator;
			lState.ContentTopAnimator = contentTopAnimator;
			lState.ContentBottomAnimator = contentBottomAnimator;
		}
	}

    // Don't see the point of this method as it's never used anywhere ...
	private void AssignGOToState<T>(GameObject iTop, GameObject iBottom) where T : ASubState {
		T[] lStates = navigationAnimator.GetBehaviours<T>();
		foreach (T lState in lStates) {
			lState.ContentBottom = iBottom;
			lState.ContentTop = iTop;
		}
	}
}
