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

	[SerializeField]
	private GameObject loadingContentTop;

	[SerializeField]
	private GameObject loadingContentBottom;

	[SerializeField]
	private GameObject firstConnectionContentBottom;

	[SerializeField]
	private GameObject firstConnectionContentTop;

	[SerializeField]
	private GameObject connectAccountContentBottom;

	[SerializeField]
	private GameObject connectAccountContentTop;

	[SerializeField]
	private GameObject createAccountContentBottom;

	[SerializeField]
	private GameObject createAccountContentTop;

	[SerializeField]
	private GameObject termsOfUsesContentBottom;

	[SerializeField]
	private GameObject termsOfUsesContentTop;

	[SerializeField]
	private GameObject selectBuddyContentBottom;

	[SerializeField]
	private GameObject selectBuddyContentTop;

	[SerializeField]
	private GameObject addBuddyContentBottom;

	[SerializeField]
	private GameObject addBuddyContentTop;

	[SerializeField]
	private GameObject editAccountContentBottom;

	[SerializeField]
	private GameObject editAccountContentTop;

	[SerializeField]
	private GameObject editBuddyContentBottom;

	[SerializeField]
	private GameObject editBuddyContentTop;

	[SerializeField]
	private GameObject buddyConnectedContentBottom;

	[SerializeField]
	private GameObject buddyConnectedContentTop;

	[SerializeField]
	private GameObject loadingBuddyContentBottom;

	[SerializeField]
	private GameObject loadingBuddyContentTop;

	// Scene Apps

	[SerializeField]
	private GameObject distantControlContentBottom;

	[SerializeField]
	private GameObject distantControlContentTop;

	[SerializeField]
	private GameObject menuAdminContentBottom;

	[SerializeField]
	private GameObject menuAdminContentTop;

	[SerializeField]
	private GameObject messageContentBottom;

	[SerializeField]
	private GameObject messageContentTop;

	[SerializeField]
	private GameObject videoCallContentBottom;

	[SerializeField]
	private GameObject videoCallContentTop;

	// Use this for initialization

	void Awake() {
		ASubState[] lStates = navigationAnimator.GetBehaviours<ASubState>();

		foreach (ASubState lState in lStates) {
			lState.BackgroundAnimator = backgroundAnimator;
			lState.ContentTopAnimator = contentTopAnimator;
			lState.ContentBottomAnimator = contentBottomAnimator;
			//lState.Init ();
		}

		AssignGOToState<LoadingState>(loadingContentTop, loadingContentBottom);
		AssignGOToState<FirstConnectionState>(firstConnectionContentTop, firstConnectionContentBottom);
		AssignGOToState<CreateAccountState>(createAccountContentTop, createAccountContentBottom);
		AssignGOToState<ConnectAccountState>(connectAccountContentTop, connectAccountContentBottom);
		AssignGOToState<AddBuddyState>(addBuddyContentTop, addBuddyContentBottom);
		AssignGOToState<SelectBuddyState>(selectBuddyContentTop, selectBuddyContentBottom);
		AssignGOToState<EditAccountState>(editAccountContentTop, editAccountContentBottom);
		AssignGOToState<EditBuddyState>(editBuddyContentTop, editBuddyContentBottom);
		AssignGOToState<ConnectBuddyState>(buddyConnectedContentTop, buddyConnectedContentBottom);
		AssignGOToState<LoadingBuddyState>(loadingBuddyContentTop, loadingBuddyContentBottom);
		AssignGOToState<DistantControlState>(distantControlContentTop, distantControlContentBottom);
		AssignGOToState<MenuAdminState>(menuAdminContentTop, menuAdminContentBottom);
		AssignGOToState<MessageState>(messageContentTop, messageContentBottom);
		AssignGOToState<VideoCallState>(videoCallContentTop, videoCallContentBottom);
		AssignGOToState<TermsOfUsesState>(termsOfUsesContentTop, termsOfUsesContentBottom);
	}

	public void SetMenuBuddyValue(int iValue) {
		navigationAnimator.SetInteger ("MenuBuddy", iValue);		
	}

	private void AssignGOToState<T>(GameObject iTop, GameObject iBottom) where T : ASubState {
		T[] lStates = navigationAnimator.GetBehaviours<T>();
		foreach (T lState in lStates) {
			lState.ContentBottom = iBottom;
			lState.ContentTop = iTop;
		}
	}
}
