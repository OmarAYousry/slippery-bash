using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBehaviour : MonoBehaviour
{
    private static GameOverBehaviour instance = null;

    [SerializeField]
    private Text gameOverText = null;
    [SerializeField]
    private Text winnerText = null;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public static void CheckEndGameCondition(PlayerBehaviour playerJustDied)
    {
        if (GameController.players.Count <= 2)
        {
            instance.gameObject.SetActive(true);
            instance.StartCoroutine(EndGame(GameController.players.Find(player => playerJustDied != player)));
        }
    }

    public static IEnumerator EndGame(PlayerBehaviour winner)
    {
        if (winner == null)
        {
            // no winner
            instance.winnerText.gameObject.SetActive(false);
            instance.gameOverText.gameObject.SetActive(true);
        }
        else
        {
            // declare winner
            instance.winnerText.text = $"Player {GameController.players.IndexOf(winner) + 1} Wins!";
            instance.winnerText.gameObject.SetActive(true);
            instance.gameOverText.gameObject.SetActive(true);
        }

        // After x seconds, reset to lobby...
        yield return new WaitForSecondsRealtime(5.0f);
        GameController.ResetToTitle();
    }

}
