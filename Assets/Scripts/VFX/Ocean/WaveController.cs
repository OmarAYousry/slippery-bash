using Crest;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script sets the weight of the two wave profiles.
/// We blend between 2 profiles: calm and wavy
/// </summary>
public class WaveController: BlendBinaryState
{
    [SerializeField] private ShapeGerstnerBatched state1;
    [SerializeField] private ShapeGerstnerBatched state2;


    //---------------------------------------------------------------------------------------------//
    protected override void ApplyProgress(float progress)
    {
        ApplyInstant(progress);
    }

    protected override void ApplyInstant(float progress)
    {
        state1._weight = 1 - progress;
        state2._weight = progress;
    }

    protected override void OverrideStateProperties(bool firstState)
    {
        Debug.Log("[WaveManager] Overriding is not necessary since changes is already made on the file.");
    }
}
