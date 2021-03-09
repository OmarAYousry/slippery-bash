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
    [SerializeField] private MeshRenderer mesh = null;

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

    public Color glowColor;
    public Color noGlowColor;

    private float floatHeight;
    private Vector3 toPos;
    private RaycastHit rayHit;
    private Vector3 currentVelocity;
    private Material material;
    private AudioSource audio;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        Instance = this;
        material = mesh.material;
        audio = transform.parent.GetComponent<AudioSource>();
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
        float sinus = Mathf.Sin(Time.time * floatSpeed);
        toPos = transform.position;
        toPos.y = floatHeight + floatOffset + sinus * floatAmplitude;
        material.SetColor("_EmissiveColor", Color.Lerp(noGlowColor, glowColor * 10, sinus / 2 + .5f));
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

        AudioController.PlaySoundEffect(SoundEffectType.CRYSTAL_SPAWN);
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

        AudioController.PlaySoundEffect(SoundEffectType.CRYSTAL_SPAWN);
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

        AudioController.PlaySoundEffect(SoundEffectType.CRYSTAL_DESTROY);
    }
}
