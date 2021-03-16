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
    [SerializeField] private Renderer[] playerPreviewColors = new Renderer[3];
    [SerializeField] private Transform playerPreview;
    [SerializeField] private Transform hatPreview;

    
    private GameObject currentHat;
    private int hatNumber;
    private float previousColorRed;
    private float previousColorBlue;
    private float previousColorGreen;

    void Update()
    {
        Vector3 transfer = new Vector3(playerPreview.transform.eulerAngles.x,
            playerPreview.transform.eulerAngles.y + .08f, playerPreview.transform.eulerAngles.z);
        playerPreview.transform.eulerAngles = transfer;
        if (redColor.value != previousColorRed ||
            blueColor.value != previousColorBlue ||
            greenColor.value != previousColorGreen)
        {
            UpdateColor();
        }
        previousColorRed = redColor.value / 255;
        previousColorBlue = blueColor.value / 255;
        previousColorGreen = greenColor.value / 255;
    }
    public void SubmitColor()
    {
        PlayerPrefs.SetInt("HatNumber", hatNumber);
        PlayerPrefs.SetFloat("PCredValue", redColor.value / 255);
        PlayerPrefs.SetFloat("PCblueValue", blueColor.value / 255);
        PlayerPrefs.SetFloat("PCgreenValue", greenColor.value / 255);
        LeaveColorPicker();
    }
    public void LeaveColorPicker()
    {
        Camera.main.transform.position = MMCameraPosition.transform.position;//MainMenu Canvas
    }
    public void UpdateColor()
    {
        foreach(Renderer rend in playerPreviewColors)
        {
            rend.material.color = new Color(redColor.value / 255, greenColor.value / 255, blueColor.value / 255, 1);
        }
        redTcolor.text = ("R: " + redColor.value);
        blueTcolor.text = ("B: " + blueColor.value);
        greenTcolor.text = ("G: " + greenColor.value);
    }
    public void OpenColorPicker()
    {
        if(currentHat != null) { Destroy(currentHat); }
        currentHat = Instantiate(HatList.hatListReference.hatList[PlayerPrefs.GetInt("HatNumber")], hatPreview);
        redColor.value = Mathf.FloorToInt(255 * PlayerPrefs.GetFloat("PCredValue"));
        blueColor.value = Mathf.FloorToInt(255 * PlayerPrefs.GetFloat("PCblueValue"));
        greenColor.value = Mathf.FloorToInt(255 * PlayerPrefs.GetFloat("PCgreenValue"));
        UpdateColor();
    }

    #region HatNumber
    public void Hat0()
    {
        if (currentHat != null) { Destroy(currentHat);}
        currentHat = Instantiate(HatList.hatListReference.hatList[0], hatPreview);
        hatNumber = 0;
    }
    public void Hat1()
    {
        if (currentHat != null) { Destroy(currentHat); }
        currentHat = Instantiate(HatList.hatListReference.hatList[1], hatPreview);
        hatNumber = 1;
    }
    public void Hat2()
    {
        if (currentHat != null) { Destroy(currentHat); }
        currentHat = Instantiate(HatList.hatListReference.hatList[2], hatPreview);
        hatNumber = 2;
    }
    public void Hat3()
    {
        if (currentHat != null) { Destroy(currentHat); }
        currentHat = Instantiate(HatList.hatListReference.hatList[3], hatPreview);
        hatNumber = 3;
    }
    public void Hat4()
    {
        if (currentHat != null) { Destroy(currentHat); }
        currentHat = Instantiate(HatList.hatListReference.hatList[4], hatPreview);
        hatNumber = 4;
    }
    #endregion
}

