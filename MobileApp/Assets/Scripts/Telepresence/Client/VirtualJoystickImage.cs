using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class VirtualJoystickImage : MonoBehaviour, IDragHandler,IPointerUpHandler,IPointerDownHandler {
    private Image bgImage;
    private Image JoystickHandle;
    private Image bgImageOn;
    private Vector3 InputVector;
    private Color alphaDown = new Color(255, 255, 255, 0);
    private Color alphaUp = new Color(255, 255, 255, 255);
    private bool down = false;
    bool isJoined = false;
    private float time;
    public bool isDragging = false;
    public bool isBody;
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private GameObject Toggle;
    [SerializeField]
    private float fadeSpeed=0.01f;

    private void Start()
    {
        bgImage = GetComponent<Image>();
        JoystickHandle = transform.GetChild(0).GetComponent<Image>();
        bgImageOn = transform.GetChild(1).GetComponent<Image>();
        
    }

    void Update()
    {
        if (isDragging && bgImageOn.color.a < 255)
        {
            Color alpha = bgImageOn.color;
            alpha.a += fadeSpeed;
            bgImageOn.color = alpha;
        }
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        isDragging = true;
        JoystickHandle.color = alphaUp;
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform,
            ped.position, ped.pressEventCamera, out pos))
        {
            //Get the input info
            pos.x = (pos.x / bgImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImage.rectTransform.sizeDelta.y);
            InputVector = new Vector3(pos.x * 2, pos.y * 2);
            InputVector = (InputVector.magnitude > 1.0f) ? InputVector.normalized : InputVector; //Normalized the vector to follow a circle
            //Move the Handler
            JoystickHandle.rectTransform.anchoredPosition = new Vector3(InputVector.x * (bgImage.rectTransform.sizeDelta.x / radius),
                InputVector.y * (bgImage.rectTransform.sizeDelta.y / radius));

        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        //Temporal offset to activate the white Halo or switch to the other joystick
        time = Time.time;
        down = true;
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        //Return Handler to origin
        isDragging = false;
        JoystickHandle.color = alphaDown;
        bgImageOn.color = alphaDown;
        JoystickHandle.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        if (Time.time - time < 0.2 && down)
        {
            down = false;
            Toggle.GetComponent<ToggleController>().toggle();
        }

    }

    //This function allowed the switching between Landscape and Portrait display.
    void OnDisable()
    {
        isDragging = false;
        JoystickHandle.color = alphaDown;
        bgImageOn.color = alphaDown;
        JoystickHandle.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        if (Time.time - time < 0.2 && down)
        {
            down = false;
            Toggle.GetComponent<ToggleController>().toggle();
        }
    }
}
