using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script sets the exposure and intensity of the sky light.
/// We blend between 2 modes: bright and dark
/// </summary>
public class SkyController: BlendBinaryState
{
    [SerializeField] private HDAdditionalLightData sun;
    [SerializeField] private Volume vfx;

    public SkyProfile state1;
    public SkyProfile state2;

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


    //---------------------------------------------------------------------------------------------//
    protected override void ApplyProgress(float progress)
    {
        baking = !baking;
        if(baking)
            return;

        ApplyInstant(progress);
    }

    protected override void ApplyInstant(float progress)
    {
        sun.lightUnit = LightUnit.Lux;
        float value = Mathf.Lerp(state1.lux, state2.lux, progress);
        sun.SetIntensity(value, LightUnit.Lux);

        hDRISky.exposure.value = Mathf.Lerp(state1.skyExposure, state2.skyExposure, progress);

        exposure.fixedExposure.value = Mathf.Lerp(state1.exposure, state2.exposure, progress);

        indirectLighting.indirectDiffuseIntensity.value = Mathf.Lerp(state1.indirect, state2.indirect, progress);
    }

    protected override void OverrideStateProperties(bool firstState)
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

        SkyProfile file = firstState ? state1 : state2;

        sun.lightUnit = LightUnit.Lux;
        file.lux = sun.intensity;
        file.skyExposure = hDRISky.exposure.value;
        file.exposure = exposure.fixedExposure.value;
        file.indirect = indirectLighting.indirectDiffuseIntensity.value;
    }
}
