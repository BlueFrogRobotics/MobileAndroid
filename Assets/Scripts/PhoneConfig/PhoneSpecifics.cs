using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class handles the drag event.
/// </summary>
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

    /// <summary>
    /// Set the drag threshold a bit bigger than normal to let users properly press on big buttons.
    /// </summary>
    private void SetDragThreshold()
    {
        if (eventSystem != null)
        {
            eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
        }
    }
}
