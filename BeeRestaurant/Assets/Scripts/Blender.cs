using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

public class Blender : NetworkBehaviour
{
    public int itemNumber = 0;
    public string completedDish = "";
    public Canvas blenderCanvas;
    public Slider blenderSlider;
    float fireTimer;
    public List<string> ingredients = new List<string>();

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
            if (itemNumber == 3)
            {
                if (ingredients[0] == "red" && ingredients[1] == "red" && ingredients[2] == "red")
                {
                    completedDish = "red";
                }
                else if (ingredients[0] == "blue" && ingredients[1] == "blue" && ingredients[2] == "blue")
                {
                    completedDish = "blue";
                }
                else if (ingredients[0] == "yellow" && ingredients[1] == "yellow" && ingredients[2] == "yellow")
                {
                    completedDish = "yellow";
                }
            }
            else if (itemNumber == 4)
            {
                if (ingredients[0] == "red")
                {
                    int i = 0;
                    while (ingredients.Contains("red"))
                    {
                        if (ingredients[i] == "red")
                        {
                            ingredients.RemoveAt(i);
                        }
                        i++;
                    }
                    if (ingredients.Count() != 2)
                    {
                        if (ingredients[0] == "blue" && ingredients[1] == "blue")
                        {
                            completedDish = "purple";
                        }
                        else if (ingredients[0] == "yellow" && ingredients[1] == "yellow")
                        {
                            completedDish = "orange";
                        }
                    }
                }
                else if (ingredients[0] == "yellow")
                {
                    int i = 0;
                    while (ingredients.Contains("yellow"))
                    {
                        if (ingredients[i] == "yellow")
                        {
                            ingredients.RemoveAt(i);
                        }
                        i++;
                    }
                    if (ingredients.Count() != 2)
                    {
                        if (ingredients[0] == "blue" && ingredients[1] == "blue")
                        {
                            completedDish = "green";
                        }
                        else if (ingredients[0] == "red" && ingredients[1] == "red")
                        {
                            completedDish = "orange";
                        }
                    }
                }
                else if (ingredients[0] == "blue")
                {
                    int i = 0;
                    while (ingredients.Contains("blue"))
                    {
                        if (ingredients[i] == "blue")
                        {
                            ingredients.RemoveAt(i);
                        }
                        i++;
                    }
                    if (ingredients.Count() != 2)
                    {
                        if (ingredients[0] == "red" && ingredients[1] == "red")
                        {
                            completedDish = "purple";
                        }
                        else if (ingredients[0] == "yellow" && ingredients[1] == "yellow")
                        {
                            completedDish = "green";
                        }
                    }
                }
            }
        }
    }

    public void AddItem(string pollenColor)
    {
        ingredients.Add(pollenColor);
        itemNumber++;
        blenderCanvas.GetComponent<Canvas>().enabled = true;
        blenderSlider.value -= 3;
        fireTimer = 0;
    }
}
