using System.Collections;
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
    RaycastHit hit;
    float sliderBefore;
    float sliderAfter;
    private GameObject pauseMenu;
    private GameObject GM;


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
        GM = GameObject.FindGameObjectWithTag("GameController");
        GM.GetComponent<GameManager>().points += 1;
        bool orderFound = false;
        int i = 0;
        if (item.Contains("Red") && item.Contains("cup") && GM.GetComponent<CustomerOrder>().currentOrders.Contains("Red"))
        {
            while (!orderFound)
            {
                if (GM.GetComponent<CustomerOrder>().currentOrders[i] == "Red")
                {
                    Debug.Log("r");
                    GM.GetComponent<CustomerOrder>().currentOrders.RemoveAt(i);
                    GM.GetComponent<CustomerOrder>().currentOrdersNumber--;
                    orderFound = true;
                    GameObject bee = GameObject.Find("OrderBackground " + i + 1).transform.GetChild(1).gameObject;
                    GM.GetComponent<GameManager>().points += bee.GetComponent<CustomerPatience>().points;
                    Destroy(GameObject.Find("OrderBackground " + i + 1));
                }
                else
                {
                    i++;
                }
            }
        }
        else if (item.Contains("Blue") && item.Contains("cup") && GM.GetComponent<CustomerOrder>().currentOrders.Contains("Blue"))
        {
            while (!orderFound)
            {
                if (GM.GetComponent<CustomerOrder>().currentOrders[i] == "Blue")
                {
                    Debug.Log("b");
                    GM.GetComponent<CustomerOrder>().currentOrders.RemoveAt(i);
                    GM.GetComponent<CustomerOrder>().currentOrdersNumber--;
                    orderFound = true;
                    GameObject bee = GameObject.Find("OrderBackground " + i + 1).transform.GetChild(1).gameObject;
                    GM.GetComponent<GameManager>().points += bee.GetComponent<CustomerPatience>().points;
                    Destroy(GameObject.Find("OrderBackground " + i + 1));
                }
                else
                {
                    i++;
                }
            }
        }
        else if (item.Contains("Yellow") && item.Contains("cup") && GM.GetComponent<CustomerOrder>().currentOrders.Contains("Yellow"))
        {
            while (!orderFound)
            {
                if (GM.GetComponent<CustomerOrder>().currentOrders[i] == "Yellow")
                {
                    Debug.Log("y");
                    GM.GetComponent<CustomerOrder>().currentOrders.RemoveAt(i);
                    GM.GetComponent<CustomerOrder>().currentOrdersNumber--;
                    orderFound = true;
                    GameObject bee = GameObject.Find("OrderBackground " + i + 1).transform.GetChild(1).gameObject;
                    GM.GetComponent<GameManager>().points += bee.GetComponent<CustomerPatience>().points;
                    Destroy(GameObject.Find("OrderBackground " + i + 1));
                }
                else
                {
                    i++;
                }
            }
        }
        else if (item.Contains("Orange") && item.Contains("cup") && GM.GetComponent<CustomerOrder>().currentOrders.Contains("Orange"))
        {
            while (!orderFound)
            {
                if (GM.GetComponent<CustomerOrder>().currentOrders[i] == "Orange")
                {
                    Debug.Log("o");
                    GM.GetComponent<CustomerOrder>().currentOrders.RemoveAt(i);
                    GM.GetComponent<CustomerOrder>().currentOrdersNumber--;
                    orderFound = true;
                    GameObject bee = GameObject.Find("OrderBackground " + i + 1).transform.GetChild(1).gameObject;
                    GM.GetComponent<GameManager>().points += bee.GetComponent<CustomerPatience>().points * 2;
                    Destroy(GameObject.Find("OrderBackground " + i + 1));
                }
                else
                {
                    i++;
                }
            }
        }
        else if (item.Contains("Green") && item.Contains("cup") && GM.GetComponent<CustomerOrder>().currentOrders.Contains("Green"))
        {
            while (!orderFound)
            {
                if (GM.GetComponent<CustomerOrder>().currentOrders[i] == "Green")
                {
                    Debug.Log("g");
                    GM.GetComponent<CustomerOrder>().currentOrders.RemoveAt(i);
                    GM.GetComponent<CustomerOrder>().currentOrdersNumber--;
                    orderFound = true;
                    GameObject bee = GameObject.Find("OrderBackground " + i + 1).transform.GetChild(1).gameObject;
                    GM.GetComponent<GameManager>().points += bee.GetComponent<CustomerPatience>().points * 2;
                    Destroy(GameObject.Find("OrderBackground " + i + 1));
                }
                else
                {
                    i++;
                }
            }
        }
        else if (item.Contains("Purple") && item.Contains("cup") && GM.GetComponent<CustomerOrder>().currentOrders.Contains("Purple"))
        {
            while (!orderFound)
            {
                if (GM.GetComponent<CustomerOrder>().currentOrders[i] == "Purple")
                {
                    Debug.Log("p");
                    GM.GetComponent<CustomerOrder>().currentOrders.RemoveAt(i);
                    GM.GetComponent<CustomerOrder>().currentOrdersNumber--;
                    orderFound = true;
                    GameObject bee = GameObject.Find("OrderBackground " + i + 1).transform.GetChild(1).gameObject;
                    GM.GetComponent<GameManager>().points += bee.GetComponent<CustomerPatience>().points * 2;
                    Destroy(GameObject.Find("OrderBackground " + i + 1));
                }
                else
                {
                    i++;
                }
            }
        }
        NetworkServer.Destroy(deliveredItem);
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
    private void CmdCupFill(string item, string blenderName, string playerName)
    {
        GameObject cup = GameObject.Find(item);
        GameObject blender = GameObject.Find(blenderName);
        GameObject player = GameObject.Find(playerName);
        if (blender.GetComponent<Blender>().completedDish == "red")
        {
            GameObject newCup = Instantiate(redSmoothiePrefab, cup.transform.position, Quaternion.identity);
            newCup.GetComponent<Pickups>().holdPlayer = cup.GetComponent<Pickups>().holdPlayer;
            player.GetComponent<PlayerInteract>().heldItem = newCup;
            newCup.name = "RedCup";
            NetworkServer.Spawn(newCup);
        }
        else if (blender.GetComponent<Blender>().completedDish == "blue")
        {
            GameObject newCup = Instantiate(blueSmoothiePrefab, cup.transform.position, Quaternion.identity);
            newCup.GetComponent<Pickups>().holdPlayer = cup.GetComponent<Pickups>().holdPlayer;
            newCup.name = "BlueCup";
            player.GetComponent<PlayerInteract>().heldItem = newCup;
            NetworkServer.Spawn(newCup);
        }
        else if (blender.GetComponent<Blender>().completedDish == "yellow")
        {
            GameObject newCup = Instantiate(yellowSmoothiePrefab, cup.transform.position, Quaternion.identity);
            newCup.GetComponent<Pickups>().holdPlayer = cup.GetComponent<Pickups>().holdPlayer;
            newCup.name = "YellowCup";
            player.GetComponent<PlayerInteract>().heldItem = newCup;
            NetworkServer.Spawn(newCup);
        }
        else if (blender.GetComponent<Blender>().completedDish == "green")
        {
            GameObject newCup = Instantiate(greenSmoothiePrefab, cup.transform.position, Quaternion.identity);
            newCup.GetComponent<Pickups>().holdPlayer = cup.GetComponent<Pickups>().holdPlayer;
            newCup.name = "GreenCup";
            player.GetComponent<PlayerInteract>().heldItem = newCup;
            NetworkServer.Spawn(newCup);
        }
        else if (blender.GetComponent<Blender>().completedDish == "orange")
        {
            GameObject newCup = Instantiate(orangeSmoothiePrefab, cup.transform.position, Quaternion.identity);
            newCup.GetComponent<Pickups>().holdPlayer = cup.GetComponent<Pickups>().holdPlayer;
            newCup.name = "OrangeCup";
            player.GetComponent<PlayerInteract>().heldItem = newCup;
            NetworkServer.Spawn(newCup);
        }
        else if (blender.GetComponent<Blender>().completedDish == "purple")
        {
            GameObject newCup = Instantiate(purpleSmoothiePrefab, cup.transform.position, Quaternion.identity);
            newCup.GetComponent<Pickups>().holdPlayer = cup.GetComponent<Pickups>().holdPlayer;
            newCup.name = "PurpleCup";
            player.GetComponent<PlayerInteract>().heldItem = newCup;
            NetworkServer.Spawn(newCup);
        }
        NetworkServer.Destroy(cup);
        CRpcSyncNames();
        CRpcCupFill(blender.name);
    }

    [ClientRpc]
    private void CRpcCupFill(string blenderName)
    {
        GameObject blender = GameObject.Find(blenderName);
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
                        CmdCupFill(heldItem.name, hit.transform.name, gameObject.name);
                        heldItem.GetComponent<Pickups>().CupFill(hit.transform.GetComponent<Blender>().completedDish);
                    }
                    else if (hit.transform.tag == "ItemPlace" && hit.transform.childCount == 0 || (hit.transform.childCount == 1 && hit.transform.gameObject.name.Split(' ')[0] == "CuttingBoard"))
                    {
                        CmdItemPlace(hit.transform.gameObject.name, heldItem.name);
                        holding = false;
                    }
                    else if (hit.transform.tag == "Delivery" && heldItem.GetComponent<Pickups>().plated)
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
                holding = false;
                heldItem = null;
            }
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1, dropLayerMask))
            {
                if (hit.transform.gameObject.tag == "ItemPlace" && hit.transform.gameObject.name.Split(' ')[0] == "CuttingBoard" && hit.transform.childCount > 1)
                {
                    if (hit.transform.GetChild(1).GetComponent<Pickups>().cuttable == true)
                    {
                        CmdItemCut(hit.transform.name, hit.transform.GetChild(1).name, Time.deltaTime);
                    }
                }
            }
        }
    }
}
