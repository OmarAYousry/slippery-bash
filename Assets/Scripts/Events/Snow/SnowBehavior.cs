using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script enables a collider above the ocean for the players to move on.
/// TODO: also freeze the tiles!
/// </summary>
public class SnowBehavior : EventBehavior
{
    [SerializeField] private Collider col;


    //---------------------------------------------------------------------------------------------//
    public override void StartBehavior(float duration)
    {
        col.enabled = true;
    }

    public override void ResetBehavior()
    {
        col.enabled = false;
    }
}
