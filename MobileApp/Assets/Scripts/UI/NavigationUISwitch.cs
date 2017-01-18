using UnityEngine;

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

    public void SwitchUI()
    {
        Debug.Log("Switching UI");
        mUIOn = !mUIOn;
        navOn.SetActive(mUIOn);
        navOff.SetActive(!mUIOn);
        navigationUI.SetActive(mUIOn);
    }
}
