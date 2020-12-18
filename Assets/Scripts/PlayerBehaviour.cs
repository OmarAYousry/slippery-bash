using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerInputController playerInputController = null;
    [SerializeField]
    private Rigidbody playerRigidbody = null;


    private void Awake()
    {
        // Add random offset to spawn
    }

    private void Start()
    {
        // should only be called once by 'host' player
        // does not matter at the moment
        GameController.RegisterPlayer(this);
    }

    public void MovePlayer(Vector2 moveDirection)
    {
        moveDirection *= 0.2f;
        //playerRigidbody.velocity = new Vector3(moveDirection.x, 0f, moveDirection.y);
        playerRigidbody.AddForce(moveDirection.x, 0f, moveDirection.y, ForceMode.VelocityChange);
        transform.LookAt(transform.position + new Vector3(moveDirection.x, 0f, moveDirection.y));
    }

    private void Update()
    {
        //Debug.LogError(playerRigidbody.velocity);
    }

    public void ChangeInputToUI()
    {
        playerInputController.ChangeInputMap("UI");
    }

    public void changeInputToGame()
    {
        playerInputController.ChangeInputMap("Player");
    }
}
