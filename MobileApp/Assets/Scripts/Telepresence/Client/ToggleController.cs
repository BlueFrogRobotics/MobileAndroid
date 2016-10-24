using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleController : MonoBehaviour {

    public bool state = false;
    public GameObject joystickHead;
    public GameObject joystickBody;
    public float speed;
    public GameObject Toggle;
    public Text Head;
    public Text Body;
    Color gray = new Color(148,148,148,255);
    Color black= new Color(0, 0, 0, 255);

    public float PosMax =  70.5f;
    public float PosMin = -70.5f;
    public float speedX=30;
    float speedY = 0;
    float speedZ = 0;


    public void Move2Head()
    {
        Toggle.transform.Translate(new Vector3(speedX, speedY, speedZ)*Time.deltaTime);
        Head.color = black;
        Body.color = gray;
        joystickBody.SetActive(false);
        joystickHead.SetActive(true);
    }
    public void Move2Body()
    {
        Toggle.transform.Translate(new Vector3(-speedX, speedY, speedZ) * Time.deltaTime);
        Head.color = gray;
        Body.color = black;
        joystickBody.SetActive(true);
        joystickHead.SetActive(false);
    }

    public void toggle()
    {
        state = !state;
    }

    void FixedUpdate()
    {
        if (state && Toggle.transform.localPosition.x < PosMax)
        {
            Move2Head();
        }
        else if (!state && Toggle.transform.localPosition.x > PosMin)
        {
            Move2Body();
        }
    }

}
