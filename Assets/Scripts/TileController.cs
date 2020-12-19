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

    int timesAlreadySteppedOn = 0;
    const int MaxStepsBeforeBreaking = 3;

    void Awake()
    {
        //if (destroyer == null)
        //    destroyer = GetComponent<MeshDestroy>();
    }

    public IEnumerator DestroyMesh(float secondsToWait)
    {
        //transform.localScale *= 0.9f;
        //destroyer.DestroyMesh();

        yield return new WaitForSeconds(secondsToWait);
        
        tileMesh.SetActive(false);
        brokenMeshParent.SetActive(true);

        foreach (Transform brokenPiece in brokenMeshParent.transform)
        {
            Rigidbody rb = brokenPiece.GetComponent<Rigidbody>();
            float randomFloat = Random.Range(1f, 2f);
            //rb.AddExplosionForce(explosionForce * randomFloat, brokenPiece.transform.position, 1f);
            rb.AddForce(Vector3.up * explosionForce);
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.LogError($"Hit {gameObject.name}");

            // ideally load a different sprite every step or 2 to show damage
            if (timesAlreadySteppedOn++ >= MaxStepsBeforeBreaking)
                StartCoroutine(DestroyMesh(secondsToWait: 1.0f));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
