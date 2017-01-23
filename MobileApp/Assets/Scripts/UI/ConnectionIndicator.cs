using UnityEngine;

public class ConnectionIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject redImage;

    [SerializeField]
    private OTONetwork oto;

    void Update()
    {
        if(oto.isActiveAndEnabled)
        {
            if (oto.HasAPeer)
                redImage.SetActive(false);
            else
                redImage.SetActive(true);
        }
    }
}