using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class MainMenu : MonoBehaviour
{
    public NetworkManager netManager;
    [SerializeField] private Transform CPCameraPosition;
    [SerializeField] private ColorPicker colorPicker;
    public Canvas joinGameCanvas;
    public Button hostButton;
    public Button joinMenuButton;
    public Button joinGameButton;
    public Button customizeButton;
    public Button quitButton;
    public Button settingsButton;
    public Button cancelButton;
    public InputField address;
    public void Settings()
    {

    }
    public void Customize()
    {
        colorPicker.OpenColorPicker();
        Camera.main.transform.position = CPCameraPosition.position;
    }
    public void HostGame()
    {
        NetworkManager.singleton.StartHost();
    }
    public void JoinGameMenu()
    {
        joinGameCanvas.GetComponent<Canvas>().enabled = true;
        
        hostButton.GetComponent<Button>().enabled = false;
        joinMenuButton.GetComponent<Button>().enabled = false;
        customizeButton.GetComponent<Button>().enabled = false;
        quitButton.GetComponent<Button>().enabled = false;
        settingsButton.GetComponent<Button>().enabled = false;

        joinGameButton.GetComponent<Button>().enabled = true;
        cancelButton.GetComponent<Button>().enabled = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void JoinGame()
    {
        if (address.text == "")
        {
            address.text = "localhost";
        }
        NetworkManager.singleton.networkAddress = address.text;
        NetworkManager.singleton.StartClient();
    }
    public void Cancel()
    {
        joinGameCanvas.GetComponent<Canvas>().enabled = false;
        
        hostButton.GetComponent<Button>().enabled = true;
        joinMenuButton.GetComponent<Button>().enabled = true;
        customizeButton.GetComponent<Button>().enabled = true;
        quitButton.GetComponent<Button>().enabled = true;
        settingsButton.GetComponent<Button>().enabled = true;

        joinGameButton.GetComponent<Button>().enabled = false;
        cancelButton.GetComponent<Button>().enabled = false;
    }
}
