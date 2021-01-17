using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRandomMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Material> materials = new List<Material>();
    void Awake()
    {
        GetComponent<Renderer>().material = materials[(int)Random.Range(0f, materials.Count)];
    }

    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
