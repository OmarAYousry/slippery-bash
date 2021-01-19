using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwimmingBehaviour : MonoBehaviour
{    
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;

    [SerializeField]
    private Animator playerAnimator = null;

    [SerializeField]
    private Rigidbody playerRigidBody;
    [SerializeField]
    private float buoyancyForce;
    [SerializeField]
    private float depthFactor;
    [SerializeField]
    private float surfaceOffset;

    [SerializeField]
    private Slider strengthBar = null;

    [SerializeField]
    private PhysicMaterial playerWalkingMaterial;

    [SerializeField]
    private PhysicMaterial playerSwimmingMaterial;

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
        strengthBar.transform.LookAt(GameCamera.Instance.transform);

        AdjustSwimTimer();

        strengthBar.value = swimTimer;
    }

    private void FixedUpdate()
    {
        if (IsSwimming)
        {
            float surfaceHeight = OceanHeightSampler.SampleHeight(playerRigidBody.gameObject, playerRigidBody.transform.position) + surfaceOffset;
            float distanceToSurface = surfaceHeight - playerRigidBody.transform.position.y;

            if (playerRigidBody.transform.position.y <= surfaceHeight)
                playerRigidBody.AddForceAtPosition(Vector3.up * (buoyancyForce * (depthFactor * distanceToSurface)), playerRigidBody.transform.position, ForceMode.Acceleration);


        }
    }

    public void CheckForSwimming()
    {
        if (!IsSwimming && transform.position.y < OceanHeightSampler.SampleHeight(gameObject, transform.position) + surfaceOffset)
        {
            playerRigidBody.velocity = new Vector3();
            playerAnimator.SetBool("Swimming", true);
            StartSwimming();
        }
        else if (IsSwimming && transform.position.y > OceanHeightSampler.SampleHeight(gameObject, transform.position) + surfaceOffset)
        {
            playerAnimator.SetBool("Swimming", false);
            EndSwimming();
        }
    }

    private void AdjustSwimTimer()
    {
        if (LobbyBehaviour.isInLobby)
            return;

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

        StopCoroutine(ApplyWalkingMaterial());
        StartCoroutine(ApplySwimmingMaterial());
    }

    public void EndSwimming()
    {
        IsSwimming = false;
        strengthBar.gameObject.SetActive(false);

        StopCoroutine(ApplySwimmingMaterial());
        StartCoroutine(ApplyWalkingMaterial());
    }

    private IEnumerator ApplySwimmingMaterial()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().material = playerSwimmingMaterial;
    }
    private IEnumerator ApplyWalkingMaterial()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().material = playerWalkingMaterial;
    }
}
