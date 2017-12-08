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

		int nativeTexturePointer = NativeWrapper.CreateTextureWrapper (mLocal, iWidth, iHeight);

		if (nativeTexturePointer == NativeWrapper.UNDEFINED_TEXTURE_POINTER)
		{
			Debug.Log ("NATIVETEXTURE - Error creating" + ( mLocal ? "local" : "remote" ) + "native texture");
		}
		else 
		{
			Debug.Log ("NATIVETEXTURE - " + (mLocal ? "LOCAL" : "REMOTE") + "ID = " + nativeTexturePointer);
		}
			
		return Texture2D.CreateExternalTexture (iWidth, iHeight, TextureFormat.RGBA32, false, true, new IntPtr (nativeTexturePointer));
	}

    public void Update()
	{
		NativeWrapper.UpdateTextureWrapper (mLocal);	
	}

	public void Destroy()
	{
		NativeWrapper.DestroyTextureWrapper (mLocal);	
	}

	public Texture2D texture { get { return mNativeTexture; } }

	private Texture2D mNativeTexture = null;
	private bool mLocal;
}
