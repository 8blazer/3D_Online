using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] private Transform MMCameraPosition;
    [SerializeField] private Slider redColor;
    [SerializeField] private Slider blueColor;
    [SerializeField] private Slider greenColor;
    [SerializeField] private Text redTcolor;
    [SerializeField] private Text blueTcolor;
    [SerializeField] private Text greenTcolor;
    [SerializeField] private Renderer playerPreview;
    private float previousColorRed;
    private float previousColorBlue;
    private float previousColorGreen;
    void Update()
    {
        Vector3 transfer = new Vector3(playerPreview.transform.eulerAngles.x,
            playerPreview.transform.eulerAngles.y + .05f, playerPreview.transform.eulerAngles.z);
        playerPreview.transform.eulerAngles = transfer;
        if(redColor.value != previousColorRed ||
            blueColor.value != previousColorBlue ||
            greenColor.value != previousColorGreen)
        {
            UpdateColor();
        }
        previousColorRed = redColor.value/255;
        previousColorBlue = blueColor.value/255;
        previousColorGreen = greenColor.value/255;
    }
    public void SubmitColor()
    {
        PlayerPrefs.SetFloat("PCredValue", redColor.value/255);
        PlayerPrefs.SetFloat("PCblueValue", blueColor.value/255);
        PlayerPrefs.SetFloat ("PCgreenValue", greenColor.value/255);
        LeaveColorPicker();
    }
    public void LeaveColorPicker()
    {
        Camera.main.transform.position = gameObject.transform.position;//MainMenu Canvas
    }
    public void UpdateColor()
    {
        playerPreview.material.color = new Color(redColor.value/255,blueColor.value/255,greenColor.value/255, 1);
        redTcolor.text = ("R: "+ redColor.value );
        blueTcolor.text = ("B: " + blueColor.value);
        greenTcolor.text = ("G: " + greenColor.value);
    }
    public void OpenColorPicker()
    {
        redColor.value = Mathf.FloorToInt(255 * PlayerPrefs.GetFloat("PCredValue"));
        blueColor.value = Mathf.FloorToInt(255 * PlayerPrefs.GetFloat("PCblueValue"));
        greenColor.value = Mathf.FloorToInt(255 * PlayerPrefs.GetFloat("PCgreenValue"));
        UpdateColor();
    }
}
