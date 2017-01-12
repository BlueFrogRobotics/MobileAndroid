using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObstacleManager : MonoBehaviour {

    [SerializeField]
    private Image lvl1;

    [SerializeField]
    private Image lvl2;

    [SerializeField]
    private Image lvl3;

    [SerializeField]
    private Image lvl4;

    [HideInInspector]
    public int lvl;

    private const float SPEED_BLINK=0.01f;
    private Color mAlphaDown;
    private Color mAlphaBlink;
    private bool mUp = true;

	// Use this for initialization
	void Start () {
        lvl = 0;
        mAlphaDown  = new Color(255, 255, 255, 0);
        mAlphaBlink = new Color(255, 255, 255, 0);
        lvl1.color = mAlphaDown;
        lvl2.color = mAlphaDown;
        lvl3.color = mAlphaDown;
        lvl4.color = mAlphaDown;
    }

    void Update()
    {
        ActiveSprite();
    }

    private void selectSprite(Image lvl)
    {
        lvl1.color = mAlphaDown;
        lvl2.color = mAlphaDown;
        lvl3.color = mAlphaDown;
        lvl4.color = mAlphaDown;

        if(lvl != null)
            lvl.color = SpriteBlink();
    }
    public void ActiveSprite()
    {
        if (lvl == 0)
            selectSprite(null);
        else if (lvl == 1)
            selectSprite(lvl1);
        else if (lvl == 2)
            selectSprite(lvl2);
        else if (lvl == 3)
            selectSprite(lvl3);
        else if (lvl == 4)
            selectSprite(lvl4);        
    }

    public Color SpriteBlink()
    {
        if (mUp) {
            mAlphaBlink.a += SPEED_BLINK;
            if (mAlphaBlink.a > 1)
                mUp = false;
        } else if(!mUp) {
            mAlphaBlink.a -= SPEED_BLINK;
            if (mAlphaBlink.a < 0)
                mUp = true;
        }
        return mAlphaBlink;
    }
}
