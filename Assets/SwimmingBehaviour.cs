using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwimmingBehaviour : MonoBehaviour
{    
    [SerializeField]
    private PlayerBehaviour playerBehaviour = null;
    [SerializeField]
    private StaminaBarBehaviour staminaBar = null;

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
    private PhysicMaterial playerWalkingMaterial;

    [SerializeField]
    private PhysicMaterial playerSwimmingMaterial;

    public bool IsSwimming { get; private set; } = false;

    void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();

        // start out not swimming
        EndSwimming();
    }

    void Update()
    {
        AdjustSwimTimer();
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

    public bool CheckForSwimming()
    {
        if (!IsSwimming && transform.position.y < OceanHeightSampler.SampleHeight(gameObject, transform.position) + surfaceOffset)
        {
            playerRigidBody.velocity = new Vector3();
            playerAnimator.SetBool("Swimming", true);
            StartSwimming();
            return true;
        }
        else if (IsSwimming && transform.position.y > OceanHeightSampler.SampleHeight(gameObject, transform.position) + surfaceOffset)
        {
            playerAnimator.SetBool("Swimming", false);
            EndSwimming();
        }
        return false;
    }

    private void AdjustSwimTimer()
    {
        float breathChange = 0;

        if (IsSwimming)
        {
            breathChange -= Time.deltaTime;
        }
        else if (playerBehaviour.IsOnGround)
        {
            breathChange += Time.deltaTime;
        }

        staminaBar.modifyStaminaByValue(breathChange);
    }

    public void StartSwimming()
    {
        playerBehaviour.IsOnGround = false;

        IsSwimming = true;

        StopCoroutine(ApplyWalkingMaterial());
        StartCoroutine(ApplySwimmingMaterial());
    }

    public void EndSwimming()
    {
        IsSwimming = false;

        StopCoroutine(ApplySwimmingMaterial());
        StartCoroutine(ApplyWalkingMaterial());
    }

    public void Drown()
    {
        IsSwimming = false;
        Destroy(this);
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
