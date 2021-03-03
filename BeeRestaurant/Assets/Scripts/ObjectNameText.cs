using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectNameText : MonoBehaviour
{
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main 1").GetComponent<Camera>();
        gameObject.GetComponentInParent<Canvas>().worldCamera = camera;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = transform.parent.parent.name;

    }
}
