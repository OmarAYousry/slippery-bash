using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    TileController[] tiles;

    void Awake()
    {
        tiles = GetComponentsInChildren<TileController>();
    }
}
