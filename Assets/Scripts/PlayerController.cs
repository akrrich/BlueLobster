using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;

    private Rigidbody2D rb;
    private Transform propPosition;
    private Animator anim;

    private int radius = 1;
    
    private float XAxis;
    private float YAxis;
    private int UPDOWNdirection = 0;

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
        animations();
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


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        propPosition = transform.Find("PropPosition").GetComponent<Transform>();
        anim = GetComponent<Animator>();
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
            transform.localScale = new Vector2(XAxis, transform.localScale.y);
            UPDOWNdirection = 0;
        }

        else
        {
            if (rb.velocity.y != 0)
            {
               if (rb.velocity.y > 0) UPDOWNdirection = 1;

               else if (rb.velocity.y < 0) UPDOWNdirection = -1;

                
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
                currentProp.ThrowObject(UPDOWNdirection);
                currentProp = null;
            }
        }
    }

    private void animations()
    {
        if(rb.velocity.x != 0 || rb.velocity.y != 0)
        {
            anim.SetBool("Running", true);
            anim.SetBool("Idle", false);
        }
        else
        {
            anim.SetBool("Running", false);
            anim.SetBool("Idle", true);
        }
    }
}
