using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonBehaviour : MonoBehaviour
{
    private static StartButtonBehaviour instance = null;

    public static bool isInLobby { get { return instance.gameObject.activeSelf; } }


    private void Awake()
    {
        instance = this;
    }

    public void StartGame()
    {
        // start countdown
        Debug.LogError("Starting game...");
        // start game
    }

    public static IEnumerator EnableStartButton(float secondsToWait = 0.0f)
    {
        yield return new WaitForSeconds(secondsToWait);

        instance.gameObject.SetActive(true);
    }    
    
    public static IEnumerator DisableStartButton(float secondsToWait = 0.0f)
    {
        yield return new WaitForSeconds(secondsToWait);

        instance.gameObject.SetActive(false);
    }    
    
    public void OnStartButtonPressed()
    {
        GameController.StartGame();
    }

    public static IEnumerator StartGameCountdown(float timeScale = 1.0f)
    {
        //instance.GetComponent<UnityEngine.UI.Text>().text = "3";
        //yield return new WaitForSeconds(timeScale);        
        
        //instance.GetComponent<UnityEngine.UI.Text>().text = "2";
        //yield return new WaitForSeconds(timeScale);        
        
        //instance.GetComponent<UnityEngine.UI.Text>().text = "1";
        //yield return new WaitForSeconds(timeScale);

        // disable text game object

        instance.gameObject.SetActive(false);

        yield return null;
    }
}
