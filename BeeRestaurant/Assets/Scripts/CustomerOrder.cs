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
    public GameObject netManager;
    public Canvas orderCanvas;
    public Image orderBackground;
    public Image redPollen;
    public Image bluePollen;
    public Image yellowPollen;
    
    [ClientRpc]
    public void AddOrder(string netOrder)
    {
        Image orderback = Instantiate(orderBackground, new Vector3(1085, 338, 0), Quaternion.identity);
        orderback.rectTransform.SetParent(orderCanvas.transform, false);
        NetworkServer.Spawn(orderback.gameObject);

        if (netOrder == "RedSmoothie")
        {
            Image red = Instantiate(redPollen, new Vector3(-32, -30, 0), Quaternion.identity);
            red.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(red.gameObject);
            red = Instantiate(redPollen, new Vector3(0, -30, 0), Quaternion.identity);
            red.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(red.gameObject);
            red = Instantiate(redPollen, new Vector3(32, -30, 0), Quaternion.identity);
            red.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(red.gameObject);
            currentOrders.Add("Red");
        }
        else if (netOrder == "BlueSmoothie")
        {
            Image blue = Instantiate(bluePollen, new Vector3(-32, -30, 0), Quaternion.identity);
            blue.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(blue.gameObject);
            blue = Instantiate(bluePollen, new Vector3(0, -30, 0), Quaternion.identity);
            blue.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(blue.gameObject);
            blue = Instantiate(bluePollen, new Vector3(32, -30, 0), Quaternion.identity);
            blue.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(blue.gameObject);
            currentOrders.Add("Blue");
        }
        else if (netOrder == "YellowSmoothie")
        {
            Image yellow = Instantiate(yellowPollen, new Vector3(-32, -30, 0), Quaternion.identity);
            yellow.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(yellow.gameObject);
            yellow = Instantiate(yellowPollen, new Vector3(0, -30, 0), Quaternion.identity);
            yellow.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(yellow.gameObject);
            yellow = Instantiate(yellowPollen, new Vector3(32, -30, 0), Quaternion.identity);
            yellow.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(yellow.gameObject);
            currentOrders.Add("Yellow");
        }
        else if (netOrder == "OrangeSmoothie")
        {
            Image yellow = Instantiate(yellowPollen, new Vector3(-40, -30, 0), Quaternion.identity);
            yellow.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(yellow.gameObject);
            yellow = Instantiate(yellowPollen, new Vector3(-10, -30, 0), Quaternion.identity);
            yellow.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(yellow.gameObject);
            Image red = Instantiate(redPollen, new Vector3(10, -30, 0), Quaternion.identity);
            red.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(red.gameObject);
            red = Instantiate(redPollen, new Vector3(40, -30, 0), Quaternion.identity);
            red.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(red.gameObject);
            currentOrders.Add("Orange");
        }
        else if (netOrder == "GreenSmoothie")
        {
            Image yellow = Instantiate(yellowPollen, new Vector3(-40, -30, 0), Quaternion.identity);
            yellow.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(yellow.gameObject);
            yellow = Instantiate(yellowPollen, new Vector3(-10, -30, 0), Quaternion.identity);
            yellow.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(yellow.gameObject);
            Image blue = Instantiate(bluePollen, new Vector3(10, -30, 0), Quaternion.identity);
            blue.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(blue.gameObject);
            blue = Instantiate(bluePollen, new Vector3(40, -30, 0), Quaternion.identity);
            blue.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(blue.gameObject);
            currentOrders.Add("Green");
        }
        else
        {
            Image red = Instantiate(redPollen, new Vector3(-40, -30, 0), Quaternion.identity);
            red.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(red.gameObject);
            red = Instantiate(redPollen, new Vector3(-10, -30, 0), Quaternion.identity);
            red.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(red.gameObject);
            Image blue = Instantiate(bluePollen, new Vector3(10, -30, 0), Quaternion.identity);
            blue.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(blue.gameObject);
            blue = Instantiate(bluePollen, new Vector3(40, -30, 0), Quaternion.identity);
            blue.rectTransform.SetParent(orderback.transform, false);
            NetworkServer.Spawn(blue.gameObject);
            currentOrders.Add("Purple");
        }
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
        timer += Time.deltaTime;
        if (((currentOrdersNumber == 0 && timer > 5))) //|| timer > 10) && currentOrdersNumber < 5)
        {
            currentOrdersNumber++;
            timer = 0;
            string order = possibleOrderList[Random.Range(0, possibleOrderList.Count)];
            AddOrder(order);
            netManager.GetComponent<NetSync>().RefreshHeirarachy();
        }
    }
}
