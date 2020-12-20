using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script spawns a ship and moves it towards the player positions.
/// TODO: the ship may respawn while still enabled. Maybe object pooling instead?
/// </summary>
public class TitanicBehavior: EventBehavior
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Transform destroyCollider;

    /// <summary>
    /// How far from the center should the ship spawn and move from?
    /// </summary>
    [Tooltip("How far from the center should the ship spawn and move from?")]
    public float spawnDistance = 100;
    /// <summary>
    /// How long should the collider be enabled to destroy tiles?
    /// </summary>
    [Tooltip("How long should the collider be enabled to destroy tiles?")]
    public float destroyLifetime = .1f;
    /// The ship moves towards the player positions.
    /// </summary>
    [Tooltip("The ship moves towards the player positions.")]
    public Transform[] targets;

    private int targetAmount;
    private Vector3 targetPoint;
    private Vector3 spawnPoint;
    private Vector3 desiredVelocity;


    //---------------------------------------------------------------------------------------------//
    public override void StartBehavior(float duration)
    {
        gameObject.SetActive(true);

        // search a point to move to
        targetPoint = Vector3.zero;
        targetAmount = 0;
        for(int i = 0; i < targets.Length; i++)
        {
            // check if the player hasn't despawned yet (not expected)
            if(targets[i])
            {
                targetPoint += targets[i].position;
                targetAmount++;
            }
        }
        targetPoint /= targetAmount;

        // spawn around that point
        spawnPoint = Random.insideUnitCircle.normalized * spawnDistance;
        spawnPoint.z = spawnPoint.y;
        spawnPoint += targetPoint;
        spawnPoint.y = OceanHeightSampler.SampleHeight(gameObject, spawnPoint);
        transform.position = spawnPoint;

        // set the velocity
        desiredVelocity = 2 * (targetPoint - spawnPoint);
        desiredVelocity.y = 0;
        desiredVelocity /= duration;

        // reset the rigidbody
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;
    }

    public override void ResetBehavior()
    {
        Sink();
    }


    //---------------------------------------------------------------------------------------------//
    private void Update()
    {
        if(desiredVelocity.sqrMagnitude > 0)
        {
            // move the ship on the ocean
            rigid.AddForce(desiredVelocity - rigid.velocity, ForceMode.Acceleration);
            spawnPoint = transform.position;
            spawnPoint.y = OceanHeightSampler.SampleHeight(gameObject, spawnPoint);
            transform.position = spawnPoint;
            // TODO: add roll
            if(rigid.velocity.sqrMagnitude > 0)
            {
                transform.rotation = Quaternion.LookRotation(rigid.velocity);
            }
        }
        else if(transform.position.y < -10)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: check if collision with tile
        if(desiredVelocity.sqrMagnitude > 0)
        {
            destroyCollider.parent = null;
            destroyCollider.position = transform.position;
            destroyCollider.rotation = Quaternion.LookRotation(desiredVelocity, Vector3.up);
            destroyCollider.gameObject.SetActive(true);

            Invoke("DeactivateDestroyCollider", destroyLifetime);

            Sink();
        }
    }

    private void DeactivateDestroyCollider()
    {
        destroyCollider.gameObject.SetActive(false);
        destroyCollider.parent = transform;
    }

    private void Sink()
    {
        desiredVelocity = Vector3.zero;
        rigid.useGravity = true;
    }
}
