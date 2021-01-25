using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPhysicsMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public List<PhysicMaterial> physicMaterials = new List<PhysicMaterial>();
    void Start()
    {
        gameObject.GetComponent<BoxCollider>().material = physicMaterials[Mathf.Abs(transform.GetChild(0).GetComponent<ChooseRandomMaterial>().selectedMat-1)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
