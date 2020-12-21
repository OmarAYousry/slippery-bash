using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance = null;
    public static List<PlayerBehaviour> players = null;
    

    private void Awake()
    {
        instance = this;
        players = new List<PlayerBehaviour>();
    }

    public static void RegisterPlayer(PlayerBehaviour playerToRegister)
    {
        if (players.Count == 0)
            StartLobby();

        players.Add(playerToRegister);
    }

    public static void StartLobby()
    {
        TitleBehaviour.DisableTitleScreen();
        instance.StartCoroutine(LobbyBehaviour.EnableLobbyUI(secondsToWait: 3.0f));
    }

    public static void StartGame()
    {
        //instance.StartCoroutine(LobbyBehaviour.DisableLobbyUI());
        instance.StartCoroutine(LobbyBehaviour.StartGameCountdown());
    }

    public static void PauseGame()
    {
        if (PauseMenuBehaviour.isGamePaused || LobbyBehaviour.isInLobby)
        {
            return;
        }

        Time.timeScale = 0.0f;
        foreach (PlayerBehaviour player in players)
        {
            // TODO: Change Control Scheme to UI
            player.ChangeInputToUI();
        }

        PauseMenuBehaviour.ShowPauseMenu();
    }

    public static void ResumeGame()
    {
        if (!PauseMenuBehaviour.isGamePaused)
        {
            return;
        }


        Time.timeScale = 1.0f;
        foreach (PlayerBehaviour player in players)
        {
            // TODO: Change Control Scheme from UI
            player.ChangeInputToGame();
        }

        PauseMenuBehaviour.HidePauseMenu();
    }

    public static void EndGame(PlayerBehaviour winner)
    {
        if (winner == null)
        {
            // no winner
        }
        else
        {
            // declare winner
        }

        // After x seconds, reset to lobby...
    }

    public static void ResetToLobby()
    {

    }
}
