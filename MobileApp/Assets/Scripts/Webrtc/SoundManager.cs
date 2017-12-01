using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
	[SerializeField]
	private Image lButtonImage;

	[SerializeField]
	private GameObject mWebRTC = null;

	/*[SerializeField]
	private PoolManager mPoolManager = null;*/

	private bool mActive = true;

	public void onToggleSound()
	{
		if (mWebRTC.activeSelf)
		{
			mActive = !mActive;
			using (AndroidJavaClass cls = new AndroidJavaClass("com.bfr.unityrtc.Webrtc"))
			{
				cls.CallStatic("SetSoundActive", mActive);
			}

			lButtonImage.sprite =  GameObject.Find("CanvasApp").GetComponent<PoolManager>().GetSprite(mActive ? "Micro" : "MicroOff");
		}
	}
}
