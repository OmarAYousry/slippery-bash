using Crest;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This helper simplifies the SampleHeightHelper, so all objects don't need to have an own sampler.
/// </summary>
public class OceanHeightSampler : MonoBehaviour
{
    private static Vector3[] positions = new Vector3[0];
    private static Vector3[] results = new Vector3[0];
    private static Dictionary<GameObject, int> dict = new Dictionary<GameObject, int>();

    /// <summary>
    /// Update the position of the object in the query and get the water height at that position.
    /// The first query is accurate on frame.
    /// After that, it returns the height of the previous frame.
    /// </summary>
    /// <param name="reference">the requesting gameObject for keeping reference</param>
    /// <param name="position">the position of the asking gameObject</param>
    /// <returns>the height of the water world space</returns>
    public static float SampleHeight(GameObject reference, Vector3 position)
    {
        if(!dict.ContainsKey(reference))
        {
            dict.Add(reference, positions.Length);
            AddToArray(ref positions, position);

            float firstHeight = 0;
            ICollProvider collProvider = OceanRenderer.Instance?.CollisionProvider;
            if(collProvider == null)
            {
                return firstHeight;
            }

            Vector3[] queryPos = new Vector3[1];
            Vector3[] queryResult = new Vector3[1];
            queryPos[0] = position;
            queryResult[0] = Vector3.zero;
            var status = collProvider.Query(reference.GetHashCode(), 0, queryPos, queryResult, null, null);

            if(!collProvider.RetrieveSucceeded(status))
            {
                AddToArray(ref results, Vector3.up * OceanRenderer.Instance.SeaLevel);
                return OceanRenderer.Instance.SeaLevel;
            }

            firstHeight = queryResult[0].y + OceanRenderer.Instance.SeaLevel;
            AddToArray(ref results, queryResult[0]);
            return firstHeight;
        }
        else
        {
            int index = dict[reference];
            positions[index] = position;
            return results[index].y + OceanRenderer.Instance.SeaLevel;
        }
    }

    private static void AddToArray(ref Vector3[] array, Vector3 value)
    {
        Vector3[] temp = new Vector3[array.Length + 1];
        for(int i = 0; i < array.Length; i++)
        {
            temp[i] = array[i];
        }
        temp[temp.Length - 1] = value;
        array = temp;
    }

    //public Vector3[] positionDebug;
    //public Vector3[] resultDebug;

    private void FixedUpdate()
    {
        //positionDebug = positions;
        //resultDebug = results;

        ICollProvider collProvider = OceanRenderer.Instance?.CollisionProvider;
        if(collProvider == null)
            return;

        collProvider.Query(GetHashCode(), 0, positions, results, null, null);
    }
}
