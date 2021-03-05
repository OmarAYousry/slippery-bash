using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRandomMaterial: MonoBehaviour
{
    // Start is called before the first frame update
    public int selectedMat;
    public List<Material> materials = new List<Material>();

    public bool SetMaterial()
    {
        float noise = Mathf.PerlinNoise((transform.position.x + RandomMaterialManager.randomX) / RandomMaterialManager.noiseSize,
            (transform.position.z + RandomMaterialManager.randomZ) / RandomMaterialManager.noiseSize);
        bool isIce = (noise > RandomMaterialManager.snowThreshold);
        selectedMat = isIce ? 1 : 0;
        //selectedMat = (int)Random.Range(0f, materials.Count);
        GetComponent<Renderer>().material = materials[selectedMat];

        return isIce;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
