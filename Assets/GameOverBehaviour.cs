using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Created by Omar
/// Modified by Akbar
/// </summary>
public class GameOverBehaviour : MonoBehaviour
{
    private static GameOverBehaviour instance = null;

    //[SerializeField]
    //private Text gameOverText = null;
    //[SerializeField]
    //private Text winnerText = null;

    [SerializeField] private GameObject gameOverText = null;
    [SerializeField] private GameObject playerWinsText = null;
    [SerializeField] private GameObject[] numberText = null;

    public static bool IsGameOver
    {
        get
        {
            return instance.gameObject.activeInHierarchy;
        }
    }


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public static void CheckEndGameCondition(PlayerBehaviour playerJustDied)
    {
        if (GameController.players.Count <= 1)
        {
            instance.gameObject.SetActive(true);
            instance.StartCoroutine(EndGame(GameController.players.Find(player => playerJustDied != player)));
        }
    }

    public static IEnumerator EndGame(PlayerBehaviour winner)
    {
        // show game over only first before zooming to the winner
        instance.gameOverText.SetActive(true);
        instance.playerWinsText.SetActive(false);
        GameCamera.Instance.State = GameCamera.CameraState.Stop;

        yield return new WaitForSecondsRealtime(3.0f);

        if (winner == null)
        {
            // no winner
            //instance.winnerText.gameObject.SetActive(false);
            //instance.gameOverText.gameObject.SetActive(true);
        }
        else
        {
            // declare winner
            instance.gameOverText.SetActive(false);
            instance.playerWinsText.SetActive(true);

            int index = winner.playerId;
            for(int i = 0; i < instance.numberText.Length; i++)
            {
                instance.numberText[i].SetActive(i == index);
            }

            // move camera to winner
            GameCamera.Instance.State = GameCamera.CameraState.Winner;
            GameCamera.Instance.SetWinnerIndex(0);

            //instance.winnerText.text = $"Player {GameController.players.IndexOf(winner) + 1} Wins!";
            //instance.winnerText.gameObject.SetActive(true);
            //instance.gameOverText.gameObject.SetActive(true);

            // After x seconds, reset to lobby...
            yield return new WaitForSecondsRealtime(5.0f);
        }

        GameController.ResetToTitle();
    }

}
