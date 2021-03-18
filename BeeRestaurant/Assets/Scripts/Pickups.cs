using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pickups : NetworkBehaviour
{
    public bool held = false;
    public bool counter = false;
    public bool blendable = false;
    public float placedHeight;
    public Transform holdPlayer = null;
    public bool cuttable = true;
    public bool plated = false;
    public int nameInt;

    // Update is called once per frame
    void Update()
    {
        if (holdPlayer != null)
        {
            transform.position = holdPlayer.position;
            transform.LookAt(holdPlayer.parent);
            if (gameObject.name.Split(' ')[0] == "Cup")
            {
                transform.eulerAngles = new Vector3(-90, 0, 0);
            }
        }
    }

    public void CupFill(string smoothie)
    {

    }
}
