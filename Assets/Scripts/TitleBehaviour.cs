using UnityEngine;

public class TitleBehaviour : MonoBehaviour
{
    private static TitleBehaviour instance = null;

    private void Awake()
    {
        gameObject.SetActive(true);
        instance = this;
    }


    public static void PrepareGameLobby()
    {
        
    }

    public static void DisableTitleScreen()
    {
        instance.gameObject.SetActive(false);
    }
}
