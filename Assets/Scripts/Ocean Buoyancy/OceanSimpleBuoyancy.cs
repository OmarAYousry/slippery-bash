using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanSimpleBuoyancy : MonoBehaviour
{
    Rigidbody rigidBody;
    public float buoyancyForce;
    public float depthFactor;
    public float surfaceOffset;

    private void Awake()
    {
        rigidBody = transform.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float surfaceHeight = OceanHeightSampler.SampleHeight(gameObject, transform.position) + surfaceOffset;
        float distanceToSurface = surfaceHeight - transform.position.y;
        rigidBody.AddForceAtPosition(Vector3.up * (buoyancyForce * (depthFactor * distanceToSurface)), transform.position);
    }
}
