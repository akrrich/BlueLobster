using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;
    private ShadowPlayer shadow;

    private Rigidbody2D rb;
    private Animator anim;

    private Animator rightHandAnim;
    private Animator leftHandAnim;

    private Transform propPositionHeavy;
    private Transform propPositionLight;

    [SerializeField] private LayerMask detectionLayer;

    private int health = 10;
    private int minHealth = 1;
    private int UPDOWNdirection = 0;
    private int damage = 1;

    private float radius = 1f;
    private float speed = 8;

    private bool isAlive = true;
    private bool canPunch = false;
    private bool isPunching = false;

    public Props CurrentProp { get => currentProp; set => currentProp = value; }

    public Rigidbody2D Rb { get => rb; }
    public Animator RightHandAnim { get => rightHandAnim; }

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
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        propPositionHeavy = transform.Find("PropPositionHeavy").GetComponent<Transform>();
        propPositionLight = transform.Find("PropPositionLight").GetComponent<Transform>();

        rightHandAnim = transform.Find("Right hand").GetComponent<Animator>();
        leftHandAnim = transform.Find("Left hand").GetComponent<Animator>();

        shadow = transform.Find("Shadow player").GetComponent<ShadowPlayer>();
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
        Vector2 direction = DeviceManager.GetMovementInput();
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
        if (DeviceManager.CurrentPlatform == "PC")
        {
            if (Time.timeScale == 1f)
            {
                if (DeviceManager.GetRightClickOrCircleB()) // Agarrar
                {
                    PickUp();
                }

                if (DeviceManager.GetLeftClickOrSquareX()) // Golpear con objeto
                {
                    Hit();
                }


                if (DeviceManager.GetRightClickOrCircleB()) // Tirar
                {
                    Throw();
                }

                if (DeviceManager.GetLeftClickOrSquareX()) // Golpear con Puño
                {
                    Punch();
                }
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
        if (currentProp != null && !currentProp.IsHitting)
        {
            currentProp.HitWithObject();
        }
    }

    private void Throw()
    {
        if (currentProp != null && currentProp.CanThorw)
        {
            currentProp.ThrowObject(UPDOWNdirection, currentProp);
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

        bool isHoldingLight = currentProp != null && currentProp.Weight == "light" && !currentProp.IsHitting && !currentProp.HasBeenThrown;
        bool isHoldingHeavy = currentProp != null && currentProp.Weight == "heavy" && !currentProp.IsHitting && !currentProp.HasBeenThrown;

        if (isAlive)
        {
            //animacion sombra player
            shadow.Animations(this);

            //animacion running e idle player
            anim.SetBool("Running", isMoving);
            anim.SetBool("Idle", !isMoving);

            //animacion running manos
            rightHandAnim.SetBool("Running", isMoving && currentProp == null);
            leftHandAnim.SetBool("Running", isMoving && currentProp == null);

            //animacion idle manos
            rightHandAnim.SetBool("Idle", !isMoving && currentProp == null);
            leftHandAnim.SetBool("Idle", !isMoving && currentProp == null);

            //animacion holding heavy object manos
            rightHandAnim.SetBool("Holding_heavy", isHoldingHeavy);
            leftHandAnim.SetBool("Holding_heavy", isHoldingHeavy);

            //animacion holding light object manos
            rightHandAnim.SetBool("Holding_light", isHoldingLight);
            leftHandAnim.SetBool("Holding_light", isHoldingLight);

            //animacion tirar light object manos
            rightHandAnim.SetBool("Throw_light", currentProp != null && currentProp.HasBeenThrown && currentProp.Weight == "light");
            leftHandAnim.SetBool("Throw_light", currentProp != null && currentProp.HasBeenThrown && currentProp.Weight == "light");

            //animacion golpear con light object manos
            rightHandAnim.SetBool("Hit_light", currentProp != null && currentProp.IsHitting && currentProp.Weight == "light");
            leftHandAnim.SetBool("Hit_light", currentProp != null && currentProp.IsHitting && currentProp.Weight == "light");

            //animacion tirar heavy object manos
            rightHandAnim.SetBool("Throw_heavy", currentProp != null && currentProp.HasBeenThrown && currentProp.Weight == "heavy");
            leftHandAnim.SetBool("Throw_heavy", currentProp != null && currentProp.HasBeenThrown && currentProp.Weight == "heavy");

            //animacion golpear con heavy object manos
            rightHandAnim.SetBool("Hit_heavy", currentProp != null && currentProp.IsHitting && currentProp.Weight == "heavy");
            leftHandAnim.SetBool("Hit_heavy", currentProp != null && currentProp.IsHitting && currentProp.Weight == "heavy");

            // agregar animacion de piña 
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

            // provisorio el desactivar el objeto (esto tira error por ahora)
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
