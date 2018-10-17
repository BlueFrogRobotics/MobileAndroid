using UnityEngine;
using System;

/// <summary>
/// Handler of a native texture for a display of WebRTC video streams.
/// </summary>
public class NativeTexture
{
    public Texture2D texture { get { return mNativeTexture; } }

    private Texture2D mNativeTexture = null;
    private bool mLocal;

    /// <summary>
    /// Native Texture constructor.
    /// </summary>
    /// <param name="iWidth">Width of the frame.</param>
    /// <param name="iHeight">Height of the frame.</param>
    /// <param name="local">Is the video stream local ? False if image corresponds to the remote stream.</param>
	public NativeTexture(int iWidth, int iHeight, bool local)
	{
		mLocal = local;
		mNativeTexture = createTextureFromNativePtr(iWidth, iHeight);
	}

    /// <summary>
    /// Create a texture from a native Java pointer.
    /// </summary>
    /// <param name="iWidth">Width of the frame.</param>
    /// <param name="iHeight">Height of the frame.</param>
    /// <returns>A 2D texture created from the data in the Java plugin.</returns>
    public Texture2D createTextureFromNativePtr(int iWidth, int iHeight)
	{
        // Call the Java plugin to create a texture from local or remote stream.
		using(AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
		{
			int nativeTexturePointer = cls.CallStatic<int>(mLocal ? "createLocalTexture" : "createRemoteTexture", iWidth, iHeight);
			return Texture2D.CreateExternalTexture(iWidth, iHeight, TextureFormat.RGBA32, false, true, new IntPtr(nativeTexturePointer));
		}
	}

    /// <summary>
    /// Update the texture from the video stream.
    /// </summary>
    public void Update()
	{
		CallStatic(mLocal ? "updateLocalTexture" : "updateRemoteTexture");

        //if (mLocal && mNativeTexture != null) {
        //    Color32[] pixels = mNativeTexture.GetPixels32();
        //    pixels = RotateMatrix(pixels, mNativeTexture.width, mNativeTexture.height);
        //    mNativeTexture.SetPixels32(pixels);
        //}
	}

    /// <summary>
    /// Destroy the texture.
    /// </summary>
	public void Destroy()
	{
		CallStatic(mLocal ? "destroyLocalTexture" : "destroyRemoteTexture");
	}

    /// <summary>
    /// Call a static method from the Java WebRTC plugin.
    /// </summary>
    /// <param name="functionName">Name of the function to call.</param>
	private void CallStatic(string functionName)
	{
		using(AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
		{
			cls.CallStatic(functionName);
		}
	}

    //private static Color32[] RotateMatrix(Color32[] matrix, int w, int h)
    //{
    //    Color32[] ret = new Color32[w * h];

    //    for (int i = 0; i < w; ++i) {
    //        for (int j = 0; j < h; ++j) {
    //            ret[i * h + j] = matrix[(h - j - 1) * w + i];
    //        }
    //    }

    //    return ret;
    //}
}
