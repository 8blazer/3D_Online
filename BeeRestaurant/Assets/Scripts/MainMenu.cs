using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
        SceneManager.LoadScene("Lobby");
    }
    public void JoinGame()
    {
        joinGameCanvas.GetComponent<Canvas>().enabled = true;

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
}
