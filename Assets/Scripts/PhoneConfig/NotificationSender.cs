using UnityEngine;

public class NotificationSender : MonoBehaviour {

    /// <summary>
    /// Display an Android notification on the device with preset content.
    /// </summary>
    public void SendNotification()
    {
        LocalNotification.SendNotification(4, 2, "BuddyApp", "You have a new friend!", new Color32(0x21, 0xba, 0xed, 255));
    }

    /// <summary>
    /// Display an Android notification on the device.
    /// </summary>
    /// <param name="iMessage"></param>
    /// <param name="iTitle"></param>
    public void SendNotification(string iMessage, string iTitle = "Buddy needs you")
    {
        Debug.Log("Sending notification " + iTitle + " : " + iMessage);
        LocalNotification.SendNotification(1, 5, iTitle, iMessage, new Color32(0x21, 0xba, 0xed, 255));
    }
}
