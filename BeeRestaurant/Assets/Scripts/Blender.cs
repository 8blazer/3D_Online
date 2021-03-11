using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Blender : NetworkBehaviour
{
    public int itemNumber = 0;
    public Canvas blenderCanvas;
    public Slider blenderSlider;
    float fireTimer;

    // Update is called once per frame
    void Update()
    {
        if (itemNumber > 0)
        {
            blenderSlider.value += Time.deltaTime;
        }
        if (blenderSlider.value > 4.99f)
        {
            fireTimer += Time.deltaTime;
        }
    }

    public void AddItem()
    {
        itemNumber++;
        blenderCanvas.GetComponent<Canvas>().enabled = true;
        blenderSlider.value -= 2;
    }
}
