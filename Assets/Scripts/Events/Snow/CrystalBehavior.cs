using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// [DATE] 2021.02.23
/// 
/// The class sets the spawn and movement behavior of the crystal that comes during the snow event.
/// </summary>
public class CrystalBehavior: MonoBehaviour
{
    public static CrystalBehavior Instance { get; private set; }

    [SerializeField] private ParticleSystem spawnParticles = null;
    [SerializeField] private ParticleSystem destroyParticles = null;

    /// <summary>
    /// Smoothen the movement changes. The higher the value, the slower the changes.
    /// </summary>
    [Tooltip("Smoothen the movement changes. The higher the value, the slower the changes.")]
    public float floatSmooth = 1f;
    /// <summary>
    /// The distance to the ground the crystal wants to keep.
    /// </summary>
    [Tooltip("The distance to the ground the crystal wants to keep.")]
    public float floatOffset = 1;
    /// <summary>
    /// The distance the crystal goes up and down.
    /// </summary>
    [Tooltip("The distance the crystal goes up and down.")]
    public float floatAmplitude = .3f;
    /// <summary>
    /// The speed the crystal goes up and down.
    /// </summary>
    [Tooltip("The speed the crystal goes up and down.")]
    public float floatSpeed = 1;

    private float floatHeight;
    private Vector3 toPos;
    private RaycastHit rayHit;
    private Vector3 currentVelocity;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdatePreferredPosition();
        transform.position = Vector3.SmoothDamp(transform.position, toPos, ref currentVelocity, floatSmooth);
    }

    private void UpdatePreferredPosition()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out rayHit, 100))
        {
            floatHeight = transform.position.y - rayHit.distance;
        }
        floatHeight = Mathf.Max(floatHeight, OceanHeightSampler.SampleHeight(gameObject, transform.position));
        toPos = transform.position;
        toPos.y = floatHeight + floatOffset + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// To be called by the snow event in the beginning.
    /// </summary>
    /// <param name="position">the point where the crystel floats</param>
    public void Spawn(Vector3 position)
    {
        gameObject.SetActive(true);

        position.y = 50;
        transform.position = position;
        UpdatePreferredPosition();
        transform.position = toPos;

        spawnParticles.transform.position = transform.position;
        spawnParticles.Play();
    }

    /// <summary>
    /// To be called by the snow event at the end.
    /// </summary>
    public void Despawn()
    {
        if(!gameObject.activeInHierarchy)
            return;

        spawnParticles.transform.position = transform.position;
        spawnParticles.Play();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// To be called by the player who hits it.
    /// </summary>
    public void Destroy()
    {
        if(!gameObject.activeInHierarchy)
            return;

        destroyParticles.transform.position = transform.position;
        destroyParticles.Play();
        gameObject.SetActive(false);
    }
}
