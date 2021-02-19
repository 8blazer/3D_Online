using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pickups : NetworkBehaviour
{
    public bool held = false;
    public bool counter = false;
    public Transform holdPlayer = null;

    // Update is called once per frame
    void Update()
    {
        if (holdPlayer != null)
        {
            transform.position = holdPlayer.position;
            transform.LookAt(holdPlayer);
        }
    }
}
