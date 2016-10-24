using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObstacleManager : MonoBehaviour {
    public Image mLvl0;
    public Image mLvl1;
    public Image mLvl2;
    public Image mLvl3;
    public Image mLvl4;
    private const float SPEED_BLINK=0.01f;
    private Color mAlphaDown;
    private Color mAlphaBlink;
    private bool mUp = true;
    [HideInInspector]
    public int lvl;

	// Use this for initialization
	void Start () {
        lvl = 0;
        mAlphaDown  = new Color(255, 255, 255, 0);
        mAlphaBlink = new Color(255, 255, 255, 0);
        mLvl1.color = mAlphaDown;
        mLvl2.color = mAlphaDown;
        mLvl3.color = mAlphaDown;
        mLvl4.color = mAlphaDown;
    }

    private void selectSprite(Image lvl)
    {
        mLvl1.color = mAlphaDown;
        mLvl2.color = mAlphaDown;
        mLvl3.color = mAlphaDown;
        mLvl4.color = mAlphaDown;
        lvl.color = spriteBlink();
    }
    public void ActiveSprite()
    {
        if (lvl == 0)
        {
            selectSprite(mLvl0);
        }
        else if (lvl == 1)
        {
            selectSprite(mLvl1);
            
        }
        else if (lvl == 2)
        {
            selectSprite(mLvl2);
        }
        else if (lvl == 3)
        {
            selectSprite(mLvl3);
        }
        else if (lvl == 4)
        {
            selectSprite(mLvl4);
        }
        
    }
    public Color spriteBlink()
    {
        if (mUp)
        {
            mAlphaBlink.a += SPEED_BLINK;
            if (mAlphaBlink.a > 1)
            {
                mUp = false;
            }
        }
        else if(!mUp)
        {
            mAlphaBlink.a -= SPEED_BLINK;
            if (mAlphaBlink.a < 0)
            {
                mUp = true;
            }
        }
        return mAlphaBlink;
    }

	void Update ()
    {
        ActiveSprite();
	}
}
