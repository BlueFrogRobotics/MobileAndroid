using UnityEngine;

/// <summary>
/// Manages the enabling/disabling of the Navigation UI (joystick, toggle switch, sensors UI, etc.)
/// </summary>
public class NavigationUISwitch : MonoBehaviour
{
    [SerializeField]
    private GameObject navOn;

    [SerializeField]
    private GameObject navOff;

    [SerializeField]
    private GameObject navigationUI;

    private bool mUIOn;

    // Use this for initialization
    void Start()
    {
        mUIOn = true;
    }

    //Enable or disable the UI depending on previous state
    public void SwitchUI()
    {
        Debug.Log("Switching UI");
        mUIOn = !mUIOn;
        navOn.SetActive(mUIOn);
        navOff.SetActive(!mUIOn);
        navigationUI.SetActive(mUIOn);
    }
}
