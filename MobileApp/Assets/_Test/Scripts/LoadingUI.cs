using UnityEngine;
using System.Collections.Generic;

public class LoadingUI : MonoBehaviour {

    public static List<GameObject> mActiveGameObjects = new List<GameObject>();

    public static void AddObject(GameObject iObject)
    {
        mActiveGameObjects.Add(iObject);
    }

    public static void ClearUI()
    {
        if (mActiveGameObjects.Count != 0)
        {
            foreach (GameObject lGO in mActiveGameObjects)
            {
                GameObject.Destroy(lGO);
            }
            mActiveGameObjects.Clear();
        }
    }
}
