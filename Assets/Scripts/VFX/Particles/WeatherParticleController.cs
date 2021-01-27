using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script sets the emission rate of all weather particle systems.
/// We blend between 3 profiles: idle, rainy and snowy
/// </summary>
public class WeatherParticleController : BlendState
{
    private const int STATE_AMOUNT = 3;

    [SerializeField] private ParticleSystem rain = null;
    [SerializeField] private ParticleSystem snow = null;

    /// <summary>
    /// The emission settings for each state.
    /// </summary>
    [Tooltip("The emission settings for each state.")]
    public ParticlesProfile[] states;
    /// <summary>
    /// The emission size depending on the current camera distance.
    /// </summary>
    [Tooltip("The emission size depending on the current camera distance.")]
    public float sizeMultiplier = 1;

    private ParticleSystem.ShapeModule rainShape;
    private ParticleSystem.ShapeModule snowShape;


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
            ParticlesProfile[] old = new ParticlesProfile[states.Length];
            for(int i = 0; i < old.Length; i++)
            {
                old[i] = states[i];
            }
            states = new ParticlesProfile[STATE_AMOUNT];
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
        rainShape = rain.shape;
        snowShape = snow.shape;
    }

    public override int StateAmount()
    {
        return STATE_AMOUNT;
    }

    private void LateUpdate()
    {
        rain.transform.position = snow.transform.position = GameCamera.Instance.GetBounds().center;
        rainShape.scale = snowShape.scale = new Vector3(1, 1, 0) *GameCamera.Instance.GetDistance() * sizeMultiplier;
    }


    //---------------------------------------------------------------------------------------------//
    protected override void ApplyTransition(int toState, float transition)
    {
        ParticlesProfile profile = states[toState];

        ParticleSystem.EmissionModule emission = rain.emission;
        float value = Mathf.Lerp(emission.rateOverTime.constant, profile.rainEmission, transition);
        emission.rateOverTime = value;

        emission = snow.emission;
        value = Mathf.Lerp(emission.rateOverTime.constant, profile.snowEmission, transition);
        emission.rateOverTime = value;
    }

    protected override void ApplyState(int toState)
    {
        ParticlesProfile profile = states[toState];

        ParticleSystem.EmissionModule emission = rain.emission;
        emission.rateOverTime = profile.rainEmission;

        emission = snow.emission;
        emission.rateOverTime = profile.snowEmission;
    }

    protected override void DisableState(int previousState) { }

    protected override void OverrideStateProperties(int index)
    {
        ParticlesProfile file = states[index];

        ParticleSystem.EmissionModule emission = rain.emission;
        file.rainEmission = emission.rateOverTime.constant;

        emission = snow.emission;
        file.snowEmission = emission.rateOverTime.constant;
    }
}
