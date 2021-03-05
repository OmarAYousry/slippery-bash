using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RandomMaterialManager: MonoBehaviour
{
    public float snowThresholdParam = .4f;
    public float noiseSizeParam = 0.5f;
    public Vector2 snowRatioRange = new Vector2(.2f, .3f);
    public int testSeed = 0;

    public bool useRandom;
    public bool generateSeeds;
    public int[] usedSeeds;


    public static float snowThreshold = .4f;
    public static float noiseSize = 0.5f;

    public static float randomX = -100;
    public static float randomZ = -100;

    private void Start()
    {
        snowThreshold = snowThresholdParam;
        noiseSize = noiseSizeParam;

        if(generateSeeds)
            GenerateSeeds();

        int seed = testSeed;
        if(useRandom)
        {
            seed = Random.Range(0, usedSeeds.Length);
            Random.InitState(usedSeeds[seed]);
        }
        else
        {
            Random.InitState(usedSeeds[seed]);
        }

        randomX = Random.Range(0, 1000f);
        randomZ = Random.Range(0, 1000f);
        SetTiles();
    }

    private void GenerateSeeds()
    {
        int seed = testSeed;
        int iterator = 0;
        usedSeeds = new int[10];
        while(iterator < usedSeeds.Length)
        {
            Random.InitState(seed);
            randomX = Random.Range(0, 1000f);
            randomZ = Random.Range(0, 1000f);

            float ratio = SetTiles();
            if(ratio >= snowRatioRange.x && ratio <= snowRatioRange.y)
            {
                usedSeeds[iterator] = seed;
                iterator++;
            }
            seed++;
        }
    }

    private float SetTiles()
    {
        List<TileController> tiles = PlatformController.tiles;
        int snowAmount = 0;
        foreach(TileController tile in tiles)
        {
            if(!tile.transform.GetChild(0).GetComponent<ChooseRandomMaterial>().SetMaterial())
                snowAmount++;
            tile.GetComponent<ApplyPhysicsMaterial>().SetPhysicsMaterial();
        }
        return (float)snowAmount / tiles.Count;
    }

    private void Update()
    {
        if(generateSeeds && Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            Random.InitState(testSeed);
            randomX = Random.Range(0, 1000f);
            randomZ = Random.Range(0, 1000f);
            SetTiles();
        }
    }
}
