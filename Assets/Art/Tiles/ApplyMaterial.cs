using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public Material newMaterial;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Renderer>().material = newMaterial;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
