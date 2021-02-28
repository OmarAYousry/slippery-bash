using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script spawns lightning objects at the player positions.
/// </summary>
public class StormBehavior: EventBehavior
{
    private static StormBehavior instance;

    [SerializeField] private GameObject prefab;

    /// <summary>
    /// The lightning can also strike outside the player positions.
    /// </summary>
    [Tooltip("The lightning can also strike outside the player positions.")]
    public float additionalSpawnRadius = 5;
    /// <summary>
    /// How many lightnings should strike during the event?
    /// </summary>
    [Tooltip("How many lightnings should strike during the event?")]
    public int lightningAmount = 5;

    private float spawnTimer;
    private float spawnOffset;
    private int amountLeft;

    private List<LightningBehavior> lightnings = new List<LightningBehavior>();
    private List<Vector3> lightningPositions = new List<Vector3>();


    //---------------------------------------------------------------------------------------------//
    public override void StartBehavior(float duration)
    {
        spawnOffset = duration / lightningAmount;
        spawnTimer = spawnOffset;
        amountLeft = lightningAmount;
    }

    public override void ResetBehavior()
    {
        amountLeft = 0;
    }

    /// <summary>
    /// Get the world positions of all active lightnings.
    /// </summary>
    /// <returns>list of the world positions</returns>
    public static List<Vector3> GetLightningPositions()
    {
        if(!instance)
            return null;

        return instance.lightningPositions;
    }


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(amountLeft > 0)
        {
            if(spawnTimer < spawnOffset)
            {
                spawnTimer += Time.deltaTime;
                return;
            }
            spawnTimer = 0;

            Spawn();
            amountLeft--;
        }

        // update the positions list
        if(lightningPositions.Count > 0)
            lightningPositions.Clear();
        for(int i = 0; i < lightnings.Count; i++)
        {
            if(lightnings[i].gameObject.activeInHierarchy)
            {
                lightningPositions.Add(lightnings[i].transform.position);
            }
        }

    }

    private void Spawn()
    {
        // check first if a lightning is available
        LightningBehavior currentLightning = null;
        for(int i = 0; i < lightnings.Count; i++)
        {
            if(!lightnings[i].gameObject.activeInHierarchy)
            {
                currentLightning = lightnings[i];
                break;
            }
        }

        // add an instance to the pool if necessary
        if(!currentLightning)
        {
            currentLightning = Instantiate(prefab, transform).GetComponent<LightningBehavior>();
            lightnings.Add(currentLightning);
        }

        // search a point to hit
        Vector3 hitPoint = Vector3.zero;
        //float currentRadius = 0;
        //int playerAmount = 0;
        //if(GameController.players != null)
        //{
        //    for(int i = 0; i < GameController.players.Count; i++)
        //    {
        //        if(GameController.players[i])
        //        {
        //            playerAmount++;
        //            hitPoint += GameController.players[i].transform.position;
        //            currentRadius = Vector3.Distance(GameController.players[i].transform.position, hitPoint / playerAmount);
        //        }
        //    }
        //    hitPoint /= playerAmount;
        //}
        //Vector2 randomOffset = Random.insideUnitCircle * (currentRadius + additionalSpawnRadius);
        //hitPoint.x += randomOffset.x;
        //hitPoint.z += randomOffset.y;
        float minDistanceMoved = float.MaxValue;
        if(GameController.players != null)
        {
            for(int i = 0; i < GameController.players.Count; i++)
            {
                if(GameController.players[i])
                {
                    if(GameController.players[i].distanceMoved < minDistanceMoved)
                    {
                        minDistanceMoved = GameController.players[i].distanceMoved;
                        hitPoint = GameController.players[i].transform.position;
                    }
                    else if(GameController.players[i].distanceMoved == minDistanceMoved)
                    {
                        float random = Random.Range(0f, 1f);
                        if(random < .5f)
                        {
                            minDistanceMoved = GameController.players[i].distanceMoved;
                            hitPoint = GameController.players[i].transform.position;
                        }
                    }
                }
            }
        }

        currentLightning.SetLightning(hitPoint);
    }
}
