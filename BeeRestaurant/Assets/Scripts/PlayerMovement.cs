using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    [Command]
    private void CmdMove(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity * speed;
        if (velocity.x == 0 && velocity.z == 0) { return; }
        transform.rotation = Quaternion.LookRotation(velocity);
    }
    
    //[ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 velocity = new Vector3(x, 0, z);

        if (x == 0 && z == 0) { return; }
        CmdMove(velocity);
    }
}
