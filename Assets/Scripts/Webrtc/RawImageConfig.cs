using UnityEngine;
using UnityEngine.UI;

public class RawImageConfig : MonoBehaviour
{

    public RawImage mRawImage = null;
    bool toggle = false;

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
