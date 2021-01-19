using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// [DATE] 2021.01.19
/// 
/// This script handles the behavior of the camera throughout the game loop.
/// </summary>
public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance { get; private set; }
    // TODO: handle the behavior


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        Instance = this;
    }
}
