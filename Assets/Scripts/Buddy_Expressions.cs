using UnityEngine;
using System.Collections;

public class Buddy_Expressions : MonoBehaviour {
	
	[SerializeField]
	private Animator SuperAnimator;

	[SerializeField]
	private GameObject Face_V2;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	}
	public void SetInteractionBuddyValue(bool iValue) {
		SuperAnimator.SetBool ("Interactions", iValue);
	}
	public void SetExpressionBuddyValue(bool iValue) {
		SuperAnimator.SetBool ("Expressions", iValue);
	}
}
