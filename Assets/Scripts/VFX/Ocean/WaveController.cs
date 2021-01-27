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

    [Header("Changing Properties")]
    [SerializeField] private OceanRenderer ocean = null;

    public ShapeGerstnerBatched[] states;
    public Material idleOceanMat;
    public Material frozenOceanMat;

    private Material oceanMat;


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

    private void Awake()
    {
        oceanMat = ocean.OceanMaterial;
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

        Material toMaterial = toState == 2 ? frozenOceanMat : idleOceanMat;
        oceanMat.SetFloat("_NormalsScale", Mathf.Lerp(oceanMat.GetFloat("_NormalsScale"), toMaterial.GetFloat("_NormalsScale"), transition));
        oceanMat.SetFloat("_NormalsStrength", Mathf.Lerp(oceanMat.GetFloat("_NormalsStrength"), toMaterial.GetFloat("_NormalsStrength"), transition));
        oceanMat.SetColor("_ScatterColourBase", Color.Lerp(oceanMat.GetColor("_ScatterColourBase"), toMaterial.GetColor("_ScatterColourBase"), transition));
        oceanMat.SetColor("_ScatterColourShadow", Color.Lerp(oceanMat.GetColor("_ScatterColourShadow"), toMaterial.GetColor("_ScatterColourShadow"), transition));
        oceanMat.SetColor("_ScatterColourShallow", Color.Lerp(oceanMat.GetColor("_ScatterColourShallow"), toMaterial.GetColor("_ScatterColourShallow"), transition));
        oceanMat.SetFloat("_ScatterColourShallowDepthMax", Mathf.Lerp(oceanMat.GetFloat("_ScatterColourShallowDepthMax"), toMaterial.GetFloat("_ScatterColourShallowDepthMax"), transition));
        oceanMat.SetFloat("_ScatterColourShallowDepthFalloff", Mathf.Lerp(oceanMat.GetFloat("_ScatterColourShallowDepthFalloff"), toMaterial.GetFloat("_ScatterColourShallowDepthFalloff"), transition));
        oceanMat.SetFloat("_SSSIntensityBase", Mathf.Lerp(oceanMat.GetFloat("_SSSIntensityBase"), toMaterial.GetFloat("_SSSIntensityBase"), transition));
        oceanMat.SetFloat("_SSSIntensitySun", Mathf.Lerp(oceanMat.GetFloat("_SSSIntensitySun"), toMaterial.GetFloat("_SSSIntensitySun"), transition));
        oceanMat.SetColor("_SSSTint", Color.Lerp(oceanMat.GetColor("_SSSTint"), toMaterial.GetColor("_SSSTint"), transition));
        oceanMat.SetFloat("_SSSSunFalloff", Mathf.Lerp(oceanMat.GetFloat("_SSSSunFalloff"), toMaterial.GetFloat("_SSSSunFalloff"), transition));
        oceanMat.SetFloat("_Specular", Mathf.Lerp(oceanMat.GetFloat("_Specular"), toMaterial.GetFloat("_Specular"), transition));
        oceanMat.SetFloat("_Occlusion", Mathf.Lerp(oceanMat.GetFloat("_Occlusion"), toMaterial.GetFloat("_Occlusion"), transition));
        oceanMat.SetFloat("_Smoothness", Mathf.Lerp(oceanMat.GetFloat("_Smoothness"), toMaterial.GetFloat("_Smoothness"), transition));
        oceanMat.SetFloat("_SmoothnessFar", Mathf.Lerp(oceanMat.GetFloat("_SmoothnessFar"), toMaterial.GetFloat("_SmoothnessFar"), transition));
        oceanMat.SetFloat("_SmoothnessFarDistance", Mathf.Lerp(oceanMat.GetFloat("_SmoothnessFarDistance"), toMaterial.GetFloat("_SmoothnessFarDistance"), transition));
        oceanMat.SetFloat("_SmoothnessFalloff", Mathf.Lerp(oceanMat.GetFloat("_SmoothnessFalloff"), toMaterial.GetFloat("_SmoothnessFalloff"), transition));
        oceanMat.SetFloat("_MinReflectionDirectionY", Mathf.Lerp(oceanMat.GetFloat("_MinReflectionDirectionY"), toMaterial.GetFloat("_MinReflectionDirectionY"), transition));
        oceanMat.SetFloat("_FoamScale", Mathf.Lerp(oceanMat.GetFloat("_FoamScale"), toMaterial.GetFloat("_FoamScale"), transition));
        oceanMat.SetFloat("_FoamFeather", Mathf.Lerp(oceanMat.GetFloat("_FoamFeather"), toMaterial.GetFloat("_FoamFeather"), transition));
        oceanMat.SetFloat("_FoamIntensityAlbedo", Mathf.Lerp(oceanMat.GetFloat("_FoamIntensityAlbedo"), toMaterial.GetFloat("_FoamIntensityAlbedo"), transition));
        oceanMat.SetFloat("_FoamIntensityEmissive", Mathf.Lerp(oceanMat.GetFloat("_FoamIntensityEmissive"), toMaterial.GetFloat("_FoamIntensityEmissive"), transition));
        oceanMat.SetFloat("_FoamSmoothness", Mathf.Lerp(oceanMat.GetFloat("_FoamSmoothness"), toMaterial.GetFloat("_FoamSmoothness"), transition));
        oceanMat.SetFloat("_FoamNormalStrength", Mathf.Lerp(oceanMat.GetFloat("_FoamNormalStrength"), toMaterial.GetFloat("_FoamNormalStrength"), transition));
        oceanMat.SetColor("_FoamBubbleColor", Color.Lerp(oceanMat.GetColor("_FoamBubbleColor"), toMaterial.GetColor("_FoamBubbleColor"), transition));
        oceanMat.SetFloat("_FoamBubbleParallax", Mathf.Lerp(oceanMat.GetFloat("_FoamBubbleParallax"), toMaterial.GetFloat("_FoamBubbleParallax"), transition));
        oceanMat.SetFloat("_FoamBubblesCoverage", Mathf.Lerp(oceanMat.GetFloat("_FoamBubblesCoverage"), toMaterial.GetFloat("_FoamBubblesCoverage"), transition));
        oceanMat.SetFloat("_RefractionStrength", Mathf.Lerp(oceanMat.GetFloat("_RefractionStrength"), toMaterial.GetFloat("_RefractionStrength"), transition));
        oceanMat.SetVector("_DepthFogDensity", Vector4.Lerp(oceanMat.GetVector("_DepthFogDensity"), toMaterial.GetVector("_DepthFogDensity"), transition));
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

        Material toMaterial = toState == 2 ? frozenOceanMat : idleOceanMat;
        oceanMat.SetFloat("_NormalsScale", toMaterial.GetFloat("_NormalsScale"));
        oceanMat.SetFloat("_NormalsStrength", toMaterial.GetFloat("_NormalsStrength"));
        oceanMat.SetColor("_ScatterColourBase", toMaterial.GetColor("_ScatterColourBase"));
        oceanMat.SetColor("_ScatterColourShadow", toMaterial.GetColor("_ScatterColourShadow"));
        oceanMat.SetColor("_ScatterColourShallow", toMaterial.GetColor("_ScatterColourShallow"));
        oceanMat.SetFloat("_ScatterColourShallowDepthMax", toMaterial.GetFloat("_ScatterColourShallowDepthMax"));
        oceanMat.SetFloat("_ScatterColourShallowDepthFalloff", toMaterial.GetFloat("_ScatterColourShallowDepthFalloff"));
        oceanMat.SetFloat("_SSSIntensityBase", toMaterial.GetFloat("_SSSIntensityBase"));
        oceanMat.SetFloat("_SSSIntensitySun", toMaterial.GetFloat("_SSSIntensitySun"));
        oceanMat.SetColor("_SSSTint", toMaterial.GetColor("_SSSTint"));
        oceanMat.SetFloat("_SSSSunFalloff", toMaterial.GetFloat("_SSSSunFalloff"));
        oceanMat.SetFloat("_Specular", toMaterial.GetFloat("_Specular"));
        oceanMat.SetFloat("_Occlusion", toMaterial.GetFloat("_Occlusion"));
        oceanMat.SetFloat("_Smoothness", toMaterial.GetFloat("_Smoothness"));
        oceanMat.SetFloat("_SmoothnessFar", toMaterial.GetFloat("_SmoothnessFar"));
        oceanMat.SetFloat("_SmoothnessFarDistance", toMaterial.GetFloat("_SmoothnessFarDistance"));
        oceanMat.SetFloat("_SmoothnessFalloff", toMaterial.GetFloat("_SmoothnessFalloff"));
        oceanMat.SetFloat("_MinReflectionDirectionY", toMaterial.GetFloat("_MinReflectionDirectionY"));
        oceanMat.SetFloat("_FoamScale", toMaterial.GetFloat("_FoamScale"));
        oceanMat.SetFloat("_FoamFeather", toMaterial.GetFloat("_FoamFeather"));
        oceanMat.SetFloat("_FoamIntensityAlbedo", toMaterial.GetFloat("_FoamIntensityAlbedo"));
        oceanMat.SetFloat("_FoamIntensityEmissive", toMaterial.GetFloat("_FoamIntensityEmissive"));
        oceanMat.SetFloat("_FoamSmoothness", toMaterial.GetFloat("_FoamSmoothness"));
        oceanMat.SetFloat("_FoamNormalStrength", toMaterial.GetFloat("_FoamNormalStrength"));
        oceanMat.SetColor("_FoamBubbleColor", toMaterial.GetColor("_FoamBubbleColor"));
        oceanMat.SetFloat("_FoamBubbleParallax", toMaterial.GetFloat("_FoamBubbleParallax"));
        oceanMat.SetFloat("_FoamBubblesCoverage", toMaterial.GetFloat("_FoamBubblesCoverage"));
        oceanMat.SetFloat("_RefractionStrength", toMaterial.GetFloat("_RefractionStrength"));
        oceanMat.SetVector("_DepthFogDensity", toMaterial.GetVector("_DepthFogDensity"));
    }

    protected override void DisableState(int previousState) { }

    protected override void OverrideStateProperties(int index)
    {
        Debug.Log("[WaveManager] Overriding is not necessary since changes is already made on the file.");
    }
}
