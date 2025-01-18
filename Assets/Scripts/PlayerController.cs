using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;

    private Rigidbody2D rb;

    private float XAxis;
    private float YAxis;

    [SerializeField] private float speed;
    [SerializeField] private int detectionRadius = 5;
    [SerializeField] private LayerMask detectionLayer;

    private bool pickObject = false;


    void Awake()
    {
        GetComponents();
        SubscribeToGameManagerEvents();
    }
    
    // Simulacion de Update
    void UpdatePlayerController()
    {
        GetAxis();
        PickUp();
        Drop();
        Throw();
    }

    // Simulacion de FixedUpdate
    void FixedUpdatePlayerController()
    {
        PlayerMovement();
    }

    void OnDestroy()
    {
        UnsubscribeToGameManagerEvents();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void SubscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStatePlaying += UpdatePlayerController;
        GameManager.Instance.OnGameStatePlayingFixedUpdate += FixedUpdatePlayerController;
    }

    private void UnsubscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStatePlaying -= UpdatePlayerController;
        GameManager.Instance.OnGameStatePlayingFixedUpdate -= FixedUpdatePlayerController;
    }

    private void GetAxis()
    {
        XAxis = Input.GetAxisRaw("Horizontal");
        YAxis = Input.GetAxisRaw("Vertical");
    }

    private void PlayerMovement()
    {
        Vector2 PlayerVelocity = new Vector2(XAxis, YAxis);
        rb.velocity = PlayerVelocity.normalized * speed;

        if (rb.velocity.x != 0 && rb.velocity.y == 0)
        {
            transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            transform.localScale = new Vector2(XAxis, transform.localScale.y);
        }

        else
        {
            if (rb.velocity.y != 0)
            {
                float angle = (YAxis > 0) ? 90f : -90f; // 90° arriba, -90° abajo
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                transform.localScale = new Vector2(1, 1);
            }
        }
    }

    private Props FindCurrentProp()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectionLayer);

        foreach (var obj in objectsInRange)
        {
            if (obj.CompareTag("Objeto"))
            {
                currentProp = obj.GetComponent<Props>();

                if (currentProp != null)
                {
                    return currentProp;
                }
            }
        }

        return null;
    }

    private void PickUp()
    {
        if (Input.GetKeyDown(KeyCode.Z) && pickObject == false)
        {
            FindCurrentProp();

            currentProp.PickObject();
            pickObject = true;
        }
    }

    private void Drop()
    {
        if (pickObject == true && Input.GetKeyDown(KeyCode.X))
        {
            currentProp.DropObject();
            pickObject = false;
        }
    }

    private void Throw()
    {
        if (pickObject == true && Input.GetKeyDown(KeyCode.C))
        {
            currentProp.ThrowObject();
            pickObject = false;
        }
    }
}
