using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script sets the exposure and intensity of the sky light.
/// We blend between 2 modes: bright and dark
/// </summary>
public class SkyController: BlendState
{
    private const int STATE_AMOUNT = 3;

    [SerializeField] private HDAdditionalLightData sun = null;
    [SerializeField] private Volume vfx = null;

    public SkyProfile[] states;

    private VolumeProfile profile;
    private HDRISky hDRISky;
    private Exposure exposure;
    private IndirectLightingController indirectLighting;

    private bool baking;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        profile = vfx.profile;
        profile.TryGet(out hDRISky);
        profile.TryGet(out exposure);
        profile.TryGet(out indirectLighting);
    }

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
            SkyProfile[] old = new SkyProfile[states.Length];
            for(int i = 0; i < old.Length; i++)
            {
                old[i] = states[i];
            }
            states = new SkyProfile[STATE_AMOUNT];
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
        baking = !baking;
        if(baking)
            return;

        SkyProfile profile = states[toState];

        sun.lightUnit = LightUnit.Lux;
        float value = Mathf.Lerp(sun.intensity, profile.lux, transition);
        sun.SetIntensity(value, LightUnit.Lux);

        value = Mathf.Lerp(hDRISky.exposure.value, profile.skyExposure, transition);
        hDRISky.exposure.value = value;

        value = Mathf.Lerp(exposure.fixedExposure.value, profile.exposure, transition);
        exposure.fixedExposure.value = value;

        value = Mathf.Lerp(indirectLighting.indirectDiffuseIntensity.value, profile.indirect, transition);
        indirectLighting.indirectDiffuseIntensity.value = value;
    }

    protected override void ApplyState(int toState)
    {
        SkyProfile profile = states[toState];

        sun.lightUnit = LightUnit.Lux;
        sun.SetIntensity(profile.lux, LightUnit.Lux);
        hDRISky.exposure.value = profile.skyExposure;
        exposure.fixedExposure.value = profile.exposure;
        indirectLighting.indirectDiffuseIntensity.value = profile.indirect;
    }

    protected override void OverrideStateProperties(int index)
    {
        VolumeProfile origin = profile;
        if(!origin)
            origin = vfx.sharedProfile;
        HDRISky hDRISky;
        Exposure exposure;
        IndirectLightingController indirectLighting;

        origin.TryGet(out hDRISky);
        origin.TryGet(out exposure);
        origin.TryGet(out indirectLighting);

        SkyProfile file = states[index];

        sun.lightUnit = LightUnit.Lux;
        file.lux = sun.intensity;
        file.skyExposure = hDRISky.exposure.value;
        file.exposure = exposure.fixedExposure.value;
        file.indirect = indirectLighting.indirectDiffuseIntensity.value;
    }
}
