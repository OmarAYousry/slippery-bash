using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// [DATE] 2021.01.19
/// 
/// This script handles the behavior of the camera throughout the game loop.
/// </summary>
public class GameCamera: MonoBehaviour
{
    public static GameCamera Instance { get; private set; }
    public enum CameraState { Stop, Lobby, InGame, Winner }

    /// <summary>
    /// The angle the camera looks down in radiants. PI/2: straight down
    /// </summary>
    [Tooltip("The angle the camera looks down in radiants. PI/2: straight down")]
    [Range(0f, Mathf.PI / 2f)]
    public float angle = Mathf.PI / 2f;
    /// <summary>
    /// The distance to the stage during lobby.
    /// </summary>
    [Tooltip("The distance to the stage during lobby.")]
    public float lobbyDistance = 50f;
    /// <summary>
    /// The x offset in the lobby.
    /// </summary>
    [Tooltip("The x offset in the lobby.")]
    public float lobbyCenterOffset = 10f;
    /// <summary>
    /// The camera zoom is clamped to a range during ingame.
    /// </summary>
    [Tooltip("The camera zoom is clamped to a range during ingame.")]
    public Vector2 distanceRange = new Vector2(10f, 50f);
    /// <summary>
    /// The distance while focusing on the winner.
    /// </summary>
    [Tooltip("The distance while focusing on the winner.")]
    public float winnerDistance = 5f;
    /// <summary>
    /// Additional radius will be added to the max position so the outer object is completely visible.
    /// </summary>
    [Tooltip("Additional radius will be added to the max position so the outer object is completely visible.")]
    public float borderMargin = 5f;
    /// <summary>
    /// Speed of the camera movement.
    /// </summary>
    [Tooltip("Speed of the camera movement.")]
    public float smoothTime = 1f;

    private float desiredDistance;
    private float desiredOffset;
    private Vector3 desiredPosition;

    private Vector3 velocity;
    private Bounds bounds = new Bounds();
    private bool firstFocusSet;
    private List<Vector3> lightningObjects = new List<Vector3>();
    private CameraState state = CameraState.Lobby;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        Instance = this;
        State = CameraState.Lobby;
    }

    private void LateUpdate()
    {
        if(state == CameraState.Stop)
            return;

        switch(state)
        {
            case CameraState.Stop:
                break;
            case CameraState.Lobby:
                SetLobbyValues();
                break;
            case CameraState.InGame:
                SetGameValues();
                break;
            case CameraState.Winner:
                SetWinnerValues();
                break;
        }

        desiredPosition = bounds.center
            + desiredDistance * (Vector3.back * Mathf.Cos(angle) + Vector3.up * Mathf.Sin(angle))
            + desiredOffset * Vector3.right;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        transform.localEulerAngles = Vector3.right * angle * Mathf.Rad2Deg;
    }

    private void SetLobbyValues()
    {
        desiredDistance = lobbyDistance;
        desiredOffset = lobbyCenterOffset;
    }

    private void SetGameValues()
    {
        bounds.center = Vector3.zero;
        bounds.extents = Vector3.zero;
        firstFocusSet = false;

        if(GameController.players != null)
        {
            for(int i = 0; i < GameController.players.Count; i++)
            {
                // check if the player hasn't despawned yet (not expected)
                if(GameController.players[i])
                {
                    if(!firstFocusSet)
                    {
                        bounds.center = GameController.players[i].transform.position;
                        firstFocusSet = true;
                    }
                    else
                    {
                        bounds.Encapsulate(GameController.players[i].transform.position);
                    }
                }
            }
        }

        if(TitanicBehavior.Instance && TitanicBehavior.Instance.gameObject.activeInHierarchy)
        {
            if(!firstFocusSet)
            {
                bounds.center = TitanicBehavior.Instance.transform.position;
                firstFocusSet = true;
            }
            else
            {
                bounds.Encapsulate(TitanicBehavior.Instance.transform.position);
            }
        }

        lightningObjects = StormBehavior.GetLightningPositions();
        if(lightningObjects != null)
        {
            for(int i = 0; i < lightningObjects.Count; i++)
            {
                if(!firstFocusSet)
                {
                    bounds.center = lightningObjects[i];
                    firstFocusSet = true;
                }
                else
                {
                    bounds.Encapsulate(lightningObjects[i]);
                }
            }
        }

        desiredDistance = Mathf.Max(bounds.size.x, bounds.size.z) + borderMargin;
    }

    private void SetWinnerValues()
    {
        desiredDistance = winnerDistance;
        bounds.center = GameController.players[0].transform.position;
    }


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// The behavior of the camera depends on the current game state.
    /// </summary>
    public CameraState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
            switch(state)
            {
                case CameraState.Stop:
                    break;
                case CameraState.Lobby:
                    bounds.center = Vector3.zero;
                    bounds.extents = Vector3.zero;
                    break;
                case CameraState.InGame:
                    desiredOffset = 0f;
                    bounds.center = Vector3.zero;
                    bounds.extents = Vector3.zero;
                    break;
                case CameraState.Winner:
                    desiredOffset = 0f;
                    bounds.center = Vector3.zero;
                    bounds.extents = Vector3.zero;
                    break;
            }
        }
    }

    /// <summary>
    /// Get the current bounds that the camera is dependent on.
    /// </summary>
    /// <returns>bounds encapsulating all importand objects</returns>
    public Bounds GetBounds()
    {
        return bounds;
    }

    /// <summary>
    /// Get the current distance dependent on the current bounds.
    /// </summary>
    /// <returns>distance of the camera to the focus point</returns>
    public float GetDistance()
    {
        return desiredDistance;
    }


    //---------------------------------------------------------------------------------------------//
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
