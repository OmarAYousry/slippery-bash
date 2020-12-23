using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script enables a collider above the ocean for the players to move on.
/// </summary>
public class SnowBehavior: EventBehavior
{
    [SerializeField] private Collider col;


    //---------------------------------------------------------------------------------------------//
    public override void StartBehavior(float duration)
    {
        col.enabled = true;
        Freeze(true);
    }

    public override void ResetBehavior()
    {
        col.enabled = false;
        Freeze(false);
    }

    private void Freeze(bool freeze)
    {
        TileController[] tiles = PlatformController.tiles;
        Rigidbody currentRb;
        if(tiles != null)
        {
            for(int i = 0; i < tiles.Length; i++)
            {
                currentRb = tiles[i].GetComponent<Rigidbody>();
                if(currentRb)
                {
                    currentRb.useGravity = !freeze;
                    currentRb.isKinematic = freeze;
                    if(freeze)
                        currentRb.velocity = Vector3.zero;
                }
            }
        }
        if(PlatformController.rigid)
        {
            currentRb = PlatformController.rigid;
            currentRb.useGravity = !freeze;
            currentRb.isKinematic = freeze;
            if(freeze)
                currentRb.velocity = Vector3.zero;
        }
    }
}
