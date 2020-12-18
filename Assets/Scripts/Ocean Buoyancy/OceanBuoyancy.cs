using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanBuoyancy : MonoBehaviour
{
    Rigidbody rigidBody;
    public float buoyancyForce;
    public float depthFactor;
    public float surfaceOffset;
    public List<GameObject> buoyancyElements;

    private void Awake()
    {
        rigidBody = transform.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (GameObject bElement in buoyancyElements) 
        {
            float surfaceHeight = OceanHeightSampler.SampleHeight(bElement, bElement.transform.position) + surfaceOffset;
            float distanceToSurface = surfaceHeight - bElement.transform.position.y;

            if (bElement.transform.position.y <= surfaceHeight)
                rigidBody.AddForceAtPosition(Vector3.up * (buoyancyForce * (depthFactor * distanceToSurface)), bElement.transform.position);

            //if (bElement.transform.position.y > surfaceHeight)
            //{
            //    Debug.Log("Applying gravity force");
            //    rigidBody.AddForceAtPosition(Vector3.down * buoyancyForce * 50f, transform.position);
            //}
        }
    }
}
