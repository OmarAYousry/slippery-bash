using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRandomMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public int selectedMat;
    public List<Material> materials = new List<Material>();
    void Awake()
    {
        selectedMat = (int)Random.Range(0f, materials.Count);
        GetComponent<Renderer>().material = materials[selectedMat];
    }

    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
