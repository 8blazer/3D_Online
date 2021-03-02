using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NetSync : MonoBehaviour
{
    List<GameObject> objects = new List<GameObject>();
    List<GameObject> objectsOfName = new List<GameObject>();
    List<int> objectsToRemove = new List<int>();
    int i;
    // Start is called before the first frame update
    void Start()
    {
        RefreshHeirarachy();
    }

    public void RefreshHeirarachy()
    {
        foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            objects.Add(gameObject);
        }

        foreach (GameObject gameObject in objects)
        {
            if (gameObject.name.Contains("(Clone)"))
            {
                gameObject.name.Replace("(Clone)", "");
            }
        }

        while (objects.Count > 0) //Goes through entire list of Game Objects and sorts them until there are none left
        {
            i = 0;
            foreach (GameObject gameObject in objects) //If a Game Object has the same name as the first Game Object in the list, add it to the counting list
            {
                if (gameObject.name.Split(' ')[0] == objects[0].name.Split(' ')[0])
                {
                    objectsOfName.Add(gameObject);
                    objectsToRemove.Add(i);
                }
                i++;
            }

            while (objectsToRemove.Count > 0) //Removes the Game Objects that are to be sorted from the list
            {
                objects.RemoveAt(objectsToRemove[objectsToRemove.Count - 1]);
                objectsToRemove.RemoveAt(objectsToRemove.Count - 1);
            }

            i = 1;
            foreach (GameObject finalObject in objectsOfName) //Takes Game Objects in counting list and gives them defining numbers
            {
                if (finalObject.name.Contains(" "))
                {
                    finalObject.name = finalObject.name.Split(' ')[0];
                }
                finalObject.name += " " + i.ToString();
                i++;
            }
            objectsOfName.Clear();
        }
    }
}
