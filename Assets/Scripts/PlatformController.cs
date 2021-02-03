using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static PlatformController instance = null;

    public static List<TileController> tiles;
    public static Rigidbody rigid;
    public static List<GameObject> subPlatforms = new List<GameObject>();
    public static int currentPlatformsNumber = 0;

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

        if ((tileGroups.Count > 1) && (tileGroups.Count != currentPlatformsNumber))
        {
            List<GameObject> subPlatformsToRemove = new List<GameObject>();
            foreach (GameObject go in subPlatforms)
                subPlatformsToRemove.Add(go);
            foreach (GameObject go in subPlatformsToRemove)
            {
                foreach (TileController childTile in go.GetComponentsInChildren<TileController>())
                {
                    childTile.transform.parent = null;
                }

                subPlatforms.Remove(go);
                Destroy(go);
            }

            foreach (List<TileController> tileGroup in tileGroups)
                CreatePlatform(tileGroup);
        }

        currentPlatformsNumber = tileGroups.Count;
    }

    public void CreatePlatform(List<TileController> tiles)
    {
        float maxX = tiles[0].transform.localPosition.x;
        float maxZ = tiles[0].transform.localPosition.z;
        float minX = tiles[0].transform.localPosition.x;
        float minZ = tiles[0].transform.localPosition.z;

        List<GameObject> buoyancyTiles = new List<GameObject>{ tiles[0].gameObject, tiles[0].gameObject, tiles[0].gameObject, tiles[0].gameObject };

        foreach (TileController tile in tiles)
        {
            if (tile.transform.localPosition.x > maxX && tile.transform.localPosition.z > maxZ)
            {
                buoyancyTiles[0] = tile.gameObject;
            }
            if (tile.transform.localPosition.x < minX && tile.transform.localPosition.z > maxZ)
            {
                buoyancyTiles[1] = tile.gameObject;
            }
            if (tile.transform.localPosition.x > maxX && tile.transform.localPosition.z < minZ)
            {
                buoyancyTiles[2] = tile.gameObject;
            }
            if (tile.transform.localPosition.x < minX && tile.transform.localPosition.z < minZ)
            {
                buoyancyTiles[3] = tile.gameObject;
            }

            if (tile.transform.position.x > maxX)
                maxX = tile.transform.position.x;
            if (tile.transform.position.x < minX)
                minX = tile.transform.position.x;
            if (tile.transform.position.z > maxZ)
                maxZ = tile.transform.position.z;
            if (tile.transform.position.z < minZ)
                minZ = tile.transform.position.z;
        }

        GameObject parentObj = new GameObject();
        parentObj.name = "sub-platform";

        Vector3 centerPos = new Vector3();
        foreach(GameObject be in buoyancyTiles)
        {
            centerPos += be.transform.position;
        }
        centerPos = centerPos / buoyancyTiles.Count;
        parentObj.transform.position = centerPos;
        parentObj.transform.rotation = this.transform.rotation;

        float offset = 0.25f;
        List<GameObject> buoyancyElements = new List<GameObject>();

        GameObject topRight = new GameObject();
        topRight.transform.localPosition = new Vector3(maxX + offset, buoyancyTiles[0].transform.localPosition.y, maxZ + offset);
        //topRight.transform.position = GetUpRightTile(tiles[0]).transform.position;
        topRight.transform.parent = parentObj.transform;
        buoyancyElements.Add(topRight);

        GameObject topLeft = new GameObject();
        topLeft.transform.localPosition = new Vector3(minX - offset, buoyancyTiles[1].transform.localPosition.y, maxZ + offset);
        //topLeft.transform.position = GetUpLeftTile(tiles[0]).transform.position;
        topLeft.transform.parent = parentObj.transform;
        buoyancyElements.Add(topLeft);

        GameObject bottomRight = new GameObject();
        bottomRight.transform.localPosition = new Vector3(maxX + offset, buoyancyTiles[2].transform.localPosition.y, minZ - offset);
        //bottomRight.transform.position = GetDownRightTile(tiles[0]).transform.position;
        bottomRight.transform.parent = parentObj.transform;
        buoyancyElements.Add(bottomRight);

        GameObject bottomLeft = new GameObject();
        bottomLeft.transform.localPosition = new Vector3(minX - offset, buoyancyTiles[3].transform.localPosition.y, minZ - offset);
        //bottomLeft.transform.position = GetDownLeftTile(tiles[0]).transform.position;
        bottomLeft.transform.parent = parentObj.transform;
        buoyancyElements.Add(bottomLeft);

        foreach (TileController tile in tiles)
        {
            if (tile.isDestroyed)
                continue;
            tile.transform.parent = parentObj.transform;
        }

        Rigidbody rb = parentObj.AddComponent<Rigidbody>();
        rb.mass = (float)tiles.Count * 2.5f;
        rb.angularDrag = 3f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        OceanBuoyancy buoyancy = parentObj.AddComponent<OceanBuoyancy>();
        buoyancy.buoyancyElements = buoyancyElements;
        buoyancy.buoyancyForce = (float)tiles.Count * 5f;
        buoyancy.depthFactor = 1;
        buoyancy.surfaceOffset = 1f;

        gameObject.GetComponent<OceanBuoyancy>().enabled = false;
        if (rigid != null)
            Destroy(rigid);
        subPlatforms.Add(parentObj);
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

    static TileController GetUpRightTile(TileController tile)
    {
        if (tile.upTile == null)
            if (tile.rightTile == null)
                return tile;
            else if (!tile.rightTile.isDestroyed)
                return GetUpRightTile(tile.rightTile);
            else
                return tile;
        else if (!tile.upTile.isDestroyed)
            return GetUpRightTile(tile.upTile);
        else
            return tile;
    }
    static TileController GetUpLeftTile(TileController tile)
    {
        if (tile.upTile == null)
            if (tile.leftTile == null)
                return tile;
            else if (!tile.leftTile.isDestroyed)
                return GetUpLeftTile(tile.leftTile);
            else
                return tile;
        else if (!tile.upTile.isDestroyed)
            return GetUpLeftTile(tile.upTile);
        else
            return tile;
    }
    static TileController GetDownRightTile(TileController tile)
    {
        if (tile.downTile == null)
            if (tile.rightTile == null)
                return tile;
            else if (!tile.rightTile.isDestroyed)
                return GetDownRightTile(tile.rightTile);
            else
                return tile;
        else if (!tile.downTile.isDestroyed)
            return GetDownRightTile(tile.downTile);
        else
            return tile;
    }
    static TileController GetDownLeftTile(TileController tile)
    {
        if (tile.downTile == null)
            if (tile.leftTile == null)
                return tile;
            else if (!tile.leftTile.isDestroyed)
                return GetDownLeftTile(tile.leftTile);
            else
                return tile;
        else if (!tile.downTile.isDestroyed)
            return GetDownLeftTile(tile.downTile);
        else
            return tile;
    }
}
