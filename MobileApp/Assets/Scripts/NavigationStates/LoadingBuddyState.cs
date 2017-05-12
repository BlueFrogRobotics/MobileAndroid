using UnityEngine;
using System.Collections;

public class LoadingBuddyState : ASubState {

    private Animator mAnimator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(null);
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fLoading("Content_Top", GoBack.LoadingBuddyMessage));
            LoadingUI.AddObject(lPoolManager.fLogo("Content_Top"));
        }

        else if(indexState == 2)
        {
            mAnimator = animator;
            SelectBuddy lSelect = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();
            if (lSelect.Remote == SelectBuddy.RemoteType.LOCAL)
            {
                
            }
            else if (lSelect.Remote == SelectBuddy.RemoteType.WEBRTC)
            {

            }
        }
    }

    private IEnumerator WaitForLocalConfirmation()
    {
        CallAcceptOTOReceiver lConfirmation = GameObject.Find("CallAcceptReceiver").GetComponent<CallAcceptOTOReceiver>();

        while(lConfirmation.Status == CallAcceptOTOReceiver.CallStatus.WAITING) {
            yield return new WaitForSeconds(0.5F);
        }

        if(lConfirmation.Status == CallAcceptOTOReceiver.CallStatus.ACCEPTED) {
            mAnimator.SetTrigger("EndScene");
        } else if(lConfirmation.Status == CallAcceptOTOReceiver.CallStatus.REJECTED) {
            mAnimator.SetTrigger("GoConnectBuddy");
            mAnimator.SetTrigger("EndScene");
        }
    }
}
