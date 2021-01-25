using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static PlatformController instance = null;

    public static List<TileController> tiles;
    public static Rigidbody rigid;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        tiles = new List<TileController>();
        rigid = GetComponent<Rigidbody>();
        foreach (TileController tile in GetComponentsInChildren<TileController>())
        {
            tiles.Add(tile);
        }
    }

    public void SplitTileGroups()
    {
        List<TileController> tempTileList = new List<TileController>();
        foreach (TileController tile in tiles)
            tempTileList.Add(tile);
        List<List<TileController>> tileGroups = new List<List<TileController>>();

        while (tempTileList.Count > 0)
        {
            Queue<TileController> tileQueue = new Queue<TileController>();
            List<TileController> tileGroup = new List<TileController>();
            TileController currentTile = tempTileList[0];
            tileQueue.Enqueue(currentTile);

            while (tileQueue.Count > 0)
            {
                TileController tile = tileQueue.Dequeue();
                tempTileList.Remove(tile);

                if (tileGroup.Contains(tile) || tile.isDestroyed)
                    continue;

                tileGroup.Add(tile);
                foreach (TileController neighbor in tile.GetNeightboringTiles())
                {
                    if (!tileGroup.Contains(neighbor))
                        tileQueue.Enqueue(neighbor);
                }
            }
            if (tileGroup.Count > 0)
                tileGroups.Add(tileGroup);
        }

        if (tileGroups.Count > 1)
        {
            foreach (List<TileController> tileGroup in tileGroups)
                CreatePlatform(tileGroup);
        }
    }

    public void CreatePlatform(List<TileController> tiles)
    {
        float maxX = tiles[0].transform.position.x;
        float maxZ = tiles[0].transform.position.z;
        float minX = tiles[0].transform.position.x;
        float minZ = tiles[0].transform.position.z;

        List<GameObject> buoyancyTiles = new List<GameObject>{ tiles[0].gameObject, tiles[0].gameObject, tiles[0].gameObject, tiles[0].gameObject };

        if (tiles.Count <= 4)
        {
            buoyancyTiles = new List<GameObject>();
            for (int i = 0; i < tiles.Count; i++)
            {
                buoyancyTiles.Add(tiles[i].gameObject);
            }
        }
        else
        {
            foreach (TileController tile in tiles)
            {
                if (tile.transform.position.x > maxX && tile.transform.position.z > maxZ)
                    buoyancyTiles[0] = tile.gameObject;
                if (tile.transform.position.x < minX && tile.transform.position.z > maxZ)
                    buoyancyTiles[1] = tile.gameObject;
                if (tile.transform.position.x > maxX && tile.transform.position.z < minZ)
                    buoyancyTiles[2] = tile.gameObject;
                if (tile.transform.position.x < minX && tile.transform.position.z < minZ)
                    buoyancyTiles[3] = tile.gameObject;
            }
        }

        List<GameObject> buoyancyElements = new List<GameObject>();
        foreach (GameObject element in buoyancyTiles)
        {
            if (!buoyancyElements.Contains(element))
                buoyancyElements.Add(element);
        }

        GameObject parentObj = new GameObject();
        parentObj.name = "sub-platform";

        foreach (TileController tile in tiles)
            tile.transform.parent = parentObj.transform;

        Rigidbody rb = parentObj.AddComponent<Rigidbody>();
        rb.mass = 30;
        OceanBuoyancy buoyancy = parentObj.AddComponent<OceanBuoyancy>();
        buoyancy.buoyancyElements = buoyancyElements;
        buoyancy.buoyancyForce = 50;
        buoyancy.depthFactor = 1;
        buoyancy.surfaceOffset = 1.5f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        //StartCoroutine(WaitThenDoAction(() =>
        //{
        //    rb.velocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //},
        //1f,
        //() =>
        //{
        //    rb.velocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}));
    }

    static IEnumerator WaitThenDoAction(System.Action beforeAction, float duration, System.Action afterAction)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            beforeAction();
            yield return new WaitForFixedUpdate();
        }
        afterAction();
    }
}
