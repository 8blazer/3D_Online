using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUI : NetworkBehaviour
{
    public GameObject netManager;
    public Button startButton;

    private void Start()
    {
        netManager = GameObject.Find("NetManager 1");
    }
    private void Update()
    {
        if (netManager.GetComponent<NetManager>().connections > 0 && NetworkServer.active && NetworkClient.isConnected)
        {
            startButton.interactable = true;
        }
    }
    public void StartGame()
    {
        netManager.GetComponent<NetManager>().isGameStarted = true;
        NetworkManager.singleton.ServerChangeScene("GameScene");
    }
    public void ExitLobby()
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
