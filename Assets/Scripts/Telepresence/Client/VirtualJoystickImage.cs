using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        if (mIsDragging && mHaloColor.a < 255) {
            mHaloColor.a += fadeSpeed;
            SetHaloAlpha(mHaloColor);
        }
    }

    private void SetHaloAlpha(Color iAlpha)
    {
        Image[] lImages = haloImage.GetComponentsInChildren<Image>();

        foreach(Image lImage in lImages) {
            lImage.color = iAlpha;
        }
    }

    //Called when the joystick cursor is being dragged
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

    public virtual void OnPointerDown(PointerEventData iPed)
    {
        //Temporal offset to activate the white Halo or switch to the other joystick
        mTime = Time.time;
        mDown = true;
    }

    public virtual void OnPointerUp(PointerEventData iPed)
    {
        //Return Handler to origin
        mIsDragging = false;
        mJoystickHandle.color = mAlphaDown;
        SetHaloAlpha(mAlphaDown);
        mJoystickHandle.rectTransform.anchoredPosition = new Vector3(0, 0, 0);

        if (Time.time - mTime < 0.2 && mDown)
            mDown = false;
    }

    //This function allows to switch between Landscape and Portrait display.
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
