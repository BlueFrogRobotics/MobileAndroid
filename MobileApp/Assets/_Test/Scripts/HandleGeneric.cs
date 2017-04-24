﻿using UnityEngine;
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
    private GameObject T_TopUI;
    [SerializeField]
    private GameObject T_RemoteUI;
    [SerializeField]
    private GameObject T_MessageTopUI;
    [SerializeField]
    private GameObject T_NavigationAccount;
    [SerializeField]
    private GameObject T_FieldLastName;
    [SerializeField]
    private GameObject T_FieldFirstName;
    [SerializeField]
    private GameObject T_TextLastName;
    [SerializeField]
    private GameObject T_TextFirstName;
    [SerializeField]
    private GameObject T_ArrowLeft;
    [SerializeField]
    private GameObject T_ArrowRight;
    [SerializeField]
    private GameObject T_Dots;

    public void DisableGeneric(ArrayList iObjectsToKeep)
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
                        EnableNavigationAccount();
                        break;
                    case "NavigationEdit":
                        EnableNavigationEdit();
                        break;
                    case "NavigationDisplay":
                        EnableNavigationDisplay();
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

    public void SetEditInfos(string iName, string iLastName)
    {
		T_FieldFirstName.GetComponentsInChildren<Text>()[1].text = iName;
		T_FieldLastName.GetComponentsInChildren<Text>()[1].text = iLastName;
    }

    public void SetDisplayInfos(string iName, string iLastName)
    {
        T_TextFirstName.GetComponent<Text>().text = iName;
        T_TextLastName.GetComponent<Text>().text = iLastName;
    }

    private void EnableNavigationAccount()
    {
        T_NavigationAccount.SetActive(true);
        T_TextFirstName.SetActive(true);
        T_TextLastName.SetActive(true);
        T_ArrowLeft.SetActive(true);
        T_ArrowRight.SetActive(true);
        T_Dots.SetActive(true);
        T_FieldFirstName.SetActive(false);
        T_FieldLastName.SetActive(false);
    }

    private void EnableNavigationEdit()
    {
        T_NavigationAccount.SetActive(true);
        T_FieldFirstName.SetActive(true);
        T_FieldLastName.SetActive(true);
        T_TextFirstName.SetActive(false);
        T_TextLastName.SetActive(false);
        T_ArrowLeft.SetActive(false);
        T_ArrowRight.SetActive(false);
        T_Dots.SetActive(false);

        DBManager lDB = GameObject.Find("DBManager").GetComponent<DBManager>();
        T_FieldFirstName.GetComponent<InputField>().text = lDB.CurrentUser.FirstName;
        T_FieldLastName.GetComponent<InputField>().text = lDB.CurrentUser.LastName;
    }

    private void EnableNavigationDisplay()
    {
        T_NavigationAccount.SetActive(true);
        T_TextFirstName.SetActive(true);
        T_TextLastName.SetActive(true);
        T_ArrowLeft.SetActive(false);
        T_ArrowRight.SetActive(false);
        T_Dots.SetActive(false);
        T_FieldFirstName.SetActive(false);
        T_FieldLastName.SetActive(false);
    }
}
