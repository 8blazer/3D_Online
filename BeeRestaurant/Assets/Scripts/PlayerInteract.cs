using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInteract : NetworkBehaviour
{
    Collider[] colliders;
    bool holding = false;
    public LayerMask layerMask;
    GameObject heldItem = null;
    List<GameObject> methodParameters = new List<GameObject>();

    [Command]
    private void ItemPlace(List<GameObject> gameObjects)
    {
        gameObjects[0].transform.position = gameObjects[1].transform.position + new Vector3(0, 1, 0);
    }

    [Command]
    private void ItemDrop(GameObject heldItem)
    {
        heldItem.GetComponent<Pickups>().held = false;
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
            Physics.Raycast(transform.position, transform.forward, out hit, 1, layerMask);
            Debug.DrawRay(transform.position, transform.forward, Color.green, 20, true);
            if (holding)
            {
                Debug.Log("hold");
                if (hit.transform.tag == "Countertop" && hit.collider.gameObject.GetComponent<Countertop>().empty)
                {
                    Debug.Log("counter");
                    methodParameters.Clear();
                    methodParameters.Add(heldItem);
                    methodParameters.Add(hit.transform.gameObject);
                    ItemPlace(methodParameters);
                }
                else if (hit.transform == null)
                {
                    Debug.Log("nocounter");
                    ItemDrop(heldItem);
                }
            }
            else if (hit.transform.gameObject.tag != null)
            {
                if (hit.transform.gameObject.tag == "Pickup")
                {
                    Debug.Log(hit.transform.gameObject);
                    ItemGrab(hit.transform.gameObject);
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
