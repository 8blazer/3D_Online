using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CupTable : NetworkBehaviour
{
    public GameObject cupPrefab;
    GameObject netManager;

    [Command]
    private void CmdCupSpawn()
    {
        Debug.Log("spa");
        GameObject cup = Instantiate(cupPrefab, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
        cup.transform.parent = transform;
        NetworkServer.Spawn(cup);
        CRpcSync();
    }

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
            GameObject cup = Instantiate(cupPrefab, transform.position + new Vector3(0, .24f, 0), Quaternion.Euler(new Vector3(-90,0,0)));
            cup.transform.parent = transform;
            NetworkServer.Spawn(cup);
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
