using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform CPCameraPosition;
    [SerializeField] private ColorPicker colorPicker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Options()
    {

    }
    public void GoToColorPicker()
    {
        Camera.main.transform.position = CPCameraPosition.position;
        colorPicker.OpenColorPicker();
    }
}
