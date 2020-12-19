using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script describes the behaviour of a decal.
/// </summary>
public class RainDecal: MonoBehaviour
{
    /// <summary>
    /// How long until the decal disapears?
    /// </summary>
    [Tooltip("How long until the decal disapears?")]
    public float lifeTime = 1;
    /// <summary>
    /// The progress curve of the scaling over time.
    /// </summary>
    [Tooltip("The progress curve of the scaling over time.")]
    public AnimationCurve scalingCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, Mathf.PI), new Keyframe(1, 1) });
    /// <summary>
    /// The progress curve of the strength over time.
    /// </summary>
    [Tooltip("The progress curve of the strength over time.")]
    public AnimationCurve strengthCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0, -Mathf.PI, 0) });

    /// <summary>
    /// The event when the decal lifetime ends and the gameObject gets disabled.
    /// </summary>
    public UnityAction<RainDecal> OnLifetimeEnd;

    private DecalProjector projector;
    private float maxSize;
    private float timeLived;
    private float currentProgress;
    private float currentSize;
    private Vector3 currentPosition;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        projector = GetComponent<DecalProjector>();
        maxSize = projector.size.x;
    }

    private void OnEnable()
    {
        projector.size = Vector3.zero;
        timeLived = 0;
    }

    private void Update()
    {
        // increment lifetime
        timeLived += Time.deltaTime;
        currentProgress = timeLived / lifeTime;

        // animate the scaling of the decal
        currentSize = scalingCurve.Evaluate(currentProgress) * maxSize;
        projector.size = Vector3.one * currentSize;

        // adjust the position to the waves
        currentPosition = transform.position;
        currentPosition.y = OceanHeightSampler.SampleHeight(gameObject, currentPosition) + currentSize / 2;
        transform.position = currentPosition;

        // adjust the strength
        projector.fadeFactor = strengthCurve.Evaluate(currentProgress);
    }

    private void LateUpdate()
    {
        // disable the object if lifetime is over
        if(timeLived > lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        OnLifetimeEnd?.Invoke(this);
    }
}
