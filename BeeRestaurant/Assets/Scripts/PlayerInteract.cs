using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerInteract : NetworkBehaviour
{
    Collider[] colliders;
    public bool holding = false;
    public LayerMask grabLayerMask;
    public LayerMask dropLayerMask;
    public GameObject heldItem = null;
    public GameObject cutFlowerPrefab;
    RaycastHit hit;

    [Command]
    private void CmdItemPlace(GameObject counter)
    {
        heldItem.transform.position = counter.transform.position + new Vector3(0, .5f, 0);
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
        heldItem.transform.parent = counter.transform;
    }

    [Client]
    private void ClntItemPlace(GameObject counter)
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

    [Client]
    private void ClntItemDrop()
    {
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
    }

    
    [Command]
    private void CmdItemGrab(string name)
    {
        GameObject grabbedItem = GameObject.Find(name);
        grabbedItem.GetComponent<Pickups>().held = true;
        //grabbedItem.GetComponent<Pickups>().holdPlayer = this.gameObject.transform.GetChild(1);
        grabbedItem.transform.parent = null;
    }

    [Client]
    private void ClntItemGrab()
    {
        heldItem.GetComponent<Pickups>().held = true;
        heldItem.GetComponent<Pickups>().holdPlayer = this.gameObject.transform.GetChild(1);
        heldItem.transform.parent = null;
    }

    [Command]
    private void CmdItemCut(Transform cutItem)
    {
        hit.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
        hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value += 1 * Time.deltaTime;
        cutItem.tag = "Untagged";

        if (hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value > .99f)
        {
            hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
            hit.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            GameObject cutFlower = Instantiate(cutFlowerPrefab, cutItem.transform.position, Quaternion.identity);
            cutFlower.transform.parent = cutItem.transform.parent;
            NetworkServer.Spawn(cutFlower);
            Destroy(cutItem.gameObject);
        }
    }

    [Client]
    private void ClntItemCut(Transform cutItem)
    {
        hit.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
        hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value += 1 * Time.deltaTime;
        cutItem.tag = "Untagged";

        if (hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value > .99f)
        {
            hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
            hit.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            GameObject cutFlower = Instantiate(cutFlowerPrefab, cutItem.transform.position, Quaternion.identity);
            cutFlower.transform.parent = cutItem.transform.parent;
            NetworkServer.Spawn(cutFlower);
            Destroy(cutItem.gameObject);
        }
    }

    [Command]
    private void CmdDeliver()
    {
        holding = false;
        Destroy(heldItem);
    }

    [Client]
    private void ClntDeliver()
    {
        holding = false;
        Destroy(heldItem);
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.DrawRay(transform.position, transform.forward, Color.green, 5);
            if (holding)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
                {
                    if (hit.transform.tag == "ItemPlace" && hit.collider.transform.childCount == 0 || (hit.collider.transform.childCount == 1 && hit.collider.transform.gameObject.name == "CuttingBoard"))
                    {
                        CmdItemPlace(hit.transform.gameObject);
                        ClntItemPlace(hit.transform.gameObject);
                        holding = false;
                    }
                    else if (hit.transform.tag == "Delivery") //&& heldItem.GetComponent<Pickups>().plated)
                    {
                        CmdDeliver();
                        ClntDeliver();
                    }
                }
                else
                {
                    CmdItemDrop();
                    ClntItemDrop();
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
                        CmdItemGrab(heldItem.name);
                        ClntItemGrab();
                        holding = true;
                    }
                }
            }
        }
        else if (Input.GetKey(KeyCode.E))
        {
            colliders = Physics.OverlapBox(transform.position, new Vector3(0, 0, 0), Quaternion.identity);

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
            {
                if (hit.transform.gameObject.tag == "ItemPlace" && hit.transform.gameObject.name == "CuttingBoard" && hit.collider.transform.childCount > 0)
                {
                    if (hit.transform.GetChild(1).GetComponent<Pickups>().cuttable == true)
                    {
                        CmdItemCut(hit.transform.GetChild(1));
                        ClntItemCut(hit.transform.GetChild(1));
                    }
                }
            }
        }
    }
}

/*
 * Issues:
 * Client can't perform any interaction command functions, but the movement command function runs perfectly.
 * Flowers on tables aren't getting properly sent to client, they aren't children of tables
 * Flowers don't spawn if they've already spawned in previous hosting without restarting program
 */
