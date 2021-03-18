﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInteract : NetworkBehaviour
{
    Collider[] colliders;
    public bool holding = false;
    public LayerMask grabLayerMask;
    public LayerMask dropLayerMask;
    public GameObject heldItem = null;
    public GameObject redPollenPrefab;
    public GameObject bluePollenPrefab;
    public GameObject yellowPollenPrefab;
    public GameObject redSmoothiePrefab;
    public GameObject blueSmoothiePrefab;
    public GameObject yellowSmoothiePrefab;
    public GameObject orangeSmoothiePrefab;
    public GameObject greenSmoothiePrefab;
    public GameObject purpleSmoothiePrefab;
    public Color red;
    public Color blue;
    public Color yellow;
    public Color orange;
    public Color green;
    public Color purple;
    RaycastHit hit;
    float sliderBefore;
    float sliderAfter;
    private GameObject pauseMenu;

    [Command]
    private void CmdItemPlace(string table, string item)
    {
        GameObject counter = GameObject.Find(table);
        GameObject placedItem = GameObject.Find(item);
        placedItem.transform.position = counter.transform.position + new Vector3(0, placedItem.GetComponent<Pickups>().placedHeight, 0);
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

    [Command]
    private void CmdItemDrop(string item)
    {
        GameObject droppedItem = GameObject.Find(item);
        droppedItem.GetComponent<Pickups>().held = false;
        droppedItem.GetComponent<Pickups>().holdPlayer = null;
        CRpcItemDrop(droppedItem.name);
    }

    [ClientRpc]
    private void CRpcItemDrop(string item)
    {
        GameObject droppedItem = GameObject.Find(item);
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
        CRpcItemGrab(grabbedItem.name, grabPlayer.name);
    }

    [ClientRpc]
    private void CRpcItemGrab(string item, string player)
    {
        GameObject grabbedItem = GameObject.Find(item);
        GameObject grabPlayer = GameObject.Find(player);
        grabbedItem.GetComponent<Pickups>().holdPlayer = grabPlayer.transform;
        grabbedItem.transform.parent = null;
    }

    [Command]
    private void CmdItemCut(string table, string item, float time)
    {
        GameObject counter = GameObject.Find(table);
        GameObject cutItem = GameObject.Find(item);
        if (!cutItem.GetComponent<Pickups>().cuttable) { return; }
        counter.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
        sliderBefore = counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value;
        counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value += 1f * time;
        sliderAfter = counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value - sliderBefore;
        cutItem.tag = "Untagged";

        if (counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value == 1)
        {
            counter.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
            counter.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            if (cutItem.name.Split(' ')[0] == "RedFlower")
            {
                GameObject cutFlower = Instantiate(redPollenPrefab, cutItem.transform.position, Quaternion.identity);
                cutFlower.transform.position = new Vector3(cutFlower.transform.position.x, cutFlower.GetComponent<Pickups>().placedHeight + .7f, cutFlower.transform.position.z);
                cutFlower.transform.parent = cutItem.transform.parent;
                NetworkServer.Spawn(cutFlower);
                NetworkServer.Destroy(cutItem);
                CmdSyncNames();
                CRpcSyncNames();
                CRpcItemCut(counter.name, cutFlower.name, sliderAfter);
            }
            else if (cutItem.name.Split(' ')[0] == "BlueFlower")
            {
                GameObject cutFlower = Instantiate(bluePollenPrefab, cutItem.transform.position, Quaternion.identity);
                cutFlower.transform.position = new Vector3(cutFlower.transform.position.x, cutFlower.GetComponent<Pickups>().placedHeight + .7f, cutFlower.transform.position.z);
                cutFlower.transform.parent = cutItem.transform.parent;
                NetworkServer.Spawn(cutFlower);
                NetworkServer.Destroy(cutItem);
                CmdSyncNames();
                CRpcSyncNames();
                CRpcItemCut(counter.name, cutFlower.name, sliderAfter);
            }
            else
            {
                GameObject cutFlower = Instantiate(yellowPollenPrefab, cutItem.transform.position, Quaternion.identity);
                cutFlower.transform.position = new Vector3(cutFlower.transform.position.x, cutFlower.GetComponent<Pickups>().placedHeight + .7f, cutFlower.transform.position.z);
                cutFlower.transform.parent = cutItem.transform.parent;
                NetworkServer.Spawn(cutFlower);
                NetworkServer.Destroy(cutItem);
                CmdSyncNames();
                CRpcSyncNames();
                CRpcItemCut(counter.name, cutFlower.name, sliderAfter);
            }
        }
        else
        {
            CRpcItemCut(counter.name, cutItem.name, sliderAfter);
        }
    }

    [ClientRpc]
    private void CRpcItemCut(string table, string item, float sliderChange)
    {
        if (NetworkServer.active && NetworkClient.isConnected) { return; }
        GameObject counter = GameObject.Find(table);
        GameObject cutItem = GameObject.Find(item);
        if (cutItem.gameObject.name.Split(' ')[0] == "RedFlower" || cutItem.gameObject.name.Split(' ')[0] == "BlueFlower" || cutItem.gameObject.name.Split(' ')[0] == "YellowFlower")
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
            GameObject netManager = GameObject.Find("NetManager");
            if (netManager == null)
            {
                netManager = GameObject.Find("NetManager 1");
            }
            netManager.GetComponent<NetSync>().RefreshHeirarachy();
        }
    }

    [Command]
    private void CmdSyncNames()
    {
        GameObject netManager = GameObject.Find("NetManager");
        if (netManager == null)
        {
            netManager = GameObject.Find("NetManager 1");
        }
        netManager.GetComponent<NetSync>().RefreshHeirarachy();
    }

    [ClientRpc]
    private void CRpcSyncNames()
    {
        GameObject netManager = GameObject.Find("NetManager");
        if (netManager == null)
        {
            netManager = GameObject.Find("NetManager 1");
        }
        netManager.GetComponent<NetSync>().RefreshHeirarachy();
    }

    [Command]
    private void CmdDeliver(string item)
    {
        GameObject deliveredItem = GameObject.Find(item);
        CRpcDeliver(deliveredItem.name);
        Destroy(deliveredItem);
    }

    [ClientRpc]
    private void CRpcDeliver(string item)
    {
        GameObject deliveredItem = GameObject.Find(item);
        Destroy(deliveredItem);
    }

    [Command]
    private void CmdBlend(string blenderName, string itemName)
    {
        GameObject blender = GameObject.Find(blenderName);
        GameObject item = GameObject.Find(itemName);
        CRpcBlend(blender.name, item.name);
        if (item.name.Split(' ')[0] == "RedPollen")
        {
            blender.GetComponent<Blender>().AddItem("red");
        }
        else if (item.name.Split(' ')[0] == "BluePollen")
        {
            blender.GetComponent<Blender>().AddItem("blue");
        }
        else
        {
            blender.GetComponent<Blender>().AddItem("yellow");
        }
        Destroy(item);
    }

    [ClientRpc]
    private void CRpcBlend(string blenderName, string itemName)
    {
        if (NetworkServer.active && NetworkClient.isConnected) { return; }
        GameObject blender = GameObject.Find(blenderName);
        GameObject item = GameObject.Find(itemName);
        if (item.name.Split(' ')[0] == "RedPollen")
        {
            blender.GetComponent<Blender>().AddItem("red");
        }
        else if (item.name.Split(' ')[0] == "BluePollen")
        {
            blender.GetComponent<Blender>().AddItem("blue");
        }
        else
        {
            blender.GetComponent<Blender>().AddItem("yellow");
        }
        Destroy(item);
    }

    [Command]
    private void CmdCupFill(string item, string blenderName)
    {
        GameObject cup = GameObject.Find(item);
        GameObject blender = GameObject.Find(blenderName);
        CRpcCupFill(cup.name, blender.name);
    }

    [ClientRpc]
    private void CRpcCupFill(string item, string blenderName)
    {
        GameObject cup = GameObject.Find(item);
        GameObject blender = GameObject.Find(blenderName);
        if (blender.GetComponent<Blender>().completedDish == "red")
        {
            cup.GetComponent<MeshRenderer>().material.color = red;
        }
        else if (blender.GetComponent<Blender>().completedDish == "blue")
        {
            cup.GetComponent<MeshRenderer>().material.color = blue;
        }
        else if (blender.GetComponent<Blender>().completedDish == "yellow")
        {
            cup.GetComponent<MeshRenderer>().material.color = yellow;
        }
        else if (blender.GetComponent<Blender>().completedDish == "orange")
        {
            cup.GetComponent<MeshRenderer>().material.color = orange;
        }
        else if (blender.GetComponent<Blender>().completedDish == "green")
        {
            cup.GetComponent<MeshRenderer>().material.color = green;
        }
        else
        {
            cup.GetComponent<MeshRenderer>().material.color = purple;
        }
        blender.GetComponent<Blender>().completedDish = "";
        blender.GetComponent<Blender>().itemNumber = 0;
        blender.GetComponent<Blender>().ingredients.Clear();
        blender.GetComponent<Blender>().blenderSlider.value = 0;
        blender.GetComponent<Blender>().blenderCanvas.enabled = false;
    }

    [Command]
    private void CmdPlant(string seedName)
    {
        GameObject seed = GameObject.Find(seedName);
        GameObject newflower = Instantiate(seed.GetComponent<Seed>().flowerType, new Vector3(seed.transform.position.x, 1, seed.transform.position.z), Quaternion.identity);
        NetworkServer.Spawn(newflower);
        NetworkServer.Destroy(seed);
        Destroy(seed);
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            pauseMenu = GameObject.Find("PauseMenu");
            if (pauseMenu == null)
            {
                pauseMenu = GameObject.Find("PauseMenu 1");
            }
            if (pauseMenu.GetComponent<Canvas>().enabled)
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (holding)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
                {
                    if (hit.transform.gameObject.name.Split(' ')[0] == "BlenderTable" && hit.transform.gameObject.GetComponent<Blender>().itemNumber < 4 && heldItem.GetComponent<Pickups>().blendable)
                    {
                        CmdBlend(hit.transform.gameObject.name, heldItem.name);
                        holding = false;
                        heldItem = null;
                    }
                    else if (hit.transform.gameObject.name.Split(' ')[0] == "BlenderTable" && hit.transform.GetComponent<Blender>().completedDish != "" && heldItem.name.Split(' ')[0] == "Cup")
                    {
                        CmdCupFill(heldItem.name, hit.transform.name);
                        heldItem.GetComponent<Pickups>().CupFill(hit.transform.GetComponent<Blender>().completedDish);
                    }
                    else if (hit.transform.tag == "ItemPlace" && hit.transform.childCount == 0 || (hit.transform.childCount == 1 && hit.transform.gameObject.name.Split(' ')[0] == "CuttingBoard"))
                    {
                        CmdItemPlace(hit.transform.gameObject.name, heldItem.name);
                        holding = false;
                    }
                    else if (hit.transform.tag == "Delivery") //&& heldItem.GetComponent<Pickups>().plated)
                    {
                        holding = false;
                        CmdDeliver(heldItem.name);
                    }
                }
                else
                {
                    CmdItemDrop(heldItem.name);
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
                        CmdItemGrab(heldItem.name, this.gameObject.transform.GetChild(0).name);
                        holding = true;
                    }
                }
            }
        }
        else if (Input.GetKey(KeyCode.E))
        {
            colliders = Physics.OverlapBox(transform.position, new Vector3(0, 0, 0), Quaternion.identity);

            if(holding && heldItem.TryGetComponent<Seed>(out Seed seedComponent))
            {
                CmdPlant(heldItem.name);
                //seedComponent.PlantFlower();
                holding = false;
                heldItem = null;
            }
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
            {
                if (hit.transform.gameObject.tag == "ItemPlace" && hit.transform.gameObject.name.Split(' ')[0] == "CuttingBoard" && hit.transform.childCount > 1)
                {
                    if (hit.transform.GetChild(1).GetComponent<Pickups>().cuttable == true) // && !methodRunning)
                    {
                        CmdItemCut(hit.transform.name, hit.transform.GetChild(1).name, Time.deltaTime);
                    }
                }
            }
        }
    }
}

/*
 * Issues:
 * It appears that the info of holding and the pairing the held items to the player kinda stuff isn't getting translated properly again?
 */
