using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager of the sound streams during a remote control session.
/// </summary>
public class SoundManager : MonoBehaviour
{
	[SerializeField]
	private Image lButtonImage;

	[SerializeField]
	private GameObject mWebRTC = null;

	/*[SerializeField]
	private PoolManager mPoolManager = null;*/

	public static bool reset = true;
	private bool mActive = true;

	public void Update()
	{
		if (reset) {
			reset = false;
			mActive = true;
			lButtonImage.sprite = GameObject.Find ("CanvasApp").GetComponent<PoolManager> ().GetSprite ("Micro");
		}
	}

    /// <summary>
    /// Toggle on or off the sound depending on the previous state.
    /// </summary>
	public void onToggleSound()
	{
        // Change only if the communication is established and active.
		if (mWebRTC.activeSelf) {
			mActive = !mActive;
            // Enable or disable the local audio stream.
			using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
			{
				cls.CallStatic("setSoundActive", mActive);
			}

            // Update the microphone image depending on its state.
			lButtonImage.sprite =  GameObject.Find("CanvasApp").GetComponent<PoolManager>().GetSprite(mActive ? "Micro" : "MicroOff");
		}
	}
}
