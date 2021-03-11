using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Rotation : MonoBehaviour
{
    public Camera camera;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(camera.transform);
        transform.eulerAngles = new Vector3(245, 0, transform.eulerAngles.z);
    }
}
