using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;

    private Rigidbody2D rb;
    private Animator anim;

    private Animator rightHandAnim;
    private Animator leftHandAnim;

    private Transform propPositionHeavy;
    private Transform propPositionLight;

    [SerializeField] private LayerMask detectionLayer;

    private int health = 3;
    private int minHealth = 1;
    private int UPDOWNdirection = 0;

    private float XAxis;
    private float YAxis;
    private float radius = 1f;
    private float speed = 8;


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
        Animations();
        CheckIfPlayerIsAlive();
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
        anim = GetComponent<Animator>();

        propPositionHeavy = transform.Find("PropPositionHeavy").GetComponent<Transform>();
        propPositionLight = transform.Find("PropPositionLight").GetComponent<Transform>();

        rightHandAnim = transform.Find("Right hand").gameObject.GetComponentInChildren<Animator>();
        leftHandAnim = transform.Find("Left hand").gameObject.GetComponentInChildren<Animator>();
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
                currentProp.PickObject(propPositionLight, propPositionHeavy);
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

    private void Animations()
    {
        bool isMoving = rb.velocity.x != 0 || rb.velocity.y != 0;

        bool isHoldingHeavy = currentProp != null && currentProp.Weight == 1;
        bool isHoldingLight = currentProp != null && currentProp.Weight == 0;

        if (health >= minHealth)
        {
            anim.SetBool("Running", isMoving);
            anim.SetBool("Idle", !isMoving);

            rightHandAnim.SetBool("Running", isMoving && currentProp == null);
            leftHandAnim.SetBool("Running", isMoving && currentProp == null);

            rightHandAnim.SetBool("Idle", !isMoving && currentProp == null);
            leftHandAnim.SetBool("Idle", !isMoving && currentProp == null);

            rightHandAnim.SetBool("Holding_heavy", isHoldingHeavy);
            leftHandAnim.SetBool("Holding_heavy", isHoldingHeavy);

            rightHandAnim.SetBool("Holding_light", isHoldingLight);
            leftHandAnim.SetBool("Holding_light", isHoldingLight);
        }

        else
        {
            // animacion de muerte
        }
    }

    private void CheckIfPlayerIsAlive()
    {
        if (health < minHealth)
        {
            PlayerEvents.OnPlayerDefeated?.Invoke();

            // aca habria que poner la animacion de muerte del player
            gameObject.SetActive(false);
        }
    }
}
