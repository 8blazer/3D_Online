using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Seed : NetworkBehaviour
{
    [SerializeField] private GameObject flowerType;
    [Command]
    public void CmdPlantFlower()
    {
        SVRPlantFlower();
    }
    [Server]
    private void SVRPlantFlower()
    {
        GameObject newflower = Instantiate(flowerType, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        NetworkServer.Spawn(newflower);
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
