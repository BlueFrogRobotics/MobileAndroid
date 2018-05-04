using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WizardPopupManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] popups;

    [SerializeField]
    private InputField inputField;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CloseAllPopups()
    {
        foreach(GameObject popup in popups)
        {
            if(popup.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popup.GetComponent<Animator>().SetTrigger("close");
            }
        }
    }

    public void OpenPopupMovement()
    {
        OpenPopup(0);
        inputField.gameObject.SetActive(false);
    }

    public void OpenPopupExpression()
    {
        OpenPopup(1);
        inputField.gameObject.SetActive(false);
    }

    public void OpenPopupSound()
    {
        if (popups[2].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            inputField.gameObject.SetActive(false);
        else
            inputField.gameObject.SetActive(true);
        OpenPopup(2);
    }
    public void OpenPopupApplication()
    {
        OpenPopup(3);
        inputField.gameObject.SetActive(false);
    }

    private void OpenPopup(int iIndex)
    {
        for(int i=0; i<popups.Length; i++)
        {
            if (i!= iIndex && popups[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
            {
                popups[i].GetComponent<Animator>().SetTrigger("close");
            }
            else if(i==iIndex && popups[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Off"))
            {
                popups[i].GetComponent<Animator>().SetTrigger("open");
            }
            else if(i == iIndex && popups[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Popup_Idle"))
                popups[i].GetComponent<Animator>().SetTrigger("close");
        }
    }

}
