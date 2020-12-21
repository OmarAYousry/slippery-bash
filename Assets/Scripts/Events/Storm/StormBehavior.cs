using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script spawns lightning objects at the player positions.
/// </summary>
public class StormBehavior : EventBehavior
{
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
    private LightningBehavior currentLightning;
    private Vector3 hitPoint;
    private float currentRadius;
    private Vector2 randomOffset;


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


    //---------------------------------------------------------------------------------------------//
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
    }

    private void Spawn()
    {
        // check first if a lightning is available
        currentLightning = null;
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
        hitPoint = Vector3.zero;
        for(int i = 0; i < GameController.players.Count; i++)
        {
                hitPoint += GameController.players[i].transform.position;
                currentRadius = Vector3.Distance(GameController.players[i].transform.position, hitPoint / GameController.players.Count);
        }
        hitPoint /= GameController.players.Count;
        randomOffset = Random.insideUnitCircle * (currentRadius + additionalSpawnRadius);
        hitPoint.x += randomOffset.x;
        hitPoint.z += randomOffset.y;

        currentLightning.SetLightning(hitPoint);
    }
}
