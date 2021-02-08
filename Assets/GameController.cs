using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by Omar
/// Modified by Akbar
/// </summary>
public class GameController : MonoBehaviour
{
    private static GameController instance = null;
    public static List<PlayerBehaviour> players = null;
    

    private void Awake()
    {
        instance = this;
        players = new List<PlayerBehaviour>();
        Time.timeScale = 1.0f;
    }

    public static int RegisterPlayer(PlayerBehaviour playerToRegister)
    {
        if (players.Count == 0)
            StartLobby();

        players.Add(playerToRegister);

        // return player id (player 1, id: 0, etc.)
        return players.Count - 1;
    }

    public static void DeregisterPlayer(PlayerBehaviour playerToDeregister)
    {
        players.Remove(playerToDeregister);
    }

    public static void CheckEndGameCondition(PlayerBehaviour playerJustDied)
    {
        GameOverBehaviour.CheckEndGameCondition(playerJustDied);
    }

    public static void StartLobby()
    {
        //TitleBehaviour.DisableTitleScreen();
        instance.StartCoroutine(LobbyBehaviour.EnableLobbyUI(secondsToWait: 3.0f));
        GameCamera.Instance.State = GameCamera.CameraState.Lobby;
    }

    public static void StartCountdown()
    {
        //instance.StartCoroutine(LobbyBehaviour.DisableLobbyUI());
        //instance.StartCoroutine(LobbyBehaviour.StartGameCountdown());
        CountdownUI.Instance.OnCountdownFinished += StartGame;
        CountdownUI.Instance.PlayCountdown();
        GameCamera.Instance.State = GameCamera.CameraState.InGame;
    }
    private static void StartGame()
    {
        CountdownUI.Instance.OnCountdownFinished -= StartGame;
        EventStateController.Instance.Play();
        LobbyBehaviour.Instance.gameWall.SetActive(false);
        LobbyBehaviour.Instance.gameObject.SetActive(false);
    }

    public static void PauseGame()
    {
        Debug.LogError(LobbyBehaviour.isInLobby);
        if (PauseMenuBehaviour.isGamePaused || LobbyBehaviour.isInLobby)
        {
            return;
        }
        Debug.LogError("here...");

        Time.timeScale = 0.0f;
        foreach (PlayerBehaviour player in players)
        {
            // TODO: Change Control Scheme to UI
            player.ChangeInputToUI();
        }
        Debug.LogError("here...2");

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

    public static void ResetToTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
