using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInteract : NetworkBehaviour
{
    Collider[] colliders;
    bool holding = false;
    public LayerMask grabLayerMask;
    public LayerMask dropLayerMask;
    GameObject heldItem = null;
    List<GameObject> methodParameters = new List<GameObject>();

    [Command]
    private void ItemPlace(List<GameObject> gameObjects)
    {
        gameObjects[0].transform.position = gameObjects[1].transform.position + new Vector3(0, .5f, 0);
        gameObjects[0].GetComponent<Pickups>().held = false;
        gameObjects[0].GetComponent<Pickups>().holdPlayer = null;
    }

    [Command]
    private void ItemDrop(GameObject droppedItem)
    {
        heldItem = droppedItem;
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
    }

    [Command]
    private void ItemGrab(GameObject grabbedItem)
    {
        heldItem = grabbedItem;
        heldItem.GetComponent<Pickups>().held = true;
        heldItem.GetComponent<Pickups>().holdPlayer = this.gameObject.transform.GetChild(1);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            if (holding)
            {
                Debug.Log("hold");
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
                {
                    if (hit.transform.tag == "ItemPlace" && hit.collider.gameObject.GetComponent<Countertop>().empty)
                    {
                        Debug.Log("counter");
                        methodParameters.Clear();
                        methodParameters.Add(heldItem);
                        methodParameters.Add(hit.transform.gameObject);
                        ItemPlace(methodParameters);
                    }
                }
                else //if (hit.transform == null || hit.transform.gameObject == heldItem)
                {
                    Debug.Log("nocounter");
                    ItemDrop(heldItem);
                }
                holding = false;
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1, grabLayerMask))
                {
                    if (hit.transform.gameObject.tag == "Pickup")
                    {
                        Debug.Log(hit.transform.gameObject);
                        ItemGrab(hit.transform.gameObject);
                        holding = true;
                    }
                }
            }
            
            /*
            colliders = Physics.OverlapBox(transform.position, new Vector3(0,0,0), Quaternion.identity);
            foreach (Collider collider in colliders)
            {
                if (holding)
                {
                    //check for shelf
                    if (collider.tag == "Countertop" && collider.GetComponent<Countertop>().empty)
                    {

                    }
                }
                else
                {
                    //check for item on ground
                }
            }
            */
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            colliders = Physics.OverlapBox(transform.position, new Vector3(0, 0, 0), Quaternion.identity);
            //check for task
        }
    }
}
