using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    [SerializeField]
    private PlayerInput playerInput = null;

    [SerializeField]
    private Animator playerAnimator = null;

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

    public void OnPunch()
    {
        playerAnimator.SetTrigger("Punch");
        Debug.Log("Shots fired!!!");
    }

    public void OnResume()
    {
        Debug.Log("Resuming...");
    }

    public void OnMove(InputValue value)
    {
        Debug.Log("Moving...");
        Vector2 moveDirection = value.Get<Vector2>();
        playerBehaviour.MovePlayer(moveDirection);
        playerAnimator.SetFloat("Speed", 1.0f);
        
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
