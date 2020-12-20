﻿using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerInputController playerInputController = null;
    [SerializeField]
    private Rigidbody playerRigidbody = null;
    [SerializeField]
    private Animator playerAnimator = null;
    [SerializeField]
    private Transform punchTransform = null;

    private Vector3 playerSpeed = new Vector3();

    private void Awake()
    {
        SpawnPlayerWithRandomOffset();
    }

    // currently 'spawning' is handled by PlayerManager
    // this method simply adds a random offset to the
    // player's position for a little variety
    private void SpawnPlayerWithRandomOffset()
    {
        // Add random offset to spawn
        float minSpawnX = -2.0f;
        float maxSpawnX = 2.0f;
        float spawnX = Random.Range(minSpawnX, maxSpawnX);

        // y is unchanged (no need to vary spawn height)
        float spawnY = transform.position.y;
        spawnY = 1.0f;

        float minSpawnZ = -2.0f;
        float maxSpawnZ = 2.0f;
        float spawnZ = Random.Range(minSpawnZ, maxSpawnZ);

        transform.position = new Vector3(spawnX, spawnY, spawnZ);
    }

    private void Start()
    {
        // Register with the GameController
        // to be considered for global changes
        GameController.RegisterPlayer(this);
    }

    public void UpdateSpeed(Vector2 inputDirection)
    {
        playerAnimator.SetFloat("Speed", inputDirection.magnitude);
        
        // Slipping animation... tricky...
        //if (inputDirection.magnitude < 0.01f)
        //{
        //    if (playerRigidbody.velocity.magnitude > 0)
        //    {
        //        playerAnimator.SetBool("Slipping", true);

        //    }
        //    else
        //    {
        //        playerAnimator.SetBool("Slipping", true);
        //    }
        //}


        // dampen user input slightly to avoid
        // janky movement (that is way too fast)
        float dampFactor = 0.2f;
        inputDirection *= dampFactor;

        // update the player speed with the new input
        playerSpeed = new Vector3(inputDirection.x, 0f, inputDirection.y);
    }

    public void MovePlayer(Vector3 moveDirection)
    {
        playerRigidbody.AddForce(moveDirection, ForceMode.VelocityChange);

        transform.LookAt(transform.position + moveDirection);
    }

    private void FixedUpdate()
    {
        // Move the player according to its current speed
        MovePlayer(playerSpeed);
    }

    public void PerformPunch()
    {
        playerAnimator.SetTrigger("Punch");

        if (Physics.SphereCast(punchTransform.transform.position, 1.0f, transform.forward, out RaycastHit hitInfo, maxDistance: 0.0f))
        {
            // assume player?? what if floor?
        }

        Vector3 punchContactPoint = punchTransform.transform.position;
        const float punchRadius = 1.0f;

        Collider[] collidersInContact =  Physics.OverlapSphere(punchContactPoint, punchRadius);

        foreach (Collider contactedCollider in collidersInContact)
        {
            Debug.LogWarning(contactedCollider.gameObject.name);
            if (contactedCollider.CompareTag("Player"))
            {
                PlayerBehaviour hitPlayer = contactedCollider.GetComponent<PlayerBehaviour>();

                // avoid overlap behaviour of player punching oneself
                if (hitPlayer == this)
                    continue;

                // get closest point on contacting collider
                //Vector3 contactPoint = contactedCollider.ClosestPoint(punchContactPoint);
                Vector3 contactPoint = hitPlayer.transform.position;
                // set up 'ray' of force from contact point with direction
                // equal to the punching player's (normalized) foward vector
                Vector3 forceVector = (contactPoint - transform.position).normalized;
                //Ray forceRay = new Ray(transform.position, transform.);
                // Let the player behaviour of the hit player
                // handle its own getting hit behaviour
                hitPlayer.GetHit(forceVector);
            }
        }        
    }

    public void GetHit(Vector3 forceVector)
    {
        playerAnimator.SetTrigger("Hit");

        const float hitPower = 5.0f;
        Vector3 scaledForceVector = forceVector * hitPower;
        playerRigidbody.AddForce(scaledForceVector, ForceMode.Impulse);
    }

    public void PerformJump()
    {
        playerAnimator.SetTrigger("Jump");

        float jumpPower = 3.0f;
        Vector3 jumpDirection = transform.up; // <-- Global up or local up better? unsure, but global up *should* be the same as local up
        playerRigidbody.AddForce(jumpDirection * jumpPower, ForceMode.Impulse);
    }

    public void ChangeInputToUI()
    {
        playerInputController.ChangeInputMap("UI");
    }

    public void ChangeInputToGame()
    {
        playerInputController.ChangeInputMap("Player");
    }
}
