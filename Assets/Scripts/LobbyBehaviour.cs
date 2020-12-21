using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBehaviour : MonoBehaviour
{
    private static LobbyBehaviour instance = null;

    [SerializeField]
    private Button startGameButton = null;

    [SerializeField]
    private GameObject tutorialInfo = null;

    [SerializeField]
    private Text countdownText = null;

    [SerializeField]
    private Text countdownTimerText = null;

    public static bool isInLobby { get { return instance.gameObject.activeSelf; } }

    private void Awake()
    {
        instance = this;
        instance.gameObject.SetActive(false);
    }

    public static IEnumerator EnableLobbyUI(float secondsToWait = 0.0f)
    {
        instance.gameObject.SetActive(true);
        instance.startGameButton.interactable = false;

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
        GameController.StartGame();
    }

    public static void virtualStartGameButtonClick()
    {
        if (instance.startGameButton.IsInteractable())
            instance.startGameButton.onClick.Invoke();
    }

    public static IEnumerator StartGameCountdown(float timeScale = 1.0f)
    {
        instance.countdownText.gameObject.SetActive(true);
        instance.countdownTimerText.gameObject.SetActive(true);

        instance.countdownTimerText.text = "3";
        yield return new WaitForSeconds(timeScale);

        instance.countdownTimerText.text = "2";
        yield return new WaitForSeconds(timeScale);

        instance.countdownTimerText.text = "1";
        yield return new WaitForSeconds(timeScale);

        instance.countdownText.gameObject.SetActive(false);

        instance.countdownTimerText.text = "GO!!!";
        yield return new WaitForSeconds(timeScale);

        instance.countdownTimerText.gameObject.SetActive(false);

        EventStateController.Instance.Play();

        instance.gameObject.SetActive(false);

        yield return null;
    }
}
