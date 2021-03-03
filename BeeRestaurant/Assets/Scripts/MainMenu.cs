using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform CPCameraPosition;
    [SerializeField] private ColorPicker colorPicker;
    public void Settings()
    {

    }
    public void Customize()
    {
        colorPicker.OpenColorPicker();
        Camera.main.transform.position = CPCameraPosition.position;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("TaranTesting");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
