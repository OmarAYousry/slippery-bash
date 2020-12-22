using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static TileController[] tiles;
    public static Rigidbody rigid;

    void Awake()
    {
        tiles = GetComponentsInChildren<TileController>();
        rigid = GetComponent<Rigidbody>();
    }
}
