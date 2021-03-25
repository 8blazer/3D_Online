using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlowerGrowing : NetworkBehaviour
{
     private float growthCheckTimer = .05f; //bigger = slower

    [SerializeField] private float seedGrowthRate = .05f;
    [SerializeField] private float sproutGrowthRate = .001f;
    [SerializeField] private float flowerGrowthRate = .0001f;

    [SerializeField] private float sproutStartSize;
    [SerializeField] private float flowerStartSize;
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
    void Start()
    {
        flowerObject.transform.localScale = Vector3.zero;
        sproutObject.transform.localScale = Vector3.zero;
        gameObject.tag = "Untagged";
        sproutStartScale = new Vector3(sproutStartSize, sproutStartSize, sproutStartSize);
        flowerStartScale = new Vector3(flowerStartSize, flowerStartSize, flowerStartSize);
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
                        seedObject.transform.localScale = new Vector3(0, 0, 0);
                        growthStage = 1;
                        sproutObject.transform.localScale = sproutStartScale;
                    }
                    else if (timer > growthCheckTimer)
                    {
                        Vector3 transfer = new Vector3(seedGrowthRate, seedGrowthRate, seedGrowthRate);
                        seedObject.transform.localScale += transfer;
                        timer = 0;
                    }
                    break;
                case 1:
                    if (sproutObject.transform.localScale.x >= sproutMaxScale)
                    {
                        sproutObject.transform.localScale = new Vector3(0, 0, 0);
                        growthStage = 2;
                        flowerObject.transform.localScale = flowerStartScale;
                    }
                    else if (timer > growthCheckTimer)
                    {
                        Vector3 transfer = new Vector3(sproutGrowthRate, sproutGrowthRate, sproutGrowthRate);
                        sproutObject.transform.localScale += transfer;
                        timer = 0;
                    }
                    break;
                case 2:
                    if (flowerObject.transform.localScale.x >= flowerMaxScale)
                    {
                        isGrown = true;
                        GetComponent<BoxCollider>().enabled = true;
                        gameObject.tag = "Pickup";
                    }
                    else if (timer > growthCheckTimer)
                    {
                        Vector3 transfer = new Vector3(flowerGrowthRate, flowerGrowthRate, flowerGrowthRate);
                        flowerObject.transform.localScale += transfer;
                        timer = 0;
                    }
                    break;
            }
        }
    }
}
