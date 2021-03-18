using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Seed : NetworkBehaviour
{
    public GameObject flowerType;
    [SerializeField] private float spawnDistanceAboveGround;//assuming ground is at 0
    [Command]
    public void CmdPlantFlower()
    {
        SVRPlantFlower();
    }
    [Server]
    private void SVRPlantFlower()
    {
        GameObject newflower = Instantiate(flowerType, new Vector3(transform.position.x, spawnDistanceAboveGround, transform.position.z), Quaternion.identity);
        NetworkServer.Spawn(newflower);
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }

    public void PlantFlower()
    {
        CmdPlantFlower();
    }
}
