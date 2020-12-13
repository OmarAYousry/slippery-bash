using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public void OnNavigate()
    {
        Debug.Log("Navigating...");
    }

    public void OnSubmit()
    {
        Debug.Log("Submitting...");
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
    }

    public void OnPunch()
    {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        Debug.Log("Shots fired!!!");
    }

    public void OnPause()
    {
        Debug.Log("Pausing...");
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
        Debug.Log("Moving..." + value.Get<Vector2>());
    }
}
