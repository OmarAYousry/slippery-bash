using UnityEngine;
using UnityEngine.UI;

public class SwimmingBehaviour : MonoBehaviour
{    
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    [SerializeField]
    private Slider strengthBar = null;
    public bool IsSwimming { get; private set; } = false;

    private readonly float drownTime = 10.0f;
    private float swimTimer = 0.0f;

    void Start()
    {
        InitSlider();

        // start out not swimming
        EndSwimming();
    }

    private void InitSlider()
    {
        strengthBar.maxValue = drownTime;
        strengthBar.minValue = swimTimer;
        strengthBar.value = swimTimer;
    }

    void Update()
    {
        // needed to keep slider facing the camera
        strengthBar.transform.rotation = Quaternion.identity;

        AdjustSwimTimer();

        strengthBar.value = swimTimer;
    }

    private void AdjustSwimTimer()
    {
        if (IsSwimming)
        {
            if (swimTimer >= drownTime)
                playerBehaviour.KillPlayer();
            else
                swimTimer += Time.deltaTime;
        }
        else
        {
            swimTimer -= Time.deltaTime;
        }

        swimTimer = Mathf.Clamp(swimTimer, 0.0f, drownTime);
    }

    public void StartSwimming()
    {
        IsSwimming = true;
        strengthBar.gameObject.SetActive(true);
    }

    public void EndSwimming()
    {
        IsSwimming = false;
        strengthBar.gameObject.SetActive(false);
    }
}
