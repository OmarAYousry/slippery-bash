using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPhysicsMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public List<PhysicMaterial> physicMaterials = new List<PhysicMaterial>();
    public GameObject foamObject;

    public GameObject foamInstance;

    public void SetPhysicsMaterial()
    {
        gameObject.GetComponent<BoxCollider>().material = physicMaterials[Mathf.Abs(transform.GetChild(0).GetComponent<ChooseRandomMaterial>().selectedMat-1)];

        if(foamInstance == null)
        {
            foamInstance = Instantiate(foamObject, transform.GetChild(0));
            foamInstance.transform.localScale = Vector3.one;
            foamInstance.transform.localRotation = Quaternion.identity;
            foamInstance.transform.localPosition = Vector3.zero;
            foamInstance.GetComponent<MeshFilter>().sharedMesh = transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
            foamInstance.transform.SetParent(transform, true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
