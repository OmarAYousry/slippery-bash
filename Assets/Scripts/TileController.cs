using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField]
    GameObject tileMesh;
    [SerializeField]
    GameObject brokenMeshParent;
    [SerializeField]
    ParticleSystem destroyEffect;
    [SerializeField]
    float explosionForce = 100f;
    [SerializeField]
    int hitsTillDestroy = 3;

    int hitsTaken = 0;
    bool destroyed = false;

    void Reset()
    {
        tileMesh = transform.GetChild(0).gameObject;
        brokenMeshParent = transform.GetChild(1).gameObject;
        foreach (Transform child in brokenMeshParent.transform)
        {
            DestroyImmediate(child.gameObject.GetComponent<Rigidbody>());
        }
    }

    public IEnumerator DestroyMesh(float secondsToWait, bool scatterPieces = false)
    {
        yield return new WaitForSeconds(secondsToWait);

        tileMesh.SetActive(false);
        brokenMeshParent.SetActive(true);

        if (destroyEffect != null)
        {
            destroyEffect = Instantiate(destroyEffect);
            destroyEffect.Play();
        }

        if (scatterPieces)
        {
            foreach (Transform brokenPiece in brokenMeshParent.transform)
            {
                Rigidbody rb = brokenPiece.gameObject.AddComponent<Rigidbody>();
                rb.mass = 0.1f;
                float randomFloat = Random.Range(1f, 2f);
                rb.AddForce(Vector3.up * explosionForce);
            }
        }

        yield return null;
    }

    public void DamageTile(float waitTime)
    {
        if (destroyed)
            return;

        hitsTaken++;
        destroyed = (hitsTaken >= hitsTillDestroy);

        StartCoroutine(DestroyMesh(waitTime, destroyed));
    }

    public void DestroyMeshCascading(float cascadeRadius = 2.0f, int maxNumCascades = 999)
    {
        DamageTile(0.0f);

        Collider[] collidersInContact = Physics.OverlapSphere(transform.position, cascadeRadius);

        int numCascades = 0;

        foreach(Collider contactedCollider in collidersInContact)
        {
            if(numCascades++ < maxNumCascades)
                break;

            if(contactedCollider.name.Contains("tile"))
            {
                contactedCollider.GetComponent<TileController>().DamageTile(0.0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // do not consider tile destruction while still in lobby
        if (LobbyBehaviour.isInLobby)
            return;

        if (other.CompareTag("Player"))
        {
            // ideally load a different sprite every step or 2 to show damage
            DamageTile(1.0f);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.LogError(collision.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
