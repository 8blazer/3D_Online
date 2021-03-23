using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderMovement : MonoBehaviour
{
    int orderNumber;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.Find("GameManager 1");
        orderNumber = gameManager.GetComponent<CustomerOrder>().currentOrdersNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if (orderNumber == 1 && transform.position.x > 150)
        {
            transform.position -= new Vector3(4, 0, 0);
        }
        else if (orderNumber == 2 && transform.position.x > 450)
        {
            transform.position -= new Vector3(4, 0, 0);
        }
        else if (orderNumber == 3 && transform.position.x > 750)
        {
            transform.position -= new Vector3(4, 0, 0);
        }
        else if (orderNumber == 4 && transform.position.x > 1050)
        {
            transform.position -= new Vector3(4, 0, 0);
        }
        else if (orderNumber == 5 && transform.position.x > 1350)
        {
            transform.position -= new Vector3(4, 0, 0);
        }
    }
}
