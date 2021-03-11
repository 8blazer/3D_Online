using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PauseMenu : NetworkBehaviour
{
    private GameObject netManager;
    // Start is called before the first frame update
    void Start()
    {
        netManager = GameObject.Find("NetManager 1");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GetComponent<Canvas>().enabled)
            {
                GetComponent<Canvas>().enabled = false;
            }
            else
            {
                GetComponent<Canvas>().enabled = true;
            }
        }
    }

    public void Resume()
    {
        GetComponent<Canvas>().enabled = false;
    }

    public void Settings()
    {

    }

    public void MainMenu()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            netManager.GetComponent<NetManager>().connections = 0;
            NetworkManager.singleton.StopHost();
        }
        else
        {
            netManager.GetComponent<NetManager>().connections--;
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
