using System.Collections.Generic;
using UnityEngine;

public class PlatformBound : MonoBehaviour
{
    [SerializeField]
    float pushForceFactor = 7.5f;

    List<GameObject> platformsToPush = new List<GameObject>();

    void OnDrawGizmos()
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;

        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = oldGizmosMatrix;

        DrawGizmoLine(transform.position, transform.forward * transform.localScale.z, new Color(0, 1, 0, 0.25f), 2);
    }

    public static void DrawGizmoLine(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    void LateUpdate()
    {
        foreach(GameObject platform in platformsToPush)
        {
            Rigidbody rb = platform.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * pushForceFactor * rb.mass);
            Debug.Log("pushing " + platform.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            GameObject platform = other.transform.parent.gameObject;
            if (!platformsToPush.Contains(platform))
                platformsToPush.Add(platform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Tile")
        {
            GameObject platform = other.transform.parent.gameObject;
            if (platformsToPush.Contains(platform))
                platformsToPush.Remove(platform);
        }
    }
}
