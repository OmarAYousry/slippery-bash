using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script adds foam to the object if it's close to the water surface.
/// </summary>
public class FoamObject : MonoBehaviour
{
    public Vector2 blend = new Vector2(1, 2);
    public Renderer rend;

    private Material mat;
    private float distance;
    private float blendRange;
    private Vector3 idleScale;

    private void Start()
    {
        mat = rend.material;
        blendRange = blend.y - blend.x;
        idleScale = transform.localScale;
    }

    private void Update()
    {
        distance = OceanHeightSampler.SampleHeight(gameObject, transform.position) - transform.position.y;
        if(distance < 0)
        {
            mat.SetFloat("_Strength", 1 - Mathf.Clamp01((-distance - blend.x) / blendRange));
        }
        else
        {
            mat.SetFloat("_Strength", 1 - Mathf.Clamp01((distance - blend.x) / blendRange));
        }
        transform.localScale = Vector3.Lerp(idleScale, Vector3.zero, Mathf.Abs(distance / blendRange));
    }
}
