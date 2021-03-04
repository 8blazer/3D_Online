using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class CustomerOrder : NetworkBehaviour
{
    public int currentOrders = 0;
    float timer = 0;
    List<string> orderList = new List<string>();
    public GameObject netManager;
    public Canvas orderCanvas;
    public Image orderBackground;
    public Image redPollen;
    public Image bluePollen;
    public Image yellowPollen;
    public Image lettuce;
    
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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        orderList.Add("RedSmoothie");
        orderList.Add("BlueSmoothie");
        orderList.Add("YellowSmoothie");
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        timer += Time.deltaTime;
        if (((currentOrders == 0 && timer > 5) || timer > 10) && currentOrders < 5)
        {
            currentOrders++;
            timer = 0;
            string order = orderList[Random.Range(0, orderList.Count)];
            AddOrder(order);
            netManager.GetComponent<NetSync>().RefreshHeirarachy();
        }
    }
}
