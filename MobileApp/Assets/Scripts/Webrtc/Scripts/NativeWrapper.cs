using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;


public class NativeWrapper : MonoBehaviour
{
	public const int UNDEFINED_TEXTURE_POINTER = -1;

	#if UNITY_IOS

	[DllImport ("__Internal")]
	private static extern void ConnectToCrossbar(string uri, string realm, string certificate, string key, string receiver);

	[DllImport ("__Internal")]
	private static extern void SetupIOSRTC(string localUser, string remoteUser, string receiver);

	[DllImport ("__Internal")]
	private static extern void StartIOSRTC ();

	[DllImport ("__Internal")]
	private static extern void StopIOSRTC ();

	[DllImport ("__Internal")]
	private static extern void Call ();

	[DllImport ("__Internal")]
	private static extern void Hangup ();

	[DllImport ("__Internal")]
	private static extern void SendPeerMessage (string message);

	[DllImport ("__Internal")]
	private static extern int  CreateTexture (bool isLocal, int width, int height);

	[DllImport ("__Internal")]
	private static extern void UpdateTexture (bool isLocal);

	[DllImport ("__Internal")] 
	private static extern void DestroyTexture (bool isLocal);

	[DllImport ("__Internal")]
	private static extern void PublishConnectionRequest();

	[DllImport ("__Internal")]
	private static extern void SubscribeToChat(string buddyId);

	[DllImport ("__Internal")]
	private static extern void SendChatMessage(string message);

	[DllImport ("__Internal")]
	private static extern void SubscribeToStatus(string buddyList);

	#elif UNITY_ANDROID

	private const string ANDROID_PACKAGE_NAME = "com.bfr.";
	private const string WEBRTC_JAVA_CLASS = ANDROID_PACKAGE_NAME + "unityrtc.Webrtc";
	private const string BACKGROUNDLISTENER_JAVA_CLASS = ANDROID_PACKAGE_NAME + "mobile.BackgroundListener";

	private static AndroidJavaObject mJavaListener;

	private static void CallAndroidInstanceMethod(AndroidJavaObject instance, string methodName, params object[] args)
	{
		if (instance == null)
		{
			Debug.Log ("NATIVEWRAPPER - Error CallAndroidInstanceMethod was called on a null object");
			return;
		}

		instance.Call (methodName, args);
	}

	#endif

	/*
	 * Crossbar management methods
	 */

	public static void SetupCrossbarConnectionWrapper(string crossbarUri, string realm, string certificate, string key)
	{
		#if (UNITY_IOS)

		ConnectToCrossbar(crossbarUri, realm, certificate, key, "BackgroundListener");

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

			mJavaListener = new AndroidJavaObject(BACKGROUNDLISTENER_JAVA_CLASS, crossbarUri, realm, certificate, key, jo);
		}

		Debug.Log ("NATIVEWRAPPER - " + (mJavaListener ==  null ? "Error" : "Success") + " instantiating android BackgroudListener");

		#endif
	}

	public static void PublishConnectionRequest (string message)
	{
		#if (UNITY_IOS)

		PublishConnectionRequest();

		#elif (UNITY_ANDROID)

		CallAndroidInstanceMethod(mJavaListener, "Publish", "ConnectRequest", message);

		#endif
	}

	public static void SubscribeToChatWrapper (string buddyId)
	{
		#if (UNITY_IOS)

		SubscribeToChat(buddyId);

		#elif (UNITY_ANDROID)

		CallAndroidInstanceMethod(mJavaListener, "SubscribeChat", buddyId);

		#endif
	}

	public static void SubscribeToStatusWrapper (string buddyId)
	{
		#if (UNITY_IOS)

		SubscribeToStatus(buddyId);

		#elif (UNITY_ANDROID)

		CallAndroidInstanceMethod(mJavaListener, "SubscribeStatus", buddyId);

		#endif
	}

	public static void SendChatMessageWrapper (string message)
	{
		#if (UNITY_IOS)

		SendChatMessage(message);

		#elif (UNITY_ANDROID)

		CallAndroidInstanceMethod(mJavaListener, "SendChatMessage", message);

		#endif
	}

	public static void SubscribeToNotificationChannelsWrapper (String buddyIds)
	{
		#if (UNITY_IOS)

		//NOT TODO FOR NOW

		#elif (UNITY_ANDROID)

		CallAndroidInstanceMethod(mJavaListener , "StartNotificationService", buddyIds);

		#endif
	}

	public static void PublishNotificationWrapper (string message)
	{
		#if (UNITY_IOS)

		//NOT TODO FOR NOW

		#elif (UNITY_ANDROID)

		CallAndroidInstanceMethod(mJavaListener , "Publish", "NotificationChannel", message);

		#endif
	}

	public static void UnsubscribeFromNotificationChannelsWrapper ()
	{
		#if (UNITY_IOS)

		//NOT TODO FOR NOW

		#elif (UNITY_ANDROID)

		CallAndroidInstanceMethod(mJavaListener , "StopNotificationService");

		#endif
	}



	/*
	 * Webrtc management methods
	 */

	public static void SetupWebrtcWrapper (string crossbarUri, string realm, string localUser, string remoteUser, string receiverName, string certificate)
	{
		#if (UNITY_IOS)

		SetupIOSRTC(localUser, remoteUser, receiverName);

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				// QUESTION : IS THAT NEEDED TO GET ANDROID CONTEXT ?
				AndroidJavaObject context = jc.GetStatic<AndroidJavaObject>("currentActivity");

				cls.CallStatic("SetupWebrtc", crossbarUri, realm, context, localUser, receiverName, certificate);
			}
		}

		#endif
	}

	public static void StartWebrtcWrapper ()
	{
		#if (UNITY_IOS)

		StartIOSRTC();

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			cls.CallStatic("StartWebrtc");
		}

		#endif
	}

	public static void StopWebrtcWrapper ()
	{
		#if (UNITY_IOS)

		StopIOSRTC();

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			cls.CallStatic("StopWebrtc");
		}

		#endif
	}

	public static void CallWrapper (string remoteUser)
	{
		#if (UNITY_IOS)

		Call();

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			cls.CallStatic("Call", remoteUser);
		}

		#endif
	}

	public static void HangupWrapper (string remoteUser)
	{
		#if (UNITY_IOS)

		Hangup();

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			cls.CallStatic("Hangup", remoteUser);
		}

		#endif
	}

	public static void SendMessageWrapper (string message, string remoteUser)
	{
		#if (UNITY_IOS)

		SendPeerMessage(message);

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			cls.CallStatic("SendMessage", message, remoteUser);
		}

		#endif
	}

	public static void SetSoundActiveWrapper (bool active)
	{
		#if (UNITY_IOS)

		// TODO

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			cls.CallStatic("SetSoundActive", active);
		}

		#endif
	}


	/**
	 * Texture related methods
	 **/

	public static int CreateTextureWrapper (bool isLocal, int width, int height)
	{
		int texturePointer = UNDEFINED_TEXTURE_POINTER;

		#if (UNITY_IOS)

		texturePointer = CreateTexture (isLocal, width, height);

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			if (isLocal)
			{
				texturePointer = cls.CallStatic<int>("createLocalTexture", width, height);
			}
			else
			{
				texturePointer = cls.CallStatic<int>("createRemoteTexture", width, height);
			}
		}

		#endif

		return texturePointer;
	}

	public static void UpdateTextureWrapper (bool isLocal)
	{
		#if (UNITY_IOS)

		UpdateTexture(isLocal);

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			if (isLocal)
			{
				cls.CallStatic("updateLocalTexture");
			}
			else
			{
				cls.CallStatic("updateRemoteTexture");

			}
		}

		#endif
	}

	public static void DestroyTextureWrapper (bool isLocal)
	{
		#if (UNITY_IOS)

		DestroyTexture(isLocal);

		#elif (UNITY_ANDROID)

		using (AndroidJavaClass cls = new AndroidJavaClass(WEBRTC_JAVA_CLASS))
		{
			if (isLocal)
			{
				cls.CallStatic("destroyLocalTexture");
			}
			else
			{
				cls.CallStatic("destroyRemoteTexture");

			}
		}

		#endif
	}
}