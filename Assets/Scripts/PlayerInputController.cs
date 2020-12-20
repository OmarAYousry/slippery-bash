using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    [SerializeField]
    private PlayerInput playerInput = null;

    public void OnCancel()
    {
        GameController.ResumeGame();
    }

    public void OnJump()
    {
        playerBehaviour.PerformJump();
    }

    public void OnPunch()
    {
        playerBehaviour.PerformPunch();
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveDirection = value.Get<Vector2>();
        playerBehaviour.UpdateSpeed(moveDirection);
    }

    public void OnPause()
    {
        GameController.PauseGame();
    }

    public void OnLobbyStart()
    {
        LobbyBehaviour.virtualStartGameButtonClick();
    }

    public void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }
}
