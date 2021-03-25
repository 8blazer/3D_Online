using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerPatience : MonoBehaviour
{
    float timer = 0;
    public int points = 4;
    public Sprite happy;
    public Sprite meh;
    public Sprite sad;
    public Sprite mad;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 20 && points > 1)
        {
            timer = 0;
            points--;
            if (GetComponent<Image>().overrideSprite == meh)
            {
                GetComponent<Image>().overrideSprite = sad;
            }
            else if (GetComponent<Image>().overrideSprite == sad)
            {
                GetComponent<Image>().overrideSprite = mad;
            }
            else
            {
                GetComponent<Image>().overrideSprite = meh;
            }
        }
    }
}
