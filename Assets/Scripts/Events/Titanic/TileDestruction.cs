using UnityEngine;

public class TileDestruction: MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<TileController>())
        {
            other.GetComponentInParent<TileController>().DamageTile(0, true);
        }
    }
}
