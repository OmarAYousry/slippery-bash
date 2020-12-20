using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script moves all particles of the lightning system to the position of this transform.
/// </summary>
public class LightningBehavior: MonoBehaviour
{
    #region PreRibbons
    [Header("Pre Ribbons")]
    [SerializeField] public ParticleSystem preRibbon0;
    [SerializeField] public ParticleSystem preRibbon1;

    /// <summary>
    /// The duration in seconds before the lightning falls.
    /// </summary>
    [Tooltip("The duration in seconds before the lightning falls.")]
    public float ribbonTime = 1;
    /// <summary>
    /// The time ratio in which the second ribbon activates.
    /// </summary>
    [Tooltip("The time ratio in which the second ribbon activates.")]
    [Range(0,1)] public float ribbonSecondPhase = .5f;

    private float ribbonTimer = -1;
    private ParticleSystem.EmissionModule emission;
    private float secondPhase;
    #endregion

    #region Lightning Fall
    [Header("Lighting Fall")]
    [SerializeField] public ParticleSystem lightning;

    /// <summary>
    /// The strength curve for redirecting the particle towards the destination over lifetime.
    /// </summary>
    [Tooltip("The strength curve for redirecting the particle towards the destination over lifetime.")]
    public AnimationCurve destinationStrength = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1, Mathf.PI, 0) });
    /// <summary>
    /// The strength curve for redirecting the particle towards the center of all particles over lifetime.
    /// </summary>
    [Tooltip("The strength curve for redirecting the particle towards the center of all particles over lifetime.")]
    public AnimationCurve gatherStrength = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, Mathf.PI), new Keyframe(1, 1) });
    /// <summary>
    /// Amount of single lightning trails that is emitted by the particle system.
    /// </summary>
    [Tooltip("Amount of single lightning trails that is emitted by the particle system.")]
    public int amount = 10;

    private float lightningTimer = -1;
    private ParticleSystem.Particle[] particles;
    private int amountOfParticles;
    private Vector3 gatherPosition;
    private Vector3 toPosition;
    #endregion

    #region Strike
    [Header("Strike")]
    [SerializeField] public LineRenderer strike;
    [SerializeField] public HDAdditionalLightData strikeLight;
    [SerializeField] public ParticleSystem strikeRibbon;
    [SerializeField] public Collider hitCollider;

    /// <summary>
    /// The duration of the strike until it disappears in seconds.
    /// </summary>
    [Tooltip("The duration of the strike until it disappears in seconds.")]
    public float strikeTime = 2;
    /// <summary>
    /// The vertical spread of segment points of the line renderer.
    /// </summary>
    [Tooltip("The vertical spread of segment points of the line renderer.")]
    public AnimationCurve segmentVerticalDistribution = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1, Mathf.PI, 0) });
    /// <summary>
    /// The horizontal offset from the origin of the segment points of the line renderer.
    /// </summary>
    [Tooltip("The horizontal offset from the origin of the segment points of the line renderer.")]
    public AnimationCurve offsetStrength = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    /// <summary>
    /// The maximum horizontal offset of a segment point to the origin.
    /// </summary>
    [Tooltip("The maximum horizontal offset of a segment point to the origin.")]
    public float maxOffset = 10;
    /// <summary>
    /// The curve of the strike fading away.
    /// </summary>
    [Tooltip("The curve of the strike fading away.")]
    public AnimationCurve falloffCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0, -Mathf.PI, 0) });
    /// <summary>
    /// The time ratio of the strike in which the players can get hit.
    /// </summary>
    [Tooltip("The time ratio of the strike in which the players can get hit.")]
    [Range(0,1)] public float hitDuration = .5f;

    private float strikeTimer = -1;
    private Vector3[] strikePoints;
    private float distance;
    private Material strikeMat;
    private float maxIntensity;
    private float maxLightIntensity;
    private float hitDisableTime;
    #endregion


    private Vector3 currentPosition;
    private float currentCurveProgress;
    private float updatingValue;


    //---------------------------------------------------------------------------------------------//
    private void Start()
    {
        // pre ribbon
        secondPhase = ribbonTime * ribbonSecondPhase;

        // lightning fall
        particles = new ParticleSystem.Particle[lightning.main.maxParticles];
        hitDisableTime = strikeTime * hitDuration;

        // strike
        strikePoints = new Vector3[strike.positionCount];
        distance = lightning.transform.localPosition.y;
        strikeMat = strike.material;
        maxIntensity = strikeMat.GetFloat("_EmissiveIntensity");
        maxLightIntensity = strikeLight.intensity;
    }

    private void Update()
    {
        if(ribbonTimer >= 0)
        {
            ribbonTimer += Time.deltaTime;

            // adjust the position to the ocean during the wait
            AdjustToOcean();

            // enable the second phase if ready and not yet done
            emission = preRibbon1.emission;
            if(!emission.enabled && ribbonTimer > secondPhase)
            {
                emission.enabled = true;
                preRibbon1.Play();
            }

            if(ribbonTimer >= ribbonTime)
            {
                ribbonTimer = -1;
                emission = preRibbon0.emission;
                emission.enabled = false;
                emission = preRibbon1.emission;
                emission.enabled = false;
                Fall();
            }
        }
        else if(lightningTimer >= 0)
        {
            lightningTimer += Time.deltaTime;

            // adjust the position to the ocean during the falling
            AdjustToOcean();

            // check if falling has reached (or Reach has fallen lol)
            if(lightningTimer >= lightning.main.startLifetime.constant)
            {
                lightningTimer = -1;
                Strike();
            }
        }
        else if(strikeTimer >= 0)
        {
            strikeTimer += Time.deltaTime;

            // animate the strike fade
            updatingValue = falloffCurve.Evaluate(strikeTimer / strikeTime);
            strikeMat.SetColor("_UnlitColor", Color.white * updatingValue);
            strikeMat.SetFloat("_EmissiveIntensity", maxIntensity * updatingValue);
            strikeLight.intensity = maxLightIntensity * updatingValue;

            // disable hit if done
            if(hitCollider.enabled && strikeTimer >= hitDisableTime)
            {
                hitCollider.enabled = false;
            }

            if(strikeTimer >= strikeTime)
            {
                strikeTimer = -1;
                strike.gameObject.SetActive(false);
                strikeLight.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }

    private void AdjustToOcean()
    {
        currentPosition = transform.position;
        updatingValue = OceanHeightSampler.SampleHeight(gameObject, currentPosition);
        currentPosition.y = updatingValue;
        transform.position = currentPosition;
    }

    private void LateUpdate()
    {
        amountOfParticles = lightning.GetParticles(particles);

        // only move if there is anything to move
        if(amountOfParticles > 0)
        {
            // first get the center position of all particles to gather them
            gatherPosition = Vector3.zero;
            for(int i = 0; i < amountOfParticles; i++)
            {
                gatherPosition += particles[i].position;
            }
            gatherPosition /= amountOfParticles;

            // move the particles accordingly
            for(int i = 0; i < amountOfParticles; i++)
            {
                currentCurveProgress = 1 - particles[i].remainingLifetime / particles[i].startLifetime;
                toPosition = Vector3.Lerp(
                    particles[i].position,
                    gatherPosition,
                    gatherStrength.Evaluate(currentCurveProgress));
                particles[i].position = Vector3.Lerp(
                    toPosition,
                    transform.position,
                    destinationStrength.Evaluate(currentCurveProgress));
            }
            lightning.SetParticles(particles, amountOfParticles);
        }
    }

    private void Fall()
    {
        // emit the lightnings
        lightning.Emit(amount);
        lightningTimer = 0;
    }

    private void Strike()
    {
        strike.gameObject.SetActive(true);
        strikeLight.gameObject.SetActive(true);
        strikeRibbon.Play();
        hitCollider.enabled = true;

        // spread the segment points
        for(int i = 0; i < strikePoints.Length; i++)
        {
            currentCurveProgress = i / (strikePoints.Length - 1f);
            strikePoints[i] = Random.insideUnitCircle * offsetStrength.Evaluate(currentCurveProgress) * maxOffset;
            strikePoints[i].z = strikePoints[i].y;
            strikePoints[i].y = segmentVerticalDistribution.Evaluate(currentCurveProgress) * distance;
        }
        strike.SetPositions(strikePoints);

        strikeTimer = 0;
    }


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Let a lightning strike onto the specified position.
    /// </summary>
    /// <param name="position">the point the lightning strikes to</param>
    public void SetLightning(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;

        emission = preRibbon0.emission;
        emission.enabled = true;
        preRibbon0.Play();
        ribbonTimer = 0;
    }
}
