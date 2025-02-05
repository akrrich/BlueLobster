using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;

    private Rigidbody2D rb;
    private Transform propPosition;

    private float radius = 1f;
    private int health = 3;
    
    private float XAxis;
    private float YAxis;

    private float speed = 8;

    [SerializeField] private LayerMask detectionLayer;


    void Awake()
    {
        GetComponents();
        SubscribeToGameManagerEvents();
    }
    
    // Simulacion de Update
    void UpdatePlayerController()
    {
        PickUp();
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
        Gizmos.DrawWireSphere(transform.position, radius);
    }


    public void GetDamage(int damage)
    {
        health -= damage;
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        propPosition = transform.Find("PropPosition").GetComponent<Transform>();
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

    private void PlayerMovement()
    {
        XAxis = Input.GetAxisRaw("Horizontal");
        YAxis = Input.GetAxisRaw("Vertical");

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

    private void PickUp()
    {
        if (currentProp == null && Input.GetKeyDown(KeyCode.Z))
        {
            currentProp = Props.FindCurrentProp(currentProp, transform.position, radius, detectionLayer);

            if (currentProp != null)
            {
                currentProp.PickObject(propPosition);
            }
        }
    }

    private void Throw()
    {
        if (currentProp != null && Input.GetKeyDown(KeyCode.C))
        {
            if (currentProp != null)
            {
                currentProp.ThrowObject();
                currentProp = null;
            }
        }
    }
}
