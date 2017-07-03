using UnityEngine;
using UnityEngine.EventSystems;

public class PhoneSpecifics : MonoBehaviour {
    private const float inchToCm = 2.54f;

    [SerializeField]
    private EventSystem eventSystem = null;

    [SerializeField]
    private float dragThresholdCM = 0.5f;

    void Start()
    {
        if (eventSystem == null)
        {
            eventSystem = GetComponent<EventSystem>();
        }
        SetDragThreshold();
    }

    private void SetDragThreshold()
    {
        if (eventSystem != null)
        {
            eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
        }
    }
}
