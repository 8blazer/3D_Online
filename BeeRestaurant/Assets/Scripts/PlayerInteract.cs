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
    public GameObject cutFlowerPrefab;

    [Command]
    private void ItemPlace(List<GameObject> gameObjects)
    {
        gameObjects[0].transform.position = gameObjects[1].transform.position + new Vector3(0, .5f, 0);
        gameObjects[0].GetComponent<Pickups>().held = false;
        gameObjects[0].GetComponent<Pickups>().holdPlayer = null;
        gameObjects[0].transform.parent = gameObjects[1].transform;
    }

    [Command]
    private void ItemDrop()
    {
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
    }

    [Command]
    private void ItemGrab()
    {
        heldItem.GetComponent<Pickups>().held = true;
        heldItem.GetComponent<Pickups>().holdPlayer = this.gameObject.transform.GetChild(1);
        if (heldItem.transform.parent != null)
        {
            heldItem.transform.parent.DetachChildren();
        }
    }

    [Command]
    private void ItemCut(Transform cutItem)
    {
        Instantiate(cutFlowerPrefab, cutItem.transform.position, Quaternion.identity);
        Destroy(cutItem.gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!hasAuthority) { return; }

        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (holding)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
                {
                    if (hit.transform.tag == "ItemPlace" && hit.collider.transform.childCount == 0)
                    {
                        methodParameters.Clear();
                        methodParameters.Add(heldItem);
                        methodParameters.Add(hit.transform.gameObject);
                        ItemPlace(methodParameters);
                        holding = false;
                    }
                }
                else
                {
                    ItemDrop();
                    holding = false;
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1, grabLayerMask))
                {
                    if (hit.transform.gameObject.tag == "Pickup")
                    {
                        heldItem = hit.transform.gameObject;
                        ItemGrab();
                        holding = true;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            colliders = Physics.OverlapBox(transform.position, new Vector3(0, 0, 0), Quaternion.identity);

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
            {
                if (hit.transform.gameObject.tag == "ItemPlace" && hit.collider.transform.childCount > 0)
                {
                    if (hit.transform.GetChild(0).GetComponent<Pickups>().cuttable == true)
                    {
                        ItemCut(hit.transform.GetChild(0));
                    }
                }
            }
        }
    }
}

/*
 * Issues:
 * Host can perform every function perfectly until they try to place a cut flower onto a table
 * Client can perform no functions at all, though they used to be able to at least grab and drop flowers onto the ground
 * Already cut flowers do not appear when client joins an already running server, though this should eventually never matter
 * I think the key to all this is that objects are somehow not perfectly getting sent to clients
 */
