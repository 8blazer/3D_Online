﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    private GameObject pauseMenu;

    [Command]
    private void CmdMove(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity * speed;
        if (velocity.x != 0 || velocity.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

    [Client]
    private void ClntMove(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity * speed;
        if (velocity.x != 0 || velocity.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
    
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            pauseMenu = GameObject.Find("PauseMenu");
            if (pauseMenu == null)
            {
                pauseMenu = GameObject.Find("PauseMenu 1");
            }
            if (pauseMenu.GetComponent<Canvas>().enabled)
            {
                return;
            }
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 velocity = new Vector3(x, 0, z);

        if (velocity.x == 0 && velocity.z == 0) { return; }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            CmdMove(velocity);
            ClntMove(velocity);
        }
        else
        {
            CmdMove(new Vector3(0, 0, 0));
            ClntMove(new Vector3(0, 0, 0));
        }
    }
}
