using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    //[SerializeField]
    //MeshDestroy destroyer;
    [SerializeField]
    GameObject tileMesh;
    [SerializeField]
    GameObject brokenMeshParent;
    [SerializeField]
    float explosionForce;

    void Awake()
    {
        //if (destroyer == null)
        //    destroyer = GetComponent<MeshDestroy>();
    }

    public void DestroyMesh()
    {
        //transform.localScale *= 0.9f;
        //destroyer.DestroyMesh();
        
        tileMesh.SetActive(false);
        brokenMeshParent.SetActive(true);

        foreach (Transform brokenPiece in brokenMeshParent.transform)
        {
            Rigidbody rb = brokenPiece.GetComponent<Rigidbody>();
            float randomFloat = Random.Range(1f, 2f);
            //rb.AddExplosionForce(explosionForce * randomFloat, brokenPiece.transform.position, 1f);
            rb.AddForce(Vector3.up * explosionForce);
        }
    }
}
