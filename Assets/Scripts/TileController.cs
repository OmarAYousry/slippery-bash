using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField]
    MeshDestroy destroyer;

    void Awake()
    {
        if (destroyer == null)
            destroyer = GetComponent<MeshDestroy>();
    }

    public void DestroyMesh()
    {
        transform.localScale *= 0.9f;
        destroyer.DestroyMesh();
    }
}
