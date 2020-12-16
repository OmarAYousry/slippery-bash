using UnityEngine;
using UnityEngine.InputSystem;

public class ClickTester : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

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
