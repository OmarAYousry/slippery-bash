using System.Collections;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script enables a collider above the ocean for the players to move on.
/// </summary>
public class SnowBehavior: EventBehavior
{
    [SerializeField] private Collider col = null;
    [SerializeField] private CrystalBehavior crystal = null;

    /// <summary>
    /// The radius in which the crystal spawns is the current players radius plus the addition.
    /// </summary>
    [Tooltip("The radius in which the crystal spawns is the current players radius plus the addition.")]
    public float additionalSpawnRadius = 5;


    //---------------------------------------------------------------------------------------------//
    public override void StartBehavior(float duration)
    {
        Freeze(true);
        StartCoroutine(EnablingIceOcean());
    }

    public override void ResetBehavior()
    {
        col.enabled = false;
        Freeze(false);
    }

    private void Freeze(bool freeze)
    {
        OceanBuoyancy[] tiles = FindObjectsOfType<OceanBuoyancy>();
        Rigidbody currentRb;
        if(tiles != null)
        {
            for(int i = 0; i < tiles.Length; i++)
            {
                currentRb = tiles[i].GetComponent<Rigidbody>();
                if(currentRb)
                {
                    tiles[i].enabled = !freeze;
                    currentRb.useGravity = !freeze;
                    currentRb.isKinematic = freeze;
                    if(freeze)
                        currentRb.velocity = Vector3.zero;
                }
            }
        }
        //if(PlatformController.rigid)
        //{
        //    currentRb = PlatformController.rigid;
        //    currentRb.useGravity = !freeze;
        //    currentRb.isKinematic = freeze;
        //    if(freeze)
        //        currentRb.velocity = Vector3.zero;
        //}

        // spawn crystal       
        if(freeze)
        {
            Vector3 spawnPoint = Vector3.zero;
            float currentRadius = 0;
            int playerAmount = 0;
            if(GameController.players != null)
            {
                for(int i = 0; i < GameController.players.Count; i++)
                {
                    if(GameController.players[i])
                    {
                        playerAmount++;
                        spawnPoint += GameController.players[i].transform.position;
                        currentRadius = Vector3.Distance(GameController.players[i].transform.position, spawnPoint / playerAmount);
                    }
                }
                spawnPoint /= playerAmount;
            }
            Vector2 randomOffset = Random.insideUnitCircle * (currentRadius + additionalSpawnRadius);
            spawnPoint.x += randomOffset.x;
            spawnPoint.z += randomOffset.y;
            crystal.Spawn(spawnPoint);
        }
        else
        {
            crystal.Despawn();
        }
    }

    private IEnumerator EnablingIceOcean()
    {
        yield return null;
        yield return null;
        yield return null;
        col.enabled = true;
    }
}
