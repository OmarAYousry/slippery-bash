using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    [SerializeField]
    private PlayerInput playerInput = null;

    public void OnNavigate()
    {
        Debug.Log("Navigating...");
    }

    public void OnCancel()
    {
        GameController.ResumeGame();
    }

    public void OnSubmit()
    {
        Debug.Log("Submitting...");
    }

    public void OnJump()
    {
        playerBehaviour.PerformJump();
    }

    public void OnPunch()
    {
        playerBehaviour.PerformPunch();
        Debug.Log("Shots fired!!!");
    }

    public void OnResume()
    {
        Debug.Log("Resuming...");
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

    public void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }
}
