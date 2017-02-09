using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HandleGeneric : MonoBehaviour {

    [SerializeField]
    private GameObject B_ScrollView;
    [SerializeField]
    private GameObject B_ControlUI;
    [SerializeField]
    private GameObject B_BottomUI;
    [SerializeField]
    private GameObject B_MessageUI;

    [SerializeField]
    private GameObject T_NavigationAccount;
    [SerializeField]
    private GameObject T_TopUI;
    [SerializeField]
    private GameObject T_RemoteUI;
    [SerializeField]
    private GameObject T_MessageTopUI;

    public void DesactivateGeneric(ArrayList iObjectsToKeep)
    {
        //SET ALL GENERIC ELEMENTS TO FALSE (LIKE SCROLLVIEW)
        B_ScrollView.SetActive(false);
        B_ControlUI.SetActive(false);
        B_BottomUI.SetActive(false);
        B_MessageUI.SetActive(false);
        T_NavigationAccount.SetActive(false);
        T_TopUI.SetActive(false);
        T_RemoteUI.SetActive(false);
        T_MessageTopUI.SetActive(false);

        if(iObjectsToKeep != null)
        {
            foreach (string lName in iObjectsToKeep)
            {
                switch (lName)
                {
                    case "ScrollView":
                        B_ScrollView.SetActive(true);
                        break;
                    case "ControlUI":
                        B_ControlUI.SetActive(true);
                        break;
                    case "BottomUI":
                        B_BottomUI.SetActive(true);
                        break;
                    case "MessageUI":
                        B_MessageUI.SetActive(true);
                        break;
                    case "NavigationAccount":
                        T_NavigationAccount.SetActive(true);
                        break;
                    case "TopUI":
                        T_TopUI.SetActive(true);
                        break;
                    case "RemoteUI":
                        T_RemoteUI.SetActive(true);
                        break;
                    case "MessageTopUI":
                        T_MessageTopUI.SetActive(true);
                        break;
                }
            }
        }
    }
}
