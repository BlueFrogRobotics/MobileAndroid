using UnityEngine;
using UnityEngine.UI;

using System.Collections;

/// <summary>
/// Manages the Buddy status UI
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
		greenImage.SetActive (false);
		orangeImage.SetActive (false);
		redImage.SetActive (true);
	}

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

	private IEnumerator UpdateBuddyStatus()
	{
		Image[] childs = this.transform.GetComponents<Image> ();
		while (true) {
			if (!dbManager.IsBuddiesListEmpty ()) {
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

	public void CloseConnection()
	{
		greenImage.SetActive(false);
		redImage.SetActive(false);
		redImage.SetActive(true);
	}
}
