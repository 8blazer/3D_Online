using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlowerSpawning : NetworkBehaviour
{
    float timer = 0;
    int flowerNumber = 0;
    public GameObject flowerPrefab;

    [ClientRpc]
    private void SyncNames()
    {
        GameObject netManager = GameObject.Find("NetManager");
        if (netManager == null)
        {
            netManager = GameObject.Find("NetManager 1");
        }
        netManager.GetComponent<NetSync>().RefreshHeirarachy();
    }

    [ServerCallback]
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 && flowerNumber < 5)
        {
            GameObject flower = Instantiate(flowerPrefab, new Vector3(Random.Range(-3.5f, 4.5f), .75f, Random.Range(-1f, -3f)), Quaternion.identity);
            NetworkServer.Spawn(flower);
            flower.gameObject.GetComponent<Pickups>().nameInt = flowerNumber;
            timer = 0;
            flowerNumber++;
            SyncNames();
        }
    }

    [ServerCallback]
    private void OnServerInitialized()
    {
        flowerNumber = 0;
        timer = 0;
    }
}
