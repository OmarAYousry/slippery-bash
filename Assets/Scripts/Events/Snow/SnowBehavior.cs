using System.Collections;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script enables a collider above the ocean for the players to move on.
/// </summary>
public class SnowBehavior: EventBehavior
{
    [SerializeField] private Collider col = null;


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
    }

    private IEnumerator EnablingIceOcean()
    {
        yield return null;
        yield return null;
        yield return null;
        col.enabled = true;
    }
}
