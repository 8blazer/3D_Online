﻿using System.Collections;
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
    bool methodRunning = false;
    float sliderBefore;
    float sliderAfter;

    [Command]
    private void CmdItemPlace(string table, string item)
    {
        GameObject counter = GameObject.Find(table);
        GameObject placedItem = GameObject.Find(item);
        placedItem.transform.position = counter.transform.position + new Vector3(0, .5f, 0);
        placedItem.GetComponent<Pickups>().held = false;
        placedItem.GetComponent<Pickups>().holdPlayer = null;
        placedItem.transform.parent = counter.transform;
        CRpcItemPlace(counter.name, placedItem.name);
    }

    [ClientRpc]
    private void CRpcItemPlace(string table, string item)
    {
        GameObject counter = GameObject.Find(table);
        GameObject placedItem = GameObject.Find(item);
        placedItem.transform.parent = counter.transform;
    }

    [Client]
    private void ClntItemPlace(GameObject counter)
    {
        heldItem.transform.position = counter.transform.position + new Vector3(0, .5f, 0);
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
        //heldItem.transform.parent = counter.transform;
    }

    [Command]
    private void CmdItemDrop(string item)
    {
        GameObject droppedItem = GameObject.Find(item);
        droppedItem.GetComponent<Pickups>().held = false;
        droppedItem.GetComponent<Pickups>().holdPlayer = null;
    }

    [Client]
    private void ClntItemDrop()
    {
        heldItem.GetComponent<Pickups>().held = false;
        heldItem.GetComponent<Pickups>().holdPlayer = null;
    }

    
    [Command]
    private void CmdItemGrab(string item, string player)
    {
        GameObject grabbedItem = GameObject.Find(item);
        GameObject grabPlayer = GameObject.Find(player);
        grabbedItem.GetComponent<Pickups>().held = true;
        grabbedItem.GetComponent<Pickups>().holdPlayer = grabPlayer.transform;
        grabbedItem.transform.parent = null;
        CRpcItemGrab(grabbedItem.name);
    }

    [ClientRpc]
    private void CRpcItemGrab(string item)
    {
        GameObject grabbedItem = GameObject.Find(item);
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
    private void CmdItemCut(string table, string item, float time)
    {
        GameObject counter = GameObject.Find(table);
        GameObject cutItem = GameObject.Find(item);
        counter.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
        sliderBefore = counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value;
        counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value += 1f * time;
        sliderAfter = counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value - sliderBefore;
        cutItem.tag = "Untagged";

        if (counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value > .99f)
        {
            counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
            counter.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            GameObject cutFlower = Instantiate(cutFlowerPrefab, cutItem.transform.position, Quaternion.identity);
            cutFlower.transform.parent = cutItem.transform.parent;
            NetworkServer.Spawn(cutFlower);
            Destroy(cutItem.gameObject);
            //Debug.Log(cutFlower.gameObject.name.Split(' ')[0]);
            CRpcItemCut(counter.name, cutFlower.name, sliderAfter);
        }
        else
        {
            CRpcItemCut(counter.name, cutItem.name, sliderAfter);
        }
    }

    [ClientRpc]
    private void CRpcItemCut(string table, string item, float sliderChange)
    {
        GameObject counter = GameObject.Find(table);
        GameObject cutItem = GameObject.Find(item);
        if (cutItem.gameObject.name.Split(' ')[0] == "Flower")
        {
            counter.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
            counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value += sliderChange;
            cutItem.tag = "Untagged";
        }
        else
        {
            counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
            counter.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            cutItem.transform.parent = counter.transform;
        }
        methodRunning = false;
    }

    [Client]
    private void ClntItemCut(Transform cutItem)
    {
        hit.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
        hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value += .01f;
        cutItem.tag = "Untagged";

        if (hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value > .99f)
        {
            Debug.Log("cl");
            hit.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
            hit.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            methodRunning = false;
            //Destroy(cutItem.gameObject);
        }
        else
        {
            methodRunning = false;
        }
    }

    [Command]
    private void CmdDeliver(string item)
    {
        GameObject deliveredItem = GameObject.Find(item);
        Destroy(deliveredItem);
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
                    if (hit.transform.tag == "ItemPlace" && hit.collider.transform.childCount == 0 || (hit.collider.transform.childCount == 1 && hit.collider.transform.gameObject.name.Split(' ')[0] == "CuttingBoard"))
                    {
                        CmdItemPlace(hit.transform.gameObject.name, heldItem.name);
                        //ClntItemPlace(hit.transform.gameObject);
                        holding = false;
                    }
                    else if (hit.transform.tag == "Delivery") //&& heldItem.GetComponent<Pickups>().plated)
                    {
                        holding = false;
                        CmdDeliver(heldItem.name);
                        ClntDeliver();
                    }
                }
                else
                {
                    CmdItemDrop(heldItem.name);
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
                        CmdItemGrab(heldItem.name, this.gameObject.transform.GetChild(1).name);
                        //ClntItemGrab();
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
                if (hit.transform.gameObject.tag == "ItemPlace" && hit.transform.gameObject.name.Split(' ')[0] == "CuttingBoard" && hit.collider.transform.childCount > 1)
                {
                    if (hit.transform.GetChild(1).GetComponent<Pickups>().cuttable == true && !methodRunning)
                    {
                        methodRunning = true;
                        CmdItemCut(hit.transform.name, hit.transform.GetChild(1).name, Time.deltaTime);
                        //ClntItemCut(hit.transform.GetChild(1));
                    }
                }
            }
        }
    }
}

/*
 * Issues:
 * Cutting flowers doesn't really work
 * Flowers don't spawn if they've already spawned in previous hosting without restarting program
 */
