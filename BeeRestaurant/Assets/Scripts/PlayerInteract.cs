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
    public GameObject cutFlowerPrefab;

    [Command]
    private void CmdItemPlace(GameObject counter)
    {
        heldItem.transform.position = counter.transform.position + new Vector3(0, .5f, 0);
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
        heldItem.transform.parent = counter.transform;
    }

    [Command]
    private void CmdItemDrop()
    {
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
    }

    [Command]
    private void CmdItemGrab()
    {
        heldItem.GetComponent<Pickups>().held = true;
        heldItem.GetComponent<Pickups>().holdPlayer = this.gameObject.transform.GetChild(1);
        if (heldItem.transform.parent != null)
        {
            heldItem.transform.parent.DetachChildren();
        }
    }

    [Command]
    private void CmdItemCut(Transform cutItem)
    {
        GameObject cutFlower = Instantiate(cutFlowerPrefab, cutItem.transform.position, Quaternion.identity);
        cutFlower.transform.parent = cutItem.transform.parent;
        NetworkServer.Spawn(cutFlower);
        Destroy(cutItem.gameObject);
    }

    // Update is called once per frame
    [ClientCallback]
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
                        CmdItemPlace(hit.transform.gameObject);
                        holding = false;
                    }
                }
                else
                {
                    CmdItemDrop();
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
                        CmdItemGrab();
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
                if (hit.transform.gameObject.tag == "ItemPlace" && hit.transform.gameObject.name == "CuttingBoard" && hit.collider.transform.childCount > 0)
                {
                    if (hit.transform.GetChild(0).GetComponent<Pickups>().cuttable == true)
                    {
                        CmdItemCut(hit.transform.GetChild(0));
                    }
                }
            }
        }
    }
}

/*
 * Issues:
 * Client can perform no interacting functions at all, though they used to be able to at least grab and drop flowers onto the ground
 * Flowers on tables aren't getting properly sent to client, they aren't children of tables
 * Flowers don't spawn if they've already spawned in previous hosting without restarting program
 * Client authority is set to false, yet the client somehow still runs past the hasAuthority check
 * I think the key to all this is that objects are somehow not perfectly getting sent to clients
 */
