using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBehaviour : MonoBehaviour
{
    private static PauseMenuBehaviour instance = null;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        HidePauseMenu();
    }

    public static bool isGamePaused { get { return instance.gameObject.activeSelf; } }

    public static void ShowPauseMenu()
    {
        if (isGamePaused)
            return;

        instance.gameObject.SetActive(true);
    }


    public void HidePauseMenu()
    {
        if (!isGamePaused)
            return;

        Debug.LogError("Beeeep");

        instance.gameObject.SetActive(false);
    }

    public void EndGame()
    {
        Debug.LogError("Boooop");
        // go back to lobby
    }
}
