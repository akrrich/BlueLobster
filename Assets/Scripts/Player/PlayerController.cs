using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;
    private JoystickTouch joystickTouch;

    private Rigidbody2D rb;
    private Animator anim;

    private Animator rightHandAnim;
    private Animator leftHandAnim;

    private Transform propPositionHeavy;
    private Transform propPositionLight;

    [SerializeField] private LayerMask detectionLayer;

    private int health = 1;
    private int minHealth = 1;
    private int UPDOWNdirection = 0;
    private int damage = 1;

    private float radius = 1f;
    private float speed = 8;

    private bool isAlive = true;
    private bool canPunch = false;
    private bool isPunching = false;


    public bool IsAlive { get => isAlive; }


    void Awake()
    {
        GetComponents();
        SubscribeToGameManagerEvents();
        SubscribeToInputEvents();
    }
    
    // Simulacion de Update
    void UpdatePlayerController()
    {
        InputsForControllerAndPC();
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
        CheckEnterColisionAndTriggerWithPropToAEnabledButton(collision.collider);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckStayColisionWithEnemy(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        CheckExitColisionWithEnemy(collision);
        CheckExitColisionAndTriggerWithPropToDisabledButton(collision.collider);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        CheckEnterColisionAndTriggerWithPropToAEnabledButton(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        CheckExitColisionAndTriggerWithPropToDisabledButton(collider);
    }

    void OnDestroy()
    {
        UnsubscribeToGameManagerEvents();
        UnsubscribeToInputEvents();
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
        joystickTouch = FindObjectOfType<JoystickTouch>();

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

    private void SubscribeToInputEvents()
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            InputTouch.OnPunchAndHit += Punch;
            InputTouch.OnPunchAndHit += Hit;
            InputTouch.OnThrow += Throw;
            InputTouch.OnPickUp += PickUp;
        }
    }

    private void UnsubscribeToInputEvents()
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            InputTouch.OnPunchAndHit -= Punch;
            InputTouch.OnPunchAndHit -= Hit;
            InputTouch.OnThrow -= Throw;
            InputTouch.OnPickUp -= PickUp;
        }
    }

    private void Movement()
    {
        Vector2 direction = DeviceManager.GetMovementInput(joystickTouch.LastDirection);
        rb.velocity = direction.normalized * speed;

        if (direction.x > 0.1f) // Derecha
        {
            UPDOWNdirection = 0; 
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }

        else if (direction.x < -0.1f) // Izquierda
        {
            UPDOWNdirection = 0;
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

        else if (direction.y > 0.1f) // Arriba
        {
            UPDOWNdirection = 1;
        }

        else if (direction.y < -0.1f) // Abajo
        {
            UPDOWNdirection = -1;
        }
    }

    private void InputsForControllerAndPC()
    {
        int leftClick = 0; int rightClick = 1;

        if (DeviceManager.CurrentPlatform == "PC")
        {
            if (Input.GetMouseButtonDown(rightClick) || Input.GetButtonDown("Circle/B"))
            {
                PickUp();
            }

            if (Input.GetMouseButtonDown(leftClick) || Input.GetButtonDown("Square/X"))
            {
                Hit();
            }

            if (Input.GetMouseButtonDown(rightClick) || Input.GetButtonDown("Circle/B"))
            {
                Throw();
            }

            if (Input.GetMouseButtonDown(leftClick) || Input.GetButtonDown("Square/X"))
            {
                Punch();
            }
        }
    }

    private void PickUp()
    {
        if (currentProp == null)
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
        if (currentProp != null)
        {  
            currentProp.HitWithObject();
        }
    }

    private void Throw()
    {
        if (currentProp != null && currentProp.CanThorw)
        {
            currentProp.ThrowObject(UPDOWNdirection);
            currentProp = null;
        }
    }

    private void Punch()
    {
        if (currentProp == null && canPunch)
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
        isAlive = health >= minHealth; 

        if (!isAlive)
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

    private void CheckEnterColisionAndTriggerWithPropToAEnabledButton(Collider2D collider)
    {
        if (currentProp == null && collider.CompareTag("Objeto"))
        {
            PlayerEvents.OnEnabledPickUpButton?.Invoke();
        }
    }

    private void CheckExitColisionAndTriggerWithPropToDisabledButton(Collider2D collider)
    {
        if (collider.CompareTag("Objeto"))
        {
            PlayerEvents.OnDisabledPickUpButton?.Invoke();
        }
    }
}
