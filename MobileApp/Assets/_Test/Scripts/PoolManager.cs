using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {

    [SerializeField]
    private GameObject BottomUI;
    [SerializeField]
    private GameObject Bubble_Blue;
    [SerializeField]
    private GameObject Bubble_White;
    [SerializeField]
    private GameObject Buddy_Contact;
    [SerializeField]
    private GameObject Button_Big;
    [SerializeField]
    private GameObject Button_L;
    [SerializeField]
    private GameObject Button_QrCode;
    [SerializeField]
    private GameObject Button_R;
    [SerializeField]
    private GameObject Button_Square;
    [SerializeField]
    private GameObject Button_User_Big;
    [SerializeField]
    private GameObject Button_User;
    [SerializeField]
    private GameObject Logo;
    [SerializeField]
    private GameObject Loading;
    [SerializeField]
    private GameObject Loading_Issues;
    [SerializeField]
    private GameObject Message_UI;
    [SerializeField]
    private GameObject Navigation_Account;
    [SerializeField]
    private GameObject Remote_UI;
    [SerializeField]
    private GameObject RemoteControl_UI;
    [SerializeField]
    private GameObject Searching_Contact;
    [SerializeField]
    private GameObject Simple_Text;
    [SerializeField]
    private GameObject ScrollView;
    [SerializeField]
    private GameObject TextField;
    [SerializeField]
    private GameObject TextField_Icon;
    [SerializeField]
    private GameObject TextField_Searching;
    [SerializeField]
    private GameObject Text_TermsOfUses;
    [SerializeField]
    private GameObject Toggle_Underline;
    [SerializeField]
    private GameObject Toggle;
    [SerializeField]
    private GameObject Top_UI;
    
    private Dictionary<string, Sprite> mAtlasMobile;
    private Dictionary<string, Sprite> mAtlasUI;

    // Retrive All used Atlas sprites
    void Start () {
        mAtlasUI = new Dictionary<string, Sprite>();
        mAtlasMobile = new Dictionary<string, Sprite>();

        Sprite[] lSpritesMobile = Resources.LoadAll<Sprite>("Sprites/AtlasMobile");
        Sprite[] lSpritesUI = Resources.LoadAll<Sprite>("Sprites/AtlasUI");

        foreach (Sprite lSprite in lSpritesMobile)
            mAtlasMobile.Add(lSprite.name, lSprite);

        foreach (Sprite lSprite in lSpritesUI)
            mAtlasUI.Add(lSprite.name, lSprite);
    }

    // Buble Function
    public GameObject fBubble(string iType, string iText, string iDate )
    {
        
        GameObject lBubble_Blue = Bubble_Blue;
        GameObject lBubble_White = Bubble_White;
        GameObject lMessage_UI = Message_UI;
        lMessage_UI.SetActive(true);

        if (iType == "Blue") {
            lBubble_Blue.gameObject.SetActive(true);
            GameObject lBubble_Blue_Text = GameObject.Find("Bubble_Blue/Bubble_Message");
            GameObject lBubble_Blue_Date = GameObject.Find("Bubble_Blue/Bubble_Date");
            lBubble_Blue_Text.GetComponent<Text>().text = iText;
            lBubble_Blue_Date.GetComponent<Text>().text = iDate;
            GameObject lFinalObject = Instantiate(Bubble_Blue);
            lFinalObject.transform.SetParent(GameObject.Find("Content_Bottom/Message_UI/ScrollView/Viewport/Content").transform, false);
            return lFinalObject;
        }
        else
        {
            lBubble_Blue.gameObject.SetActive(true);
            GameObject lBubble_White_Text = GameObject.Find("Bubble_White/Bubble_Message");
            GameObject lBubble_White_Date = GameObject.Find("Bubble_White/Bubble_Date");
            lBubble_White_Text.GetComponent<Text>().text = iText;
            lBubble_White_Date.GetComponent<Text>().text = iDate;
            GameObject lFinalObject = Instantiate(Bubble_White);
            lFinalObject.transform.SetParent(GameObject.Find("Content_Bottom/Message_UI/ScrollView/Viewport/Content").transform, false);
            return lFinalObject;
        }
    }

    // Button Square Function
    public GameObject fButton_Square(string iParent, string iText, string iIconName, List<UnityAction> iCallbacks)
    {
        Button lButton = Button_Square.GetComponentInChildren<Button>();
        lButton.gameObject.SetActive(true);
        lButton.GetComponentInChildren<Text>().text = iText;

        if (!string.IsNullOrEmpty(iIconName))
        {
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iIconName))
                lSprite = mAtlasMobile[iIconName];
            else if (mAtlasUI.ContainsKey(iIconName))
                lSprite = mAtlasUI[iIconName];
            else
                lSprite = new Sprite();
            lButton.GetComponentsInChildren<Image>()[1].sprite = lSprite;
        }
        else
            lButton.GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);

        lButton.onClick.RemoveAllListeners();
        if(iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        GameObject lFinalObject = Instantiate(Button_Square);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Button Big Function
    public GameObject fButton_Big(string iParent, string iText, string iIconName, List<UnityAction> iCallbacks)
    {
        Button lButton = Button_Big.GetComponent<Button>();
        lButton.gameObject.SetActive(true);
        lButton.GetComponentInChildren<Text>().text = iText;

        if (!string.IsNullOrEmpty(iIconName))
        {
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iIconName))
                lSprite = mAtlasMobile[iIconName];
            else if (mAtlasUI.ContainsKey(iIconName))
                lSprite = mAtlasUI[iIconName];
            else
                lSprite = new Sprite();
            lButton.GetComponentsInChildren<Image>()[2].sprite = lSprite;
        }
        else
            lButton.GetComponentsInChildren<Image>()[2].gameObject.SetActive(false);

        lButton.onClick.RemoveAllListeners();
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        GameObject lFinalObject = Instantiate(Button_Big);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Button_L Function
    public GameObject fButton_L(string iParent, string iIconName, List<UnityAction> iCallbacks)
    {
        Button lButton = Button_L.GetComponent<Button>();
        lButton.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(iIconName))
        {
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iIconName))
                lSprite = mAtlasMobile[iIconName];
            else if (mAtlasUI.ContainsKey(iIconName))
                lSprite = mAtlasUI[iIconName];
            else
                lSprite = new Sprite();
            lButton.GetComponentsInChildren<Image>()[1].sprite = lSprite;
        }
        else
            lButton.GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);

        lButton.onClick.RemoveAllListeners();
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        GameObject lFinalObject = Instantiate(Button_L);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Button_QrCode Function
    public GameObject fButton_QrCode(string iParent, List<UnityAction> iCallbacks)
    {
        Button lButton = Button_QrCode.GetComponent<Button>();
        lButton.gameObject.SetActive(true);

        lButton.onClick.RemoveAllListeners();
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        GameObject lFinalObject = Instantiate(Button_QrCode);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Button_R Function
    public GameObject fButton_R(string iParent, string iIconName, List<UnityAction> iCallbacks)
    {
        Button lButton = Button_R.GetComponent<Button>();
        lButton.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(iIconName))
        {
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iIconName))
                lSprite = mAtlasMobile[iIconName];
            else if (mAtlasUI.ContainsKey(iIconName))
                lSprite = mAtlasUI[iIconName];
            else
                lSprite = new Sprite();
            lButton.GetComponentsInChildren<Image>()[1].sprite = lSprite;
        }
        else
            lButton.GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);

        lButton.onClick.RemoveAllListeners();
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        GameObject lFinalObject = Instantiate(Button_R);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Simple_Text Function
    public GameObject fSimple_Text(string iParent, string iText)
    {
        GameObject lSimple_Text = Simple_Text;
        lSimple_Text.GetComponentInChildren<Text>().text = iText;

        lSimple_Text.gameObject.SetActive(true);

        GameObject lFinalObject = Instantiate(Simple_Text);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Button_User_Big
    public GameObject fButton_User_Big(string iParent, string iPicture, List<UnityAction> iCallbacks)
    {
        Button lButton = Button_User_Big.GetComponent<Button>();
        lButton.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(iPicture))
        {
            // Checker la photo utilisateur 
        }

        else { }

        lButton.onClick.RemoveAllListeners();
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        GameObject lFinalObject = Instantiate(Button_User_Big);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Button_User
    public GameObject fButton_User(string iParent, string iPicture, List<UnityAction> iCallbacks)
    {
        Button lButton = Button_User.GetComponent<Button>();
        lButton.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(iPicture))
        {
            // Checker la photo utilisateur 
        }

        else { }

        lButton.onClick.RemoveAllListeners();
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        GameObject lFinalObject = Instantiate(Button_User);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Loading
    public GameObject fLoading(string iParent, string iText)
    {
        GameObject lLoading = Loading;
        lLoading.gameObject.SetActive(true);
        lLoading.GetComponentInChildren<Text>().text = iText;

        GameObject lFinalObject = Instantiate(Loading);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Logo
    public GameObject fLogo(string iParent)
    {
        GameObject lLogo = Logo;
        lLogo.gameObject.SetActive(true);

        GameObject lFinalObject = Instantiate(Logo);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
