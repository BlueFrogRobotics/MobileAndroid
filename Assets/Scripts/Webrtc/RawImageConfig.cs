using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Raw image configuration to handle the native textures for WebRTC.
/// </summary>
public class RawImageConfig : MonoBehaviour
{
    public RawImage mRawImage = null;
    bool toggle = false;

    /// <summary>
    /// Set the anchors of the raw image.
    /// </summary>
    public void setRealSize()
    {
        if (!toggle)
        {
            mRawImage.rectTransform.anchorMin = new Vector2(0.0f, 0.47f);
            mRawImage.rectTransform.anchorMax = new Vector2(1.0f, 0.82f);
        }
        else {
            mRawImage.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
            mRawImage.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        }
        toggle = !toggle;
    }
}
