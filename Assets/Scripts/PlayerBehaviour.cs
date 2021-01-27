using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerInputController playerInputController = null;
    [SerializeField]
    private Rigidbody playerRigidbody = null;
    [SerializeField]
    private Animator playerAnimator = null;
    [SerializeField]
    private AudioSource playerAudioSrc = null;
    [SerializeField]
    private Transform punchTransform = null;

    [SerializeField]
    private SwimmingBehaviour swimBehaviour = null;

    private Vector3 playerSpeed = new Vector3();

    bool isJumping = false;

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

    private void Update()
    {
        swimBehaviour.CheckForSwimming();
    }

    public void PerformPunch()
    {
        playerAnimator.SetTrigger("Punch");
        AudioController.PlaySoundEffect(SoundEffectType.PLAYER_PUNCH, playerAudioSrc);

        StartCoroutine(WaitThenDoAction(0.5f, ()=> {
            Vector3 punchContactPoint = punchTransform.transform.position;
            const float punchRadius = 1.0f;

            Collider[] collidersInContact = Physics.OverlapSphere(punchContactPoint, punchRadius);

            foreach (Collider contactedCollider in collidersInContact)
            {
                if (contactedCollider.CompareTag("Player"))
                {
                    PlayerBehaviour hitPlayer = contactedCollider.GetComponent<PlayerBehaviour>();

                    // avoid overlap behaviour of player punching oneself
                    if (hitPlayer == this)
                        continue;

                    // get closest point on contacting collider
                    Vector3 contactPoint = hitPlayer.transform.position;
                    // set up force vector from contact point with direction
                    // equal to the punching player's (normalized) foward vector
                    Vector3 forceVector = (contactPoint - transform.position).normalized;
                    // Let the player behaviour of the hit player
                    // handle its own getting hit behaviour
                    hitPlayer.GetHit(forceVector);
                }
                if (contactedCollider.CompareTag("Tile"))
                {
                    TileController hitTile = contactedCollider.GetComponent<TileController>();
                    hitTile.DamageTile();
                }
            }
        }));     
    }

    public void GetHit(Vector3 forceVector)
    {
        AudioController.PlaySoundEffect(SoundEffectType.PLAYER_HIT, playerAudioSrc);
        playerAnimator.SetTrigger("Hit");

        const float hitPower = 5.0f;
        Vector3 scaledForceVector = forceVector * hitPower;
        playerRigidbody.AddForce(scaledForceVector, ForceMode.Impulse);
    }

    [SerializeField]
    private float jumpPower = 0.0f;

    public void PerformJump()
    {
        if (isJumping)
            return;

        isJumping = true;

        AudioController.PlaySoundEffect(SoundEffectType.PLAYER_JUMP, playerAudioSrc);

        playerAnimator.SetTrigger("Jump");

        Vector3 jumpDirection = transform.up; // <-- Global up or local up better? unsure, but global up *should* be the same as local up
        playerRigidbody.AddForce(jumpDirection * jumpPower, ForceMode.Impulse);
    }

    private void LateUpdate()
    {
        //Debug.Log(GetComponent<UnityEngine.InputSystem.PlayerInput>().currentActionMap);
    }

    public void ChangeInputToUI()
    {
        playerInputController.ChangeInputMap("UI");
    }

    public void ChangeInputToGame()
    {
        playerInputController.ChangeInputMap("Player");
    }

    public void KillPlayer()
    {
        Destroy(gameObject);
        GameController.CheckEndGameCondition(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.LogError(collision.gameObject.SetActive(false));
        //collision.gameObject.SetActive(false);
        // should maybe check "Floor" tag -- not yet implemented
        isJumping = false;
    }

    IEnumerator WaitThenDoAction(float duration, System.Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
    }
}
