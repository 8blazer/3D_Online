using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerGrowing : MonoBehaviour
{
    [SerializeField] private float growthCheckTimer = .05f; //bigger = slower
    [SerializeField] private float upwardsGrowth = .01f; // rate that plant moves upwards
    public bool isGrown;
    private float timer;

    void Start()
    {
        gameObject.tag = "Untagged";
    }
    void FixedUpdate()
    {
        if(gameObject.transform.localScale == Vector3.one)
        {
            isGrown = true;
            gameObject.tag = "Pickup";
        }
        timer += Time.deltaTime;
        if (timer > growthCheckTimer && !isGrown)
        {
            timer = 0;
        }
    }
    private void Grow()
    {
        Vector3 transfer1 = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + upwardsGrowth, gameObject.transform.position.z);
        gameObject.transform.position = transfer1;
        Vector3 transfer = new Vector3(.05f, .05f, .05f);
        gameObject.transform.localScale += transfer;
    }
}
