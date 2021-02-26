using UnityEngine;
using UnityEngine.UI;

public class StaminaBarBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    [SerializeField]
    private Slider actualStaminaBar = null;

    private readonly float initialMaxStamina = 5.0f;
    private float maxStamina = 5.0f;
    private float currentStamina = 0.0f;

    private RectTransform staminaBarRectTransform = null;
    private float initialBarWidth = 0f;

    void Start()
    {
        InitSlider();
    }

    private void InitSlider()
    {
        maxStamina = initialMaxStamina;
        actualStaminaBar.maxValue = maxStamina;
        actualStaminaBar.minValue = 0;

        currentStamina = maxStamina;
        actualStaminaBar.value = currentStamina;

        staminaBarRectTransform = actualStaminaBar.GetComponent<RectTransform>();
        initialBarWidth = staminaBarRectTransform.sizeDelta.x;

        actualStaminaBar.gameObject.SetActive(true);
    }

    void Update()
    {
        // needed to keep slider facing the camera
        actualStaminaBar.transform.LookAt(GameCamera.Instance.transform);

        //AdjustSwimTimer();

        actualStaminaBar.value = currentStamina;
    }

    public void modifyMaxStaminaByRatio(float ratioChange)
    {
        if (LobbyBehaviour.isInLobby)
            return;

        float oldRatio = maxStamina / initialMaxStamina;
        float newRatio = Mathf.Clamp01(oldRatio + ratioChange);


        maxStamina = Mathf.Clamp(initialMaxStamina * newRatio, 0, initialMaxStamina);
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // TODO: Maybe this can have a better look, i.e. a black bar overlaying to be better visualized for players
        staminaBarRectTransform.sizeDelta = new Vector2(initialBarWidth * newRatio, staminaBarRectTransform.sizeDelta.y);

        updateStaminaBar();
    }

    public void modifyStaminaByValue(float staminaChange)
    {
        if (LobbyBehaviour.isInLobby)
            return;

        currentStamina = Mathf.Clamp(currentStamina + staminaChange, 0, maxStamina);
        updateStaminaBar();
    }

    private void updateStaminaBar()
    {
        actualStaminaBar.maxValue = maxStamina;
        actualStaminaBar.value = currentStamina;

        if (currentStamina <= 0)
            playerBehaviour.KillPlayer();
    }

    public void Drown()
    {
        Destroy(actualStaminaBar.transform.parent.gameObject);
        Destroy(this);
    }
}
