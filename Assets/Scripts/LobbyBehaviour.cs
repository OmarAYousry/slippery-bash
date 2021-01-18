using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// Created by Omar
/// Modified by Akbar
/// </summary>
public class LobbyBehaviour : MonoBehaviour
{
    private static LobbyBehaviour instance = null;
    public static LobbyBehaviour Instance { get { return instance; } }

    [SerializeField]
    private Button startGameButton = null;

    [SerializeField]
    private GameObject tutorialInfo = null;

    //[SerializeField]
    //private Text countdownText = null;

    //[SerializeField]
    //private Text countdownTimerText = null;

    public static bool isInLobby { get { return instance.gameObject.activeSelf; } }

    private void Awake()
    {
        instance = this;
        //instance.gameObject.SetActive(false);
    }

    public static IEnumerator EnableLobbyUI(float secondsToWait = 0.0f)
    {
        instance.gameObject.SetActive(true);
        instance.startGameButton.interactable = false;
        instance.anim.Animate(true);

        yield return new WaitForSeconds(secondsToWait);

        instance.startGameButton.interactable = true;
    }    
    
    public static IEnumerator DisableLobbyUI(float secondsToWait = 0.0f)
    {
        yield return new WaitForSeconds(secondsToWait);

        instance.gameObject.SetActive(false);
    }    
    
    public void OnStartGameButtonPressed()
    {
        startGameButton.interactable = false;
        anim.Animate(false);
        GameController.StartCountdown();
    }

    public static void virtualStartGameButtonClick()
    {
        if (instance.startGameButton.IsInteractable())
            instance.startGameButton.onClick.Invoke();
    }

    // is triggered in the game controller and handled in the countdown ui
    //public static IEnumerator StartGameCountdown(float timeScale = 1.0f)
    //{
    //    instance.countdownText.gameObject.SetActive(true);
    //    instance.countdownTimerText.gameObject.SetActive(true);

    //    instance.countdownTimerText.text = "3";
    //    yield return new WaitForSeconds(timeScale);

    //    instance.countdownTimerText.text = "2";
    //    yield return new WaitForSeconds(timeScale);

    //    instance.countdownTimerText.text = "1";
    //    yield return new WaitForSeconds(timeScale);

    //    instance.countdownText.gameObject.SetActive(false);

    //    instance.countdownTimerText.text = "GO!!!";
    //    yield return new WaitForSeconds(timeScale);

    //    instance.countdownTimerText.gameObject.SetActive(false);


    //    instance.gameObject.SetActive(false);

    //    yield return null;
    //}


    // added by Akbar Suriaganda
    //-----------------------------------------------------------------------------------------------------------------------------------------------//
    [SerializeField] private TextMeshProUGUI objective = null;
    [SerializeField] private PlayerJoinUI[] playerUI = null;
    [SerializeField] private MainMenuAnimation anim = null;

    private int playerJoined;


    //---------------------------------------------------------------------------------------------//
    private void Update()
    {
        UpdateObjectiveAndButton();
    }

    private void UpdateObjectiveAndButton()
    {
        playerJoined = GameController.players.Count;
        startGameButton.interactable = playerJoined > 0;

        for(int i = 0; i < playerUI.Length; i++)
        {
            playerUI[i].ToggleJoin(i < playerJoined, i);
        }

        switch(playerJoined)
        {
            case 0: objective.text = "NO PLAYER JOINED";
                break;
            case 1: objective.text = "SURVIVAL!";
                break;
            default: objective.text = "LAST MAN STANDING!";
                break;
        }
    }


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Leave the game and close the app.
    /// </summary>
    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
