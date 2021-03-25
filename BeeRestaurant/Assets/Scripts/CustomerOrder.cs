using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class CustomerOrder : NetworkBehaviour
{
    public int currentOrdersNumber = 0;
    float timer = 0;
    List<string> possibleOrderList = new List<string>();
    public List<string> currentOrders = new List<string>();
    private GameObject netManager;
    public Canvas orderCanvas;
    public Image orderBackground;
    public Image pollen;
    public Color red;
    public Color blue;
    public Color yellow;
    public Color green;
    public Color orange;
    public Color purple;
    public Image bee;
    
    [Server]
    public void AddOrder(string netOrder)
    {
        Image orderback = Instantiate(orderBackground, new Vector3(1085, 338, 0), Quaternion.Euler(0,0,90));
        orderback.rectTransform.SetParent(orderCanvas.transform, false);
        NetworkServer.Spawn(orderback.gameObject);
        
        Image cup = Instantiate(pollen, new Vector3(-10, 5, 0), Quaternion.Euler(0, 0, -90));
        cup.rectTransform.SetParent(orderback.transform, false);
        if (netOrder == "RedSmoothie")
        {
            cup.color = red;
            currentOrders.Add("Red");
        }
        else if (netOrder == "BlueSmoothie")
        {
            cup.color = blue;
            currentOrders.Add("Blue");
        }
        else if (netOrder == "YellowSmoothie")
        {
            cup.color = yellow;
            currentOrders.Add("Yellow");
        }
        else if (netOrder == "OrangeSmoothie")
        {
            cup.color = orange;
            currentOrders.Add("Orange");
        }
        else if (netOrder == "GreenSmoothie")
        {
            cup.color = green;
            currentOrders.Add("Green");
        }
        else
        {
            cup.color = purple;
            currentOrders.Add("Purple");
        }


        NetworkServer.Spawn(cup.gameObject);
        Image happyBee = Instantiate(bee, new Vector3(50, 0, 0), Quaternion.Euler(0, 0, -90));
        happyBee.rectTransform.SetParent(orderback.transform, false);
        happyBee.transform.localScale = new Vector3(.7f, .7f, 1);
        NetworkServer.Spawn(happyBee.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        possibleOrderList.Add("RedSmoothie");
        possibleOrderList.Add("BlueSmoothie");
        possibleOrderList.Add("YellowSmoothie");
        possibleOrderList.Add("OrangeSmoothie");
        possibleOrderList.Add("GreenSmoothie");
        possibleOrderList.Add("PurpleSmoothie");
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        if (currentOrdersNumber < 3)
        {
            timer += Time.deltaTime;
        }
        if ((currentOrdersNumber == 0 && timer > 5) || (timer > 5 && currentOrdersNumber < 3))
        {
            currentOrdersNumber++;
            timer = 0;
            string order = possibleOrderList[Random.Range(0, possibleOrderList.Count)];
            AddOrder(order);
            CRpcSyncNames();
        }
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
}
