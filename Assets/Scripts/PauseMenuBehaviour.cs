using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBehaviour : MonoBehaviour
{
    private static PauseMenuBehaviour instance = null;

    void Awake()
    {
        instance = this;
        HidePauseMenu();
    }

    void Start()
    {
    }

    public static bool isGamePaused { get { return instance.gameObject.activeSelf; } }

    public static void ShowPauseMenu()
    {
        if (isGamePaused)
            return;

        instance.gameObject.SetActive(true);
    }

    public static void HidePauseMenu()
    {
        instance.OnResumeButtonPressed();
    }

    public void OnResumeButtonPressed()
    {
        if (!isGamePaused)
            return;

        Debug.LogError("Beeeep");

        instance.gameObject.SetActive(false);

        GameController.ResumeGame();
    }


    public void OnEndGameButtonPressed()
    {
        Debug.LogError("Boooop");
        // go back to lobby
        instance.gameObject.SetActive(false);

        GameController.ResetToLobby();
    }
}
