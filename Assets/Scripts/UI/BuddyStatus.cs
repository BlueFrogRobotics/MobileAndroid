using UnityEngine;
using UnityEngine.UI;

using System.Collections;

/// <summary>
/// Manages the Buddy status UI.
/// </summary>
public class BuddyStatus : MonoBehaviour
{
	[SerializeField]
	private GameObject redImage;

	[SerializeField]
	private GameObject orangeImage;

	[SerializeField]
	private GameObject greenImage;

	[SerializeField]
	private GameObject text;

	[SerializeField]
	private DBManager dbManager;

	private string mId = "";

	void Start()
	{
        // Display all Buddies as "offline" at the start.
		greenImage.SetActive (false);
		orangeImage.SetActive (false);
		redImage.SetActive (true);
	}

    /// <summary>
    /// Set the data for the current Buddy and start a Coroutine to update its status.
    /// </summary>
    /// <param name="id">Buddy's unique ID</param>
    /// <param name="displayText">The display text.</param>
	public void SetData(string id, bool displayText)
	{
		mId = id;
		if (displayText) {
			text.SetActive(true);
			text.GetComponent<Text>().text = "Hors ligne";
		} else {
			text.SetActive (false);
		}

		StartCoroutine(UpdateBuddyStatus());
	}

    /// <summary>
    /// Is the Buddy accessible for a remote control ?
    /// </summary>
    /// <param name="id">Buddy's unique ID.</param>
    /// <param name="startRC">Is WebRTC initialized ?</param>
    /// <returns>True if buddy is accessible for a WebRTC session.</returns>
	public bool BuddyAccess(string id, bool startRC)
	{
		if (!dbManager.IsBuddiesListEmpty ()) {
			foreach (BuddyDB lBuddy in dbManager.BuddiesList) {
				if (lBuddy.ID.ToUpper() == id.ToUpper() && !lBuddy.status.Equals ("offline")) {
					if (!startRC) {
						return true;
					}
					if(!lBuddy.appName.Equals("RemoteControl")) {
						return true;
					}
				}
			}
		}

		return false;
	}

    /// <summary>
    /// Coroutine to update Buddy's status.
    /// </summary>
    /// <returns>An enumerator.</returns>
	private IEnumerator UpdateBuddyStatus()
	{
		Image[] childs = this.transform.GetComponents<Image> ();
		while (true) {
			if (!dbManager.IsBuddiesListEmpty ()) {
                // Check for each Buddy if it's online, busy or offline.
				foreach (BuddyDB lBuddy in dbManager.BuddiesList) {
					if (lBuddy.ID == mId) {
						if (lBuddy.status.Equals ("online")) {
							redImage.SetActive (false);
							orangeImage.SetActive (false);
							greenImage.SetActive (true);
							if (text.activeInHierarchy) {
								text.GetComponent<Text>().text = "En ligne";
							}
						} else if (lBuddy.status.Equals ("offline")) {
							greenImage.SetActive (false);
							orangeImage.SetActive (false);
							redImage.SetActive (true);
							if (text.activeInHierarchy) {
								text.GetComponent<Text>().text = "Hors ligne";
							}
						} else if (lBuddy.status.Equals ("busy")) {
							greenImage.SetActive (false);
							redImage.SetActive (false);
							orangeImage.SetActive (true);
							if (text.activeInHierarchy) {
								text.GetComponent<Text>().text = "Occupé... (" + lBuddy.appName + ")";
							}
						}

						break;
					}
				}
			}

			yield return new WaitForSeconds (5F);
		}
	}

    /// <summary>
    /// Close connection and set the Buddy as "offline".
    /// </summary>
	public void CloseConnection()
	{
		greenImage.SetActive(false);
		redImage.SetActive(false);
		redImage.SetActive(true);
	}
}
