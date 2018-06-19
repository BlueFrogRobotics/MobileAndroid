using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles the list and the destruction of game objects that were procedurally created.
/// </summary>
public class LoadingUI : MonoBehaviour {

    public static List<GameObject> mActiveGameObjects = new List<GameObject>();

    public static void AddObject(GameObject iObject)
    {
        mActiveGameObjects.Add(iObject);
    }

    /// <summary>
    /// Destroy all game objects from a specific screen to leave space for the next screen to display.
    /// </summary>
    public static void ClearUI()
    {
        //REMOVE ALL CREATING OBJECTS TO CLEAR AND DISPLAY NEXT ONES
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
