using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script describes the general behavior of an event such as starting and resetting it.
/// </summary>
public abstract class EventBehavior : MonoBehaviour
{
    public abstract void StartBehavior(float duration);
    public abstract void ResetBehavior();
}
