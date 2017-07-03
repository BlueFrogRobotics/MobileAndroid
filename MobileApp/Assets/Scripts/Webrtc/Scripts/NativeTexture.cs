using UnityEngine;
using System;

public class NativeTexture
{
	public NativeTexture(int iWidth, int iHeight, bool local)
	{
		mLocal = local;
		mNativeTexture = createTextureFromNativePtr(iWidth, iHeight);
	}

    public Texture2D createTextureFromNativePtr(int iWidth, int iHeight)
	{
		using(AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
		{
			int nativeTexturePointer = cls.CallStatic<int>(mLocal ? "createLocalTexture" : "createRemoteTexture", iWidth, iHeight);
			return Texture2D.CreateExternalTexture(iWidth, iHeight, TextureFormat.RGBA32, false, true, new IntPtr(nativeTexturePointer));
		}
	}

    public void Update()
	{
		CallStatic(mLocal ? "updateLocalTexture" : "updateRemoteTexture");
	}

	public void Destroy()
	{
		CallStatic(mLocal ? "destroyLocalTexture" : "destroyRemoteTexture");
	}

	public Texture2D texture { get { return mNativeTexture; } }

	private void CallStatic(string functionName)
	{
		using(AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
		{
			cls.CallStatic(functionName);
		}
	}

	private Texture2D mNativeTexture = null;
	private bool mLocal;
}
