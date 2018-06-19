using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {

    [SerializeField]
    private GameObject Bubble_Blue;
    [SerializeField]
    private GameObject Bubble_White;
    [SerializeField]
    private GameObject Buddy_Contact;
    [SerializeField]
    private GameObject Buddy_Separator;
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
    private GameObject Button_Text_Underline;
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
    private GameObject Searching_Contact;
    [SerializeField]
    private GameObject Simple_Text;
    [SerializeField]
    private GameObject Simple_Title;
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
    private GameObject PopUps;
    [SerializeField]
	private GameObject MessageContent;
	[SerializeField]
	private GameObject Buddy_Status;

    private Dictionary<string, Sprite> mAtlasMobile;
    private Dictionary<string, Sprite> mAtlasUI;
    private Dictionary<string, Sprite> mAtlasIcon;

    public Dictionary Dictionary { private set; get; }

    // Retrieve All used Atlas sprites
    void Start () {
        //Create the dictionary, default being in French.
        Dictionary = new Dictionary(Language.FR, this);
        mAtlasUI = new Dictionary<string, Sprite>();
        mAtlasMobile = new Dictionary<string, Sprite>();
        mAtlasIcon = new Dictionary<string, Sprite>();
        
        //Load sprite and store them in Dictionaries for further easy access.
        Sprite[] lSpritesMobile = Resources.LoadAll<Sprite>("Sprites/AtlasMobile");
        Sprite[] lSpritesUI = Resources.LoadAll<Sprite>("Sprites/AtlasUI");
        Sprite[] lSpritesIcon = Resources.LoadAll<Sprite>("Sprites/Atlas_Icons");

        foreach (Sprite lSprite in lSpritesMobile)
            mAtlasMobile.Add(lSprite.name, lSprite);

        foreach (Sprite lSprite in lSpritesUI)
            mAtlasUI.Add(lSprite.name, lSprite);

        foreach (Sprite lSprite in lSpritesIcon)
            mAtlasIcon.Add(lSprite.name, lSprite);
    }

    /// <summary>
    /// Create a Bubble (designed for the chat).
    /// </summary>
    /// <param name="iType">"Blue" or "White". Blue is for someone else's message, white being the user's message.</param>
    /// <param name="iText">The text to display in the message.</param>
    /// <param name="iDate">The timestamp of the message.</param>
    /// <returns>The "Bubble" game object with the proper configuration.</returns>
    public GameObject fBubble(string iType, string iText, string iDate)
    {        
        GameObject lBubble_Blue = Bubble_Blue;
        GameObject lBubble_White = Bubble_White;
        GameObject lMessage_UI = GameObject.Find("Content_Bottom/Message_UI");

        if (iType == "Blue") {
            GameObject lFinalObject = Instantiate(Bubble_Blue);
            Text lBubble_Blue_Text = lFinalObject.GetComponentsInChildren<Text>()[0];
            Text lBubble_Blue_Date = lFinalObject.GetComponentsInChildren<Text>()[1];
            lBubble_Blue_Text.text = iText;
            lBubble_Blue_Date.text = iDate;  
            lFinalObject.transform.SetParent(MessageContent.transform, false);
            return lFinalObject;
        }
        else
        { 
            GameObject lFinalObject = Instantiate(Bubble_White);
            Text lBubble_White_Text = lFinalObject.GetComponentsInChildren<Text>()[0];
            Text lBubble_White_Date = lFinalObject.GetComponentsInChildren<Text>()[1];
            lBubble_White_Text.text = iText;
            lBubble_White_Date.text = iDate;
            lFinalObject.transform.SetParent(MessageContent.transform, false);
            return lFinalObject;
        }
    }

    /// <summary>
    /// Creates an indicator to display Buddy's status.
    /// </summary>
    /// <param name="iParent">The parent game object in which to display the indicator.</param>
    /// <param name="iId">Buddy's ID.</param>
    /// <param name="displayText">Some special text to display next to the status icon.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
	public GameObject fBuddy_Status(string iParent, string iId, bool displayText)
	{
		GameObject lFinalObject = Instantiate(Buddy_Status);
		lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);

		BuddyStatus statusScript = lFinalObject.GetComponent<BuddyStatus>();
		statusScript.SetData(iId, true);

		return lFinalObject;
	}

    /// <summary>
    /// Create a game object to display Buddies in the selection menu.
    /// </summary>
    /// <param name="iParent">The Scrollview to display the buddy in.</param>
    /// <param name="iName">Buddy's name to be displayed.</param>
    /// <param name="iID">Buddy's ID to be displayed.</param>
    /// <param name="iUserPic">A special icon to display.</param>
    /// <param name="iLocal">Is the Buddy on a local network or not ?</param>
    /// <param name="iListed">Is the detected Buddy already linked to the active account ?</param>
    /// <param name="iStatus">Current status of Buddy: online, busy, offline.</param>
    /// <param name="iCallbacks">Actions to be launched if the Buddy is to be edited or added if not listed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
	public GameObject fBuddy_Contact(string iParent, string iName, string iID, string iUserPic, bool iLocal, bool iListed, string iStatus, List<UnityAction> iCallbacks)
    {
		
        GameObject lFinalObject = Instantiate(Buddy_Contact);
        Button lButton_Edit = lFinalObject.GetComponentsInChildren<Button>()[1];
        Button lButton_Add = lFinalObject.GetComponentsInChildren<Button>()[2];
        Image lUser_Pic = lFinalObject.GetComponentsInChildren<Image>()[7];
        Text lName = lFinalObject.GetComponentsInChildren<Text>()[0];
		Text lID = lFinalObject.GetComponentsInChildren<Text>()[1];
        Image lLocal = lFinalObject.GetComponentsInChildren<Image>()[11];
		BuddyStatus statusScript = lFinalObject.GetComponentsInChildren<BuddyStatus>()[0];

		statusScript.SetData(iID, false);

        // ADD TEXT NAME AND ID OF THIS CONTACT
        lName.text = iName;
		lID.text = (iLocal ? "IP " : "ID ") + iID;


        // GET USER PICS OR SET IT TO DEFAULT
        if (!string.IsNullOrEmpty(iUserPic))
        {
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iUserPic))
                lSprite = mAtlasMobile[iUserPic];
            else if (mAtlasUI.ContainsKey(iUserPic))
                lSprite = mAtlasUI[iUserPic];
            else
                lSprite = Resources.Load<Sprite>(iUserPic) as Sprite;
            lUser_Pic.sprite = lSprite;
        }

        // HANDLE LOCAL OR INTERNET SPRITE.
        if (iLocal == true)
            lLocal.GetComponent<Image>().sprite = mAtlasMobile["Icon_Local"];
        else
            lLocal.GetComponent<Image>().sprite = mAtlasMobile["Icon_Net"];

		// Desactivate all buttons for now (not used)
		lButton_Edit.gameObject.SetActive(false);
		lButton_Add.gameObject.SetActive(false);

        // CREATE CALL BACK ON BUTTON ADD IF iListed = False OR ON EDIT IF iListed = True
        if (iCallbacks != null)
        {
			// ACTIVATE OR DESACTIVATE BUTTON ADD IF iListed = False OR ON EDIT IF iListed = True
			if (iListed)
			{
				lButton_Edit.gameObject.SetActive(true);
				lButton_Add.gameObject.SetActive(false);
				foreach (UnityAction lCallback in iCallbacks)
					lButton_Edit.onClick.AddListener(lCallback);
			}
			else
			{
				lButton_Edit.gameObject.SetActive(false);
				lButton_Add.gameObject.SetActive(true);
				foreach (UnityAction lCallback in iCallbacks)
					lButton_Add.onClick.AddListener(lCallback);
			}
		}

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    // Buddy_Separator
    public GameObject fBuddy_Separator(string iParent, string iKey)
    {
        GameObject lFinalObject = Instantiate(Buddy_Separator);
        GameObject lBuddy_Separator = lFinalObject;
        lBuddy_Separator.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// A square button to press.
    /// </summary>
    /// <param name="iParent">The game object's parent.</param>
    /// <param name="iKey">The dictionary key to display text inside the button.</param>
    /// <param name="iIconName">Icon name for UI display.</param>
    /// <param name="iCallbacks">Actions to be launched when button is pressed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_Square(string iParent, string iKey, string iIconName, List<UnityAction> iCallbacks)
    {
        GameObject lFinalObject = Instantiate(Button_Square);
        Button lButton = lFinalObject.GetComponentInChildren<Button>();
        lButton.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);

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

        if(iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// A big square button to press.
    /// </summary>
    /// <param name="iParent">The game object's parent.</param>
    /// <param name="iKey">The dictionary key to display text inside the button.</param>
    /// <param name="iIconName">Icon name for UI display.</param>
    /// <param name="iCallbacks">Actions to be launched when button is pressed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_Big(string iParent, string iKey, string iIconName, List<UnityAction> iCallbacks)
    {
        GameObject lFinalObject = Instantiate(Button_Big);
        Button lButton = lFinalObject.GetComponent<Button>();
        lButton.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);

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

        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// A button with a left anchor.
    /// </summary>
    /// <param name="iParent">The parent in which to display the button.</param>
    /// <param name="iIconName">Icon name for UI display.</param>
    /// <param name="iCallbacks">Actions to be launched when button is pressed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_L(string iParent, string iIconName, List<UnityAction> iCallbacks)
    {
        GameObject lFinalObject = Instantiate(Button_L);
        Button lButton = lFinalObject.GetComponent<Button>();

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

        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Button with QRCode icon
    /// </summary>
    /// <param name="iParent">The parent in which to display the button.</param>
    /// <param name="iCallbacks">Actions to be launched when button is pressed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_QrCode(string iParent, List<UnityAction> iCallbacks)
    {
        GameObject lFinalObject = Instantiate(Button_QrCode);
        Button lButton = lFinalObject.GetComponent<Button>();

        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// A button with a left anchor.
    /// </summary>
    /// <param name="iParent">The parent in which to display the button.</param>
    /// <param name="iIconName">Icon name for UI display.</param>
    /// <param name="iCallbacks">Actions to be launched when button is pressed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_R(string iParent, string iIconName, List<UnityAction> iCallbacks)
    {
        GameObject lFinalObject = Instantiate(Button_R);
        Button lButton = lFinalObject.GetComponent<Button>();

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

        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create an underlined text.
    /// </summary>
    /// <param name="iParent">The parent to display the text in.</param>
    /// <param name="iKey">The dictionary key for the text to display.</param>
    /// <param name="iCallbacks">The callbacks to be called when text is clicked.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_Text_Underline(string iParent, string iKey, List<UnityAction> iCallbacks)
    {
        GameObject lFinalObject = Instantiate(Button_Text_Underline);
        lFinalObject.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
            {
                Button lButton_Text_Underline = lFinalObject.GetComponentInChildren<Button>();
                lButton_Text_Underline.onClick.AddListener(lCallback);
            }
        }
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a big clickable user icon button.
    /// </summary>
    /// <param name="iParent">The parent to display the user icon in.</param>
    /// <param name="iPicture">The name of the picture to load. Will either be loaded in the sprites atlas, load from the resources or display the currently active user's profile picture.</param>
    /// <param name="iCallbacks">Actions to be called when clicking on the picture.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_User_Big(string iParent, string iPicture, List<UnityAction> iCallbacks)
    {
        DBManager lDBManager = GameObject.Find("DBManager").GetComponent<DBManager>();
        GameObject lFinalObject = Instantiate(Button_User_Big);
        Button lButton = lFinalObject.GetComponent<Button>();
        Image lUser_Pic = lFinalObject.GetComponentsInChildren<Image>()[2];

		if (lDBManager.CurrentUser.LastName.Contains ("DEMO")) {
			iPicture = "DefaultUser";
		}

        if (!string.IsNullOrEmpty(iPicture))
        {
            // Checker la photo utilisateur 
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iPicture))
                lSprite = mAtlasMobile[iPicture];
            else if (mAtlasUI.ContainsKey(iPicture))
                lSprite = mAtlasUI[iPicture];
            else
                lSprite = Resources.Load<Sprite>(iPicture) as Sprite;
            lUser_Pic.sprite = lSprite;
        } else {
            lButton.GetComponentInChildren<AspectRatioFitter>().aspectRatio = 1;
            Sprite lSprite = lDBManager.GetCurrentUserImage();
            if (lSprite != null)
                lUser_Pic.sprite = lSprite;
        }

        if (iCallbacks != null)
        {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a clickable user icon button.
    /// </summary>
    /// <param name="iParent">The parent to display the user icon in.</param>
    /// <param name="iPicture">The name of the picture to load. Will either be loaded in the sprites atlas, load from the resources or display the currently active user's profile picture.</param>
    /// <param name="iCallbacks">Actions to be called when clicking on the picture.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fButton_User(string iParent, string iPicture, bool iActive, List<UnityAction> iCallbacks)
    {
        DBManager lDBManager = GameObject.Find("DBManager").GetComponent<DBManager>();
        GameObject lFinalObject = Instantiate(Button_User);
        Button lButton = lFinalObject.GetComponent<Button>();
        Image lUser_Pic = lFinalObject.GetComponentsInChildren<Image>()[2];

		if (lDBManager.CurrentUser.LastName.Contains ("DEMO")) {
			iPicture = "DefaultUser";
		}
        
		if (!string.IsNullOrEmpty(iPicture)) {
            // Look for the user profile picture.
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iPicture))
                lSprite = mAtlasMobile[iPicture];
            else if (mAtlasUI.ContainsKey(iPicture))
                lSprite = mAtlasUI[iPicture];
            else
                lSprite = Resources.Load<Sprite>(iPicture) as Sprite;
            lUser_Pic.sprite = lSprite;
        } else {
            lButton.GetComponentInChildren<AspectRatioFitter>().aspectRatio = 1;
            Sprite lSprite = lDBManager.GetCurrentUserImage();
            if (lSprite != null)
                lUser_Pic.sprite = lSprite;
        }

        if (iCallbacks != null) {
            foreach (UnityAction lCallback in iCallbacks)
                lButton.onClick.AddListener(lCallback);
        }

        if (iActive == true) {
            lButton.interactable = true;
        } else {
            lButton.interactable = false;
            lButton.GetComponentsInChildren<Image>()[2].sprite = null;
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a "loading" icon with text under.
    /// </summary>
    /// <param name="iParent">The parent to display the loading icon in.</param>
    /// <param name="iKey">The dictionary key for the text to be displayed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fLoading(string iParent, string iKey)
    {
        GameObject lFinalObject = Instantiate(Loading);
        GameObject lLoading = lFinalObject;
        lLoading.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create an icon to display loading issues with an explanatory text.
    /// </summary>
    /// <param name="iParent">The parent to display the icon in.</param>
    /// <param name="iText">The dictionary key for the text to be displayed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fLoading_Issues(string iParent, string iText)
    {
        GameObject lFinalObject = Instantiate(Loading_Issues);
        GameObject lLoading_Issues = lFinalObject;
        lLoading_Issues.GetComponentInChildren<Text>().text = iText;

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Display Blue Frog's logo.
    /// </summary>
    /// <param name="iParent">The parent to display the logo in.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fLogo(string iParent)
    {
        GameObject lFinalObject = Instantiate(Logo);
        GameObject lLogo = lFinalObject;

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a "searching" icon.
    /// </summary>
    /// <param name="iParent">The parent to display the icon in.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fSearching(string iParent)
    {
        GameObject lFinalObject = Instantiate(Searching_Contact);
        GameObject lSearching_Contact = lFinalObject;
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a simple text.
    /// </summary>
    /// <param name="iParent">The parent to display the text in.</param>
    /// <param name="iKey">The dictionary key for the text to display.</param>
    /// <param name="iPopup">Is the text in a popup ? If true, the color is changed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
	public GameObject fSimple_Text(string iParent, string iKey, bool iPopup)
    {
        GameObject lFinalObject = Instantiate(Simple_Text);
        GameObject lSimple_Text = lFinalObject;
        lSimple_Text.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);

        if (iPopup == true)
        {
			lSimple_Text.GetComponentInChildren<Text>().color = new Color32(85, 85, 85, 255);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a "title" text.
    /// </summary>
    /// <param name="iParent">The parent to display the text in.</param>
    /// <param name="iKey">The dictionary key for the text to display.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fSimple_Title(string iParent, string iKey)
    {
        GameObject lFinalObject = Instantiate(Simple_Title);
        GameObject lSimple_Title = lFinalObject;
        lSimple_Title.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a text field with an icon.
    /// </summary>
    /// <param name="iParent">The parent to display the textfield in.</param>
    /// <param name="iPlaceHolderKey">The dictionary key for the placeholder.</param>
    /// <param name="iText">The dictionary key for the text to display.</param>
    /// <param name="iIconName">The name of the icon to be displayed next to the text field.</param>
    /// <param name="iCallbacksChange">Callbacks when the text inside changes.</param>
    /// <param name="iCallbacksEnd">Callbacks when text edition is finished.</param>
    /// <param name="iCallbacksInfos">Callbacks when the text field is being clicked on.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fTextField_Icon(string iParent, string iPlaceHolderKey, string iText, string iIconName, List<UnityAction<string>> iCallbacksChange, List<UnityAction<string>> iCallbacksEnd, List<UnityAction> iCallbacksInfos)
    {
        GameObject lFinalObject = Instantiate(TextField_Icon);
        InputField lTextField_Icon = lFinalObject.GetComponent<InputField>();

        lTextField_Icon.GetComponentInChildren<Text>().text = Dictionary.GetString(iPlaceHolderKey);
        lTextField_Icon.GetComponent<InputField>().text = iText;

        if (!string.IsNullOrEmpty(iIconName))
        {
            Sprite lSprite;
            if (mAtlasMobile.ContainsKey(iIconName))
                lSprite = mAtlasMobile[iIconName];
            else if (mAtlasUI.ContainsKey(iIconName))
                lSprite = mAtlasUI[iIconName];
            else
                lSprite = new Sprite();
            lTextField_Icon.GetComponentsInChildren<Image>()[3].sprite = lSprite;
        } else {
            lTextField_Icon.GetComponentsInChildren<Image>()[3].sprite = null;
        }

        // CREATE CALL BACK FOR EVERY CHANGE IN THIS TEXTFIELD
        if (iCallbacksChange != null)
        {
            foreach (UnityAction<string> lCallback in iCallbacksChange)
                lTextField_Icon.onValueChanged.AddListener(lCallback);
        }

        // CREATE CALL BACK FOR THE END OF EDITION OF IN THIS TEXTFIELD
        if (iCallbacksEnd != null)
        {
            foreach (UnityAction<string> lCallback in iCallbacksEnd)
                lTextField_Icon.onEndEdit.AddListener(lCallback);
        }

        // CREATE A BUTTON BUBBLE TO DISPLAY HELP IF ANY "CallBacks_Infos"
        Button lButton_Infos = lFinalObject.GetComponentInChildren<Button>();
        if (iCallbacksInfos != null)
        {
            foreach (UnityAction lCallbackInfos in iCallbacksInfos)
                lButton_Infos.onClick.AddListener(lCallbackInfos);
        }
        else
            lButton_Infos.gameObject.SetActive(false);

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a text field with a "search" icon.
    /// </summary>
    /// <param name="iParent">The parent to display the textfield in.</param>
    /// <param name="iPlaceHolderKey">The dictionary key for the placeholder.</param>
    /// <param name="iText">The text to be displayed inside the text field.</param>
    /// <param name="iCallbacksChange">Callbacks when the text is changing.</param>
    /// <param name="iCallbacksEnd">Callbacks when the text edition is finished.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fTextField_Searching(string iParent, string iPlaceHolderKey, string iText, List<UnityAction<string>> iCallbacksChange, List<UnityAction<string>> iCallbacksEnd)
    {
        GameObject lFinalObject = Instantiate(TextField_Searching);
        InputField lTextFiel_Searching = lFinalObject.GetComponentInChildren<InputField>();

        lTextFiel_Searching.GetComponentsInChildren<Text>()[0].text = Dictionary.GetString(iPlaceHolderKey);
        lTextFiel_Searching.GetComponent<InputField>().text = iText;

        // CREATE CALL BACK FOR EVERY CHANGE IN THIS TEXTFIELD
        if (iCallbacksChange != null)
        {
            foreach (UnityAction<string> lCallback in iCallbacksChange)
                lTextFiel_Searching.onValueChanged.AddListener(lCallback);
        }

        // CREATE CALL BACK FOR THE END OF EDITION OF IN THIS TEXTFIELD
        if (iCallbacksEnd != null)
        {
            foreach (UnityAction<string> lCallback in iCallbacksEnd)
                lTextFiel_Searching.onEndEdit.AddListener(lCallback);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a "terms of use" text.
    /// </summary>
    /// <param name="iParent">The parent to display the text in.</param>
    /// <param name="iText">The text to be displayed.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fTerms(string iParent, string iText)
    {
        GameObject lFinalObject = Instantiate(Text_TermsOfUses);
        //lFinalObject.GetComponent<Text>().text = iText;
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a toggle button with underlined text.
    /// </summary>
    /// <param name="iParent">The parent to display the toggle button in.</param>
    /// <param name="iText">The text to be displayed.</param>
    /// <param name="iPopup">Is the text displayed in a popup ? If so, color will change accordingly.</param>
    /// <param name="iCallbacks">Callbacks when the text is clicked.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fToggle_Underline(string iParent, string iText, bool iPopup, List<UnityAction> iCallbacks)
    {
        GameObject lFinalObject = Instantiate(Toggle_Underline);
        lFinalObject.GetComponentInChildren<Text>().text = iText;

        if (iPopup == true)
        {
            lFinalObject.GetComponentInChildren<Text>().color = new Color32(85, 85, 85, 255);
        }

        // ADD ALL CALLBACKS TO THE UNDERLINE TEXT
        Button lButton_Infos = lFinalObject.GetComponentInChildren<Button>();
        if (iCallbacks != null)
        {
            foreach (UnityAction lCallbackInfos in iCallbacks)
                lButton_Infos.onClick.AddListener(lCallbackInfos);
        }

        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Create a toggle button.
    /// </summary>
    /// <param name="iParent">The parent to display the toggle button in.</param>
    /// <param name="iKey">The dictionary key for the displayed text.</param>
    /// <param name="iPopup">Is the toggle button in a popup ? if so, color will change accordingly.</param>
    /// <returns>The generated game object with the proper configuration.</returns>
    public GameObject fToggle(string iParent, string iKey, bool iPopup)
    {
        GameObject lFinalObject = Instantiate(Toggle);
        lFinalObject.GetComponentInChildren<Text>().text = Dictionary.GetString(iKey);
        if (iPopup == true)
        {
            lFinalObject.GetComponentInChildren<Text>().color = new Color32(85, 85, 85, 255);
			lFinalObject.GetComponentInChildren<Image> ().color = new Color32 (85, 85, 85, 255);
        }
        lFinalObject.transform.SetParent(GameObject.Find(iParent).transform, false);
        return lFinalObject;
    }

    /// <summary>
    /// Get the sprite in an atlas.
    /// </summary>
    /// <param name="iSpriteName">The name of the sprite to find.</param>
    /// <returns>The sprite in either the mobile, the UI or Icon atlas.</returns>
    public Sprite GetSprite(string iSpriteName)
    {
        Sprite lSprite = new Sprite();

        if (!string.IsNullOrEmpty(iSpriteName)) {
            if (mAtlasMobile.ContainsKey(iSpriteName))
                lSprite = mAtlasMobile[iSpriteName];
            else if (mAtlasUI.ContainsKey(iSpriteName))
                lSprite = mAtlasUI[iSpriteName];
            else if (mAtlasIcon.ContainsKey(iSpriteName))
                lSprite = mAtlasIcon[iSpriteName];
        }

        return lSprite;
    }
}

