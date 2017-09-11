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
	private DBManager dbManager;

	private string mStatus = "offline";
	private string mId = "";

	void Start()
	{
		greenImage.SetActive (false);
		orangeImage.SetActive (false);
		redImage.SetActive (true);
	}

	public void SetData(string id)
	{
		//mStatus = status;
		mId = id;
		Debug.Log ("BUDDY STATUS CIRCLE SET DATA ID " + mId);
		StartCoroutine(UpdateBuddyStatus());
	}

	private IEnumerator UpdateBuddyStatus()
	//void update()
	{
		Image[] childs = this.transform.GetComponents<Image> ();
		while (true) {
			if (!dbManager.IsBuddiesListEmpty ()) {
				//Debug.Log ("BUDDY STATUS CIRCLE BUDDIES LIST NOT EMPTY");
				foreach (BuddyDB lBuddy in dbManager.BuddiesList) {
					//Debug.Log ("BUDDY STATUS CIRCLE IDS " + lBuddy.ID + " " + mId);
					if (lBuddy.ID == mId) {
						if (lBuddy.status.Equals ("online")) {
							Debug.Log ("BUDDY STATUS CIRCLE ONLINE " + lBuddy.ID);
							redImage.SetActive (false);
							orangeImage.SetActive (false);
							greenImage.SetActive (true);
							//childs[0].color = new Color32 (18, 218, 64, 255);
						} else if (lBuddy.status.Equals ("offline")) {
							Debug.Log ("BUDDY STATUS CIRCLE OFFLINE" + lBuddy.ID);
							greenImage.SetActive (false);
							orangeImage.SetActive (false);
							redImage.SetActive (true);
							//redImage.GetComponent<Image> ().color = new Color32 (255, 0, 0, 255);
						} else if (lBuddy.status.Equals ("busy")) {
							Debug.Log ("BUDDY STATUS CIRCLE BUSY" + lBuddy.ID);
							greenImage.SetActive (false);
							redImage.SetActive (false);
							orangeImage.SetActive (true);	
							//redImage.GetComponent<Image> ().color = new Color32 (249, 145, 13, 255);	
						} else {
							Debug.Log ("BUDDY STATUS CIRCLE NOTHING");			
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
		/*greenImage.SetActive(false);
		redImage.SetActive(false);
		redImage.SetActive(true);*/
	}
}
