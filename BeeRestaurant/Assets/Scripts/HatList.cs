using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatList : MonoBehaviour
{
    public static HatList hatListReference;
    public List<GameObject> hatList = new List<GameObject>();
    void Awake()
    {
        hatListReference = this;
        DontDestroyOnLoad(gameObject);
    }
}
