using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlowerGrowing : NetworkBehaviour
{
    [SerializeField] private float growthCheckTimer = .05f; //bigger = slower
    //[SerializeField] private float upwardsGrowth = .01f; // rate that plant moves upwards

    [SerializeField] private float seedMaxScale;
    [SerializeField] private float sproutMaxScale;
    [SerializeField] private float flowerMaxScale;

    [SerializeField] private GameObject seedObject;
    [SerializeField] private GameObject sproutObject;
    [SerializeField] private GameObject flowerObject;

    

    private int growthStage = 0; //0 seed 1 sprout 2 flower
    public bool isGrown;
    private float timer;

    private Vector3 sproutStartScale;
    private Vector3 flowerStartScale;

    [ServerCallback]
    private void Awake()
    {
        sproutStartScale = sproutObject.transform.localScale;
        flowerStartScale = flowerObject.transform.localScale;
        flowerObject.transform.localScale = Vector3.zero;
        sproutObject.transform.localScale = Vector3.zero;
    }
    [ServerCallback]
    void Start()
    {
        gameObject.tag = "Untagged";
    }
    [ServerCallback]
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (!isGrown)
        {
            switch (growthStage)
            {
                case 0:
                    if (seedObject.transform.localScale.x >= seedMaxScale)
                    {
                        seedObject.transform.localScale.Set(0, 0, 0);
                        growthStage = 1;
                        sproutObject.transform.localScale = sproutStartScale;
                    }
                    else if (timer > growthCheckTimer)
                    {
                        Grow(seedObject);
                        timer = 0;
                    }
                    break;
                case 1:
                    if (sproutObject.transform.localScale.x >= sproutMaxScale)
                    {
                        sproutObject.transform.localScale.Set(0, 0, 0);
                        growthStage = 2;
                        flowerObject.transform.localScale = flowerStartScale;
                    }
                    else if (timer > growthCheckTimer)
                    {
                        Grow(sproutObject);
                        timer = 0;
                    }
                    break;
                case 2:
                    if (flowerObject.transform.localScale.x >= flowerMaxScale)
                    {
                        isGrown = true;
                        gameObject.tag = "Pickup";
                    }
                    else if (timer > growthCheckTimer)
                    {
                        Grow(flowerObject);
                        timer = 0;
                    }
                    break;
            }
        }
    }
    [Server]
    private void Grow(GameObject growingObject)
    {
        //Vector3 transfer1 = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + upwardsGrowth, gameObject.transform.position.z);
        //gameObject.transform.position = transfer1;
        Vector3 transfer = new Vector3(.05f, .05f, .05f);
        growingObject.transform.localScale += transfer;
    }
}
