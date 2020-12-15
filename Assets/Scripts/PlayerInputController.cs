using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    public void OnNavigate()
    {
        Debug.Log("Navigating...");
    }

    public void OnSubmit()
    {
        Debug.Log("Submitting...");
        //GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
    }

    public void OnPunch()
    {
        //GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        Debug.Log("Shots fired!!!");
    }

    public void OnResume()
    {
        Debug.Log("Resuming...");
    }

    public void OnJoin()
    {
        Debug.Log("Joining...");
    }

    public void OnLeave()
    {
        Debug.Log("Leaving...");
    }

    public void OnMove(InputValue value)
    {
        Debug.Log("Moving...");
        Vector2 moveDirection = value.Get<Vector2>();
        playerBehaviour.MovePlayer(moveDirection);
    }

    public void OnPause()
    {
        PauseMenuBehaviour.ShowPauseMenu();
    }
}
