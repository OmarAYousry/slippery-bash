using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerInputController playerInputController = null;
    [SerializeField]
    private Rigidbody playerRigidbody = null;
    [SerializeField]
    private AudioSource playerAudioSrc = null;
    [SerializeField]
    private SwimmingBehaviour swimBehaviour = null;

    [SerializeField]
    private GameObject[] playerAvatarPrefabs = null;

    private Animator playerAnimator = null;

    private Transform punchTransform = null;

    private GameObject playerAvatar = null;

    private Vector3 playerSpeed = new Vector3();

    public int playerId { get; private set; }

    bool isJumping = false;

    private void Awake()
    {
        // Register with the GameController
        // to be considered for global changes
        playerId = GameController.RegisterPlayer(this);

        SpawnPlayerAvatar();
    }

    // this method simply adds a random offset to the
    // player's position for a little variety
    private void AddRandomOffsetToPlayerSpawn()
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

    public void SpawnPlayerAvatar()
    {
        int prefabIndex = playerId % playerAvatarPrefabs.Length;
        playerAvatar = Instantiate(playerAvatarPrefabs[prefabIndex], transform);
        playerAnimator = playerAvatar.GetComponent<Animator>();
        punchTransform = playerAvatar.transform.Find("PunchPoint");
        AddRandomOffsetToPlayerSpawn();
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
        //float dampFactor = 0.2f;
        //inputDirection *= dampFactor;

        inputDirection *= 5.0f;
        // update the player speed with the new input
        playerSpeed = new Vector3(inputDirection.x, 0f, inputDirection.y);
    }

    public void MovePlayer(Vector3 moveDirection)
    {
        //playerRigidbody.AddForce(moveDirection, ForceMode.VelocityChange);
        playerRigidbody.velocity = new Vector3(moveDirection.x, playerRigidbody.velocity.y, moveDirection.z);



        transform.LookAt(transform.position + moveDirection);
    }
    bool isStopped = false;
    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (swimBehaviour.CheckForSwimming())
        {
            isJumping = false;
            isOnIce = false;
        }

        if (!isStunned)
        {
            if (playerSpeed.magnitude > 0.0f)
            {
                isStopped = false;
                MovePlayer(playerSpeed);
            }
            else if (!isStopped && !isJumping)
            {
                isStopped = true;
                // good place to add the stopping 'slippiness' speed
                if (isOnIce)
                {
                    MovePlayer(transform.forward * iceSlipSpeed);
                }
                else
                {
                    MovePlayer(transform.forward * 0.0f);
                }
            }
        }
        else
        {
            MovePlayer(new Vector3());
        }
    }

    bool isPunchOnCD = false;
    float punchCD = 0.5f;

    public void PerformPunch()
    {
        if (isDead)
            return;

        // cannot punch if on cooldown
        if (isPunchOnCD)
            return;

        // cannot punch if swimming
        if (swimBehaviour.IsSwimming)
            return;


        playerAnimator.SetTrigger("Punch");
        AudioController.PlaySoundEffect(SoundEffectType.PLAYER_PUNCH, playerAudioSrc);

        StartCoroutine(ApplyPunchCooldown());
        
        StartCoroutine(WaitThenDoAction(0.4f, ()=> {
            Vector3 punchContactPoint = punchTransform.transform.position;
            const float punchRadius = 2.0f;

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
                    hitPlayer.GetHit(forceVector,transform.rotation);
                }
                if (!LobbyBehaviour.isInLobby && contactedCollider.CompareTag("Tile"))
                {
                    TileController hitTile = contactedCollider.GetComponent<TileController>();
                    hitTile.DamageTile();
                }
            }
        }));     
    }

    public IEnumerator ApplyPunchCooldown()
    {
        isPunchOnCD = true;
        yield return new WaitForSeconds(punchCD);
        isPunchOnCD = false;
    }

    public void GetHit(Vector3 forceVector, Quaternion newRot)
    {
        AudioController.PlaySoundEffect(SoundEffectType.PLAYER_HIT, playerAudioSrc);
        playerAnimator.Play("Hit");
        const float hitPower = 4000.0f;
        transform.rotation = newRot;
        Vector3 scaledForceVector = forceVector * hitPower;
        playerRigidbody.AddForce(new Vector3(scaledForceVector.x,1f,scaledForceVector.z), ForceMode.Acceleration);
        //playerRigidbody.AddExplosionForce(400f, transform.position, 100f);
        StartCoroutine(applyStun());
    }
    private bool isStunned = false;
    private IEnumerator applyStun(float stunDuration = 2.0f)
    {
        isStunned = true;
        yield return new WaitForSecondsRealtime(stunDuration);
        isStunned = false;
    }

    [SerializeField]
    private float jumpPower = 0.0f;

    [SerializeField]
    private float iceSlipSpeed = 3.0f;

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

    public void ChangeInputToUI()
    {
        playerInputController.ChangeInputMap("UI");
    }

    public void ChangeInputToGame()
    {
        playerInputController.ChangeInputMap("Player");
    }


    bool isDead = false;

    public void KillPlayer()
    {
        Destroy(playerAvatar);
        swimBehaviour.Drown();
        GameController.DeregisterPlayer(this);
        GameController.CheckEndGameCondition(this);
        Destroy(playerInputController);
        isDead = true;
    }

    bool isOnIce = false;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.LogError(collision.gameObject.SetActive(false));
        //collision.gameObject.SetActive(false);
        // should maybe check "Floor" tag -- not yet implemented
        isJumping = false;

        if (collision.collider.material.name.ToLower().Contains("snow"))
        {
            isOnIce = false;
        }
        else if (collision.collider.material.name.ToLower().Contains("ice"))
        {
            isOnIce = true;
        }
    }

    IEnumerator WaitThenDoAction(float duration, System.Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
    }
}
