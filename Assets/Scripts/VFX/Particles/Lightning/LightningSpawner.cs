﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script spawns a lightning object at a player position.
/// </summary>
public class LightningSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    /// <summary>
    /// The lightning strikes near the player positions.
    /// </summary>
    [Tooltip("The lightning strikes near the player positions.")]
    public Transform[] targets;
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
    private int targetAmount;
    private float currentRadius;
    private Vector2 randomOffset;


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Let the thunder begin!
    /// </summary>
    /// <param name="duration">How long is the event?</param>
    public void Spawn(float duration)
    {
        spawnOffset = duration / lightningAmount;
        spawnTimer = spawnOffset;
        amountLeft = lightningAmount;
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
        targetAmount = 0;
        for(int i = 0; i < targets.Length; i++)
        {
            // check if the player hasn't despawned yet (not expected)
            if(targets[i])
            {
                hitPoint += targets[i].position;
                targetAmount++;
                currentRadius = Vector3.Distance(targets[i].position, hitPoint / targetAmount);
            }
        }
        hitPoint /= targetAmount;
        randomOffset = Random.insideUnitSphere * (currentRadius + additionalSpawnRadius);
        hitPoint.x += randomOffset.x;
        hitPoint.z += randomOffset.y;

        currentLightning.SetLightning(hitPoint);
    }
}
