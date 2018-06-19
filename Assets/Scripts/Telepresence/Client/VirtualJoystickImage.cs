using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI Manager of the virtual joystick.
/// </summary>
public class VirtualJoystickImage : MonoBehaviour, IDragHandler,IPointerUpHandler,IPointerDownHandler {

    [SerializeField]
    private float radius = 2f;

    [SerializeField]
    private GameObject haloImage;

    [SerializeField]
    private float fadeSpeed = 0.01f;

    public bool mIsDragging;
    private bool mDown;
    private bool mIsJoined;
    private float mTime;
    private Color mHaloColor;
    private Image mBGImage;
    private Image mJoystickHandle;
    private Vector3 mInputVector;
    private Color mAlphaDown;
    private Color mAlphaUp;

    private void Start()
    {
        // Initialize the members.
        mIsDragging = false;
        mDown = false;
        mIsJoined = false;
        mHaloColor = new Color(255, 255, 255, 0);
        mAlphaDown = new Color(255, 255, 255, 0);
        mAlphaUp = new Color(255, 255, 255, 255);
        mBGImage = GetComponent<Image>();
        mJoystickHandle = transform.GetChild(0).GetComponent<Image>();  
    }

    void Update()
    {
        // Slowly increase the alpha of the halo when the joystick is being dragged.
        if (mIsDragging && mHaloColor.a < 255) {
            mHaloColor.a += fadeSpeed;
            SetHaloAlpha(mHaloColor);
        }
    }

    /// <summary>
    /// Set the halo image's alpha value.
    /// </summary>
    /// <param name="iAlpha"></param>
    private void SetHaloAlpha(Color iAlpha)
    {
        Image[] lImages = haloImage.GetComponentsInChildren<Image>();

        foreach(Image lImage in lImages) {
            lImage.color = iAlpha;
        }
    }

    /// <summary>
    /// Called when the joystick cursor is being dragged.
    /// </summary>
    /// <param name="iPed">The position of the user cursor.</param>
    public virtual void OnDrag(PointerEventData iPed)
    {
        mIsDragging = true;
        mJoystickHandle.color = mAlphaUp;
        Vector2 lPos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mBGImage.rectTransform,
            iPed.position, iPed.pressEventCamera, out lPos)) {
            //Get the input info
            lPos.x = (lPos.x / mBGImage.rectTransform.sizeDelta.x);
            lPos.y = (lPos.y / mBGImage.rectTransform.sizeDelta.y);
            mInputVector = new Vector3(lPos.x * 2, lPos.y * 2);
            //Normalized the vector to follow a circle
            mInputVector = (mInputVector.magnitude > 1.0f) ? mInputVector.normalized : mInputVector;
            //Move the Handler
            mJoystickHandle.rectTransform.anchoredPosition = new Vector3(mInputVector.x * (mBGImage.rectTransform.sizeDelta.x / radius),
                mInputVector.y * (mBGImage.rectTransform.sizeDelta.y / radius));

        }
    }

    /// <summary>
    /// Called whenever a user points down on the screen.
    /// </summary>
    /// <param name="iPed">The position of the user cursor.</param>
    public virtual void OnPointerDown(PointerEventData iPed)
    {
        //Temporal offset to activate the white Halo or switch to the other joystick
        mTime = Time.time;
        mDown = true;
    }

    /// <summary>
    /// Called whenever a user doesn't point on the screen anymore.
    /// </summary>
    /// <param name="iPed">The position of the user cursor.</param>
    public virtual void OnPointerUp(PointerEventData iPed)
    {
        //Return Handler to origin and set the halo to alpha 0.
        mIsDragging = false;
        mJoystickHandle.color = mAlphaDown;
        mHaloColor = mAlphaDown;
        SetHaloAlpha(mAlphaDown);
        mJoystickHandle.rectTransform.anchoredPosition = new Vector3(0, 0, 0);

        if (Time.time - mTime < 0.2 && mDown)
            mDown = false;
    }

    void OnDisable()
    {
        mIsDragging = false;
        mJoystickHandle.color = mAlphaDown;
        SetHaloAlpha(mAlphaDown);
        mJoystickHandle.rectTransform.anchoredPosition = new Vector3(0, 0, 0);

        if (Time.time - mTime < 0.2 && mDown)
            mDown = false;
    }
}
