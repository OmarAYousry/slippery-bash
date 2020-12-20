using Crest;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script sets the weight of the two wave profiles.
/// We blend between 2 profiles: calm and wavy
/// </summary>
public class WaveController: BlendState
{
    private const int STATE_AMOUNT = 3;

    public ShapeGerstnerBatched[] states;


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Make sure the state array is always size STATE_AMOUNT.
    /// </summary>
    private void OnValidate()
    {
        if(startState >= STATE_AMOUNT)
        {
            startState = STATE_AMOUNT - 1;
        }

        if(states.Length != STATE_AMOUNT)
        {
            ShapeGerstnerBatched[] old = new ShapeGerstnerBatched[states.Length];
            for(int i = 0; i < old.Length; i++)
            {
                old[i] = states[i];
            }
            states = new ShapeGerstnerBatched[STATE_AMOUNT];
            if(old.Length > 0)
            {
                for(int i = 0; i < states.Length; i++)
                {
                    states[i] = old[Mathf.Min(i, old.Length - 1)];
                }
            }
        }
    }

    public override int StateAmount()
    {
        return STATE_AMOUNT;
    }


    //---------------------------------------------------------------------------------------------//
    protected override void ApplyTransition(int toState, float transition)
    {
        float weight;
        ShapeGerstnerBatched target = states[toState];
        for(int i = 0; i < states.Length; i++)
        {
            if(states[i] != target)
            {
                weight = Mathf.Lerp(states[i]._weight, 0, transition);
                states[i]._weight = weight;
            }
        }
        weight = Mathf.Lerp(target._weight, 1, transition);
        target._weight = weight;
    }

    protected override void ApplyState(int toState)
    {
        ShapeGerstnerBatched target = states[toState];
        for(int i = 0; i < states.Length; i++)
        {
            if(states[i] != target)
            {
                states[i]._weight = 0;
            }
        }
        target._weight = 1;
    }

    protected override void DisableState(int previousState) { }

    protected override void OverrideStateProperties(int index)
    {
        Debug.Log("[WaveManager] Overriding is not necessary since changes is already made on the file.");
    }
}
