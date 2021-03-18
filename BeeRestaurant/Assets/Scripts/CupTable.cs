using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CupTable : NetworkBehaviour
{
    public GameObject cupPrefab;
    public GameObject redSeedPrefab;
    public GameObject blueSeedPrefab;
    public GameObject yellowSeedPrefab;
    GameObject netManager;

    [ClientRpc]
    private void CRpcSync()
    {
        GameObject netManager = GameObject.Find("NetManager");
        if (netManager == null)
        {
            netManager = GameObject.Find("NetManager 1");
        }
        netManager.GetComponent<NetSync>().RefreshHeirarachy();
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkServer.active) { return; }

        if (transform.childCount < 1)
        {
            if (gameObject.name.Split(' ')[0] == "CupTable")
            {
                GameObject cup = Instantiate(cupPrefab, transform.position + new Vector3(0, .24f, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
                cup.transform.parent = transform;
                cup.name = "Cup";
                NetworkServer.Spawn(cup);
            }
            else if (gameObject.name.Split(' ')[0] == "RedSeedTable")
            {
                GameObject redSeed = Instantiate(redSeedPrefab, transform.position + new Vector3(0, .24f, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
                redSeed.transform.parent = transform;
                redSeed.name = "RedSeed";
                NetworkServer.Spawn(redSeed);
            }
            else if (gameObject.name.Split(' ')[0] == "BlueSeedTable")
            {
                GameObject blueSeed = Instantiate(blueSeedPrefab, transform.position + new Vector3(0, .24f, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
                blueSeed.transform.parent = transform;
                blueSeed.name = "BlueSeed";
                NetworkServer.Spawn(blueSeed);
            }
            else
            {
                GameObject yellowSeed = Instantiate(yellowSeedPrefab, transform.position + new Vector3(0, .24f, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
                yellowSeed.transform.parent = transform;
                yellowSeed.name = "YellowSeed";
                NetworkServer.Spawn(yellowSeed);
            }
            GameObject netManager = GameObject.Find("NetManager");
            if (netManager == null)
            {
                netManager = GameObject.Find("NetManager 1");
            }
            netManager.GetComponent<NetSync>().RefreshHeirarachy();
            CRpcSync();
        }
    }
}
