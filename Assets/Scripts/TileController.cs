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
    float tilePieceMass = 0.01f;
    [SerializeField]
    float explosionForce = 10f;
    [SerializeField]
    int hitsTillDestroy = 3;

    [Header("Neighbor Tiles")]
    public TileController upTile;
    public TileController downTile;
    public TileController rightTile;
    public TileController leftTile;

    int hitsTaken = 0;
    bool destroyed = false;
    System.Action onDestroy = null;

    const float neighborRayDistance = 3f;

    public bool isDestroyed
    {
        get { return destroyed; }
    }

    void Reset()
    {
        tileMesh = transform.GetChild(0).gameObject;
        brokenMeshParent = transform.GetChild(1).gameObject;
        LocateNeighboringTiles();
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
                brokenPiece.gameObject.AddComponent<TimedDestroyer>();
                Rigidbody rb = brokenPiece.gameObject.AddComponent<Rigidbody>();
                rb.mass = 0.001f;
                float randomFloat = Random.Range(1f, 2f);
                rb.AddForce(Vector3.up * explosionForce);
            }

            PlatformController.instance.SplitTileGroups();

            if (onDestroy != null)
            {
                onDestroy();
                onDestroy = null;
            }
        }

        yield return null;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public List<TileController> GetNeightboringTiles()
    {
        List<TileController> neightbors = new List<TileController>();
        if (upTile != null)
            if (!upTile.destroyed)
                neightbors.Add(upTile);
        if (downTile != null)
            if (!downTile.destroyed)
                neightbors.Add(downTile);
        if (rightTile != null)
            if (!rightTile.destroyed)
                neightbors.Add(rightTile);
        if (leftTile != null)
            if (!leftTile.destroyed)
                neightbors.Add(leftTile);
        return neightbors;
    }

    public void SeperateTileFromParent(Transform newParent = null)
    {
        if (transform.parent == null)
            return;

        transform.parent = newParent;

        if (newParent == null)
            gameObject.AddComponent<Rigidbody>().mass = 5f;

        OceanSimpleBuoyancy buoyancy = gameObject.GetComponent<OceanSimpleBuoyancy>();
        if (buoyancy == null)
            buoyancy = gameObject.AddComponent<OceanSimpleBuoyancy>();

        buoyancy.buoyancyForce = 50f;
        buoyancy.depthFactor = 1;
        buoyancy.surfaceOffset = 1f;
    }

    public void LocateNeighboringTiles()
    {
        Quaternion oldRot = transform.rotation;
        transform.rotation = Quaternion.identity;

        RaycastHit hitUp, hitDown, hitRight, hitLeft;
        if (Physics.Raycast(transform.position, transform.forward, out hitUp, neighborRayDistance))
            upTile = hitUp.collider.gameObject.GetComponent<TileController>();
        if (Physics.Raycast(transform.position, -transform.forward, out hitDown, neighborRayDistance))
            downTile = hitDown.collider.gameObject.GetComponent<TileController>();
        if (Physics.Raycast(transform.position, transform.right, out hitRight, neighborRayDistance))
            rightTile = hitRight.collider.gameObject.GetComponent<TileController>();
        if (Physics.Raycast(transform.position, -transform.right, out hitLeft, neighborRayDistance))
            leftTile = hitLeft.collider.gameObject.GetComponent<TileController>();

        transform.rotation = oldRot;
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

    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
