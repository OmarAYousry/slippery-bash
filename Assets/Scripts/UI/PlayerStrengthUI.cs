using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// [DATE] 2021.01.19
/// 
/// This script shows the current strength of the player on a slider looking at the camera.
/// </summary>
public class PlayerStrengthUI : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------//
    private void LateUpdate()
    {
        // look at camera
        transform.LookAt(GameCamera.Instance.transform, Vector3.up);

        // value is updated via SwimmingBehaviour
    }
}
