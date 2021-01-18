using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterialBrokenTiles : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject solidTile;

    void Awake()
    {


    }
    void Start()
    {
        Material newMaterial = solidTile.GetComponent<Renderer>().material;
        foreach (Transform child in transform)
        {
            child.GetComponent<Renderer>().material = newMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
