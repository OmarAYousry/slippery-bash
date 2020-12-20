using UnityEngine;

public class OceanHeightHelperExample: MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x;
        float y = OceanHeightSampler.SampleHeight(gameObject, transform.position);
        float z = transform.position.z;
        transform.position = new Vector3(x, y, z);
    }
}
