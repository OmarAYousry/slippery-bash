using UnityEngine;

public class ClickTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                TileController tile = hit.collider.gameObject.GetComponent<TileController>();
                if (tile != null)
                {
                    tile.DestroyMesh();
                }
            }
        }
    }
}
