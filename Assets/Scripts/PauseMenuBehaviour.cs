using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuBehaviour : MonoBehaviour
{
    private static PauseMenuBehaviour instance = null;

    public Button resumeButton;

    void Awake()
    {
        instance = this;
        HidePauseMenu();
    }

    public static bool isGamePaused { get { return instance.gameObject.activeSelf; } }

    public static void ShowPauseMenu()
    {
        Debug.LogError(isGamePaused);
        if (isGamePaused)
            return;

        instance.gameObject.SetActive(true);
        instance.resumeButton.Select();

        Debug.LogError("????");
    }

    public static void HidePauseMenu()
    {
        Debug.LogError("???");
        instance.gameObject.SetActive(false);
        //instance.OnResumeButtonPressed();
    }

    public void OnResumeButtonPressed()
    {
        if (!isGamePaused)
            return;

        GameController.ResumeGame();
    }


    public void OnEndGameButtonPressed()
    {
        instance.gameObject.SetActive(false);

        GameController.ResetToTitle();
    }
}
