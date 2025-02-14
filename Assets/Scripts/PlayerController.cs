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

    private int health = 100;
    private int minHealth = 1;
    private int UPDOWNdirection = 0;
    private int damage = 1;

    private float XAxis;
    private float YAxis;
    private float radius = 1f;
    private float speed = 8;

    private bool canPunch = false;
    private bool isPunching = false;


    void Awake()
    {
        GetComponents();
        SubscribeToGameManagerEvents();
    }
    
    // Simulacion de Update
    void UpdatePlayerController()
    {
        PickUp();
        Hit();
        Throw();
        Punch();
        Animations();
    }

    // Simulacion de FixedUpdate
    void FixedUpdatePlayerController()
    {
        Movement();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckEnterColisionWithEnemy(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckStayColisionWithEnemy(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        CheckExitColisionWithEnemy(collision);
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

        CheckIfPlayerIsAlive();
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

    private void Movement()
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

    private void Hit()
    {
        if (currentProp != null && Input.GetKeyDown(KeyCode.X))
        {
            if (currentProp != null)
            {
                currentProp.HitWithObject();
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

    private void Punch()
    {
        if (currentProp == null && canPunch && Input.GetKeyDown(KeyCode.X))
        {
            isPunching = true;
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

            // agregar animacion de piña y animacion de golpear con objeto
        }

        else
        {
            // agregar animacion de muerte
        }
    }

    private void CheckIfPlayerIsAlive()
    {
        if (health < minHealth)
        {
            PlayerEvents.OnPlayerDefeated?.Invoke();

            // provisorio el desactivar el objeto
            gameObject.SetActive(false);
        }
    }

    private void CheckEnterColisionWithEnemy(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            canPunch = true;
        }
    }

    private void CheckStayColisionWithEnemy(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            canPunch = true;

            if (isPunching)
            {
                Enemy currentEnemy = collision.collider.GetComponent<Enemy>();

                if (currentEnemy != null)
                {
                    currentEnemy.GetDamage(damage);
                }

                isPunching = false;
            }
        }
    }

    private void CheckExitColisionWithEnemy(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            canPunch = false;
        }
    }
}
