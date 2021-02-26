using UnityEngine;
using UnityEngine.UI;

public class StaminaBarBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    [SerializeField]
    private Slider actualStaminaBar = null;

    [SerializeField]
    private Slider maxStaminaDepletionBar = null;

    private readonly float initialMaxStamina = 5.0f;
    private float maxStamina = 5.0f;
    private float currentStamina = 0.0f;

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

        maxStaminaDepletionBar.normalizedValue = 0;

        actualStaminaBar.gameObject.SetActive(true);
    }

    void Update()
    {
        // needed to keep slider facing the camera
        actualStaminaBar.transform.LookAt(GameCamera.Instance.transform);

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

        maxStaminaDepletionBar.normalizedValue = 1 - newRatio;

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
