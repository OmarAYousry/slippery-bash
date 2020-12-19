using Crest;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script manages all the decal objects and place them accordingly to the hit points of the rain particle system.
/// </summary>
public class RainDecalManager : MonoBehaviour
{
    [SerializeField] private GameObject decalPrefab = null;
    [SerializeField] private ParticleSystem rain = null;

    /// <summary>
    /// The amount of objects in the pool.
    /// </summary>
    [Tooltip("The amount of objects in the pool.")]
    public byte maxDecals = byte.MaxValue;
    /// <summary>
    /// Skip updates of checking the particles to save some performance.
    /// </summary>
    [Tooltip("Skip updates of checking the particles to save some performance.")]
    public int skipUpdates = 0;
    /// <summary>
    /// The range in which the particle spawns decals. (too small: no spawn; too big: too many spawns)
    /// </summary>
    [Tooltip("The range in which the particle spawns decals. (too small: no spawn; too big: too many spawns)")]
    public float spawnHeight = .1f;

    private ParticleSystem.Particle[] particles;
    private List<RainDecal> inactiveDecals;
    private int amountOfActiveParticles;

    private ParticleSystem.Particle currentParticle;
    private Vector3 currentParticlePosition;
    private RainDecal currentDecal;
    private Vector4 oceanRange; // [min, max, seaLevel, maxDisp]
    private float oceanHeight;


    //---------------------------------------------------------------------------------------------//
    private void Start()
    {
        particles = new ParticleSystem.Particle[rain.main.maxParticles];
        inactiveDecals = new List<RainDecal>();

        for(int i = 0; i < maxDecals; i++)
        {
            currentDecal = Instantiate(decalPrefab, transform).GetComponent<RainDecal>();
            OceanHeightSampler.SampleHeight(currentDecal.gameObject, Vector3.zero);   // prepare the sampler for potential calculations
            currentDecal.gameObject.SetActive(false);
            inactiveDecals.Add(currentDecal);
        }
    }

    private void Update()
    {
        SpawnDecal();
    }

    private void SpawnDecal()
    {
        // check first if a decal is available before spawning new ones
        if(inactiveDecals.Count > 0)
        {
            // get the first available decal
            currentDecal = inactiveDecals[0];

            // Get all active particles from the particle system.
            amountOfActiveParticles = rain.GetParticles(particles);

            // get the current range of the ocean
            oceanRange.z = OceanRenderer.Instance.SeaLevel;
            oceanRange.w = OceanRenderer.Instance.MaxVertDisplacement;
            oceanRange.x = oceanRange.z - oceanRange.w;
            oceanRange.y = oceanRange.z + oceanRange.w;

            // check all the particles if they're close to the water surface
            for(int i = 0; i < amountOfActiveParticles; i++)
            {
                currentParticle = particles[i];

                currentParticlePosition = currentParticle.position;
                // only sample when the particle is close
                if(currentParticlePosition.y > oceanRange.x && currentParticlePosition.y < oceanRange.y)
                {
                    oceanHeight = OceanHeightSampler.SampleHeight(currentDecal.gameObject, currentParticlePosition);

                    // spawn only in the specified range
                    if(currentParticlePosition.y > oceanHeight - spawnHeight && currentParticlePosition.y < oceanHeight)
                    {
                        // spawn the decal accordingly and set the listener for proper disabling
                        currentDecal.transform.position = currentParticlePosition;
                        currentDecal.OnLifetimeEnd += SetDecalToInactive;
                        currentDecal.gameObject.SetActive(true);
                        // update the list
                        inactiveDecals.RemoveAt(0);

                        // check if more decals can be spawned
                        if(inactiveDecals.Count == 0)
                        {
                            return;
                        }

                        // other wise increment the current decal
                        currentDecal = inactiveDecals[0];
                    }
                }
            }
        }
    }

    private void SetDecalToInactive(RainDecal decal)
    {
        decal.OnLifetimeEnd -= SetDecalToInactive;

        // update the list
        inactiveDecals.Add(decal);
    }
}
