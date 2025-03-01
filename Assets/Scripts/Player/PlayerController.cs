using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;
    private JoystickTouch joystickTouch;
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
    private int lastWeight = 0;

    private bool isAlive = true;
    private bool canPunch = false;
    private bool isPunching = false;
    private bool isHitting = false;
    private bool isThrowing = false;

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
        Animations_Shadow();
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

        shadow = transform.Find("Shadow player").GetComponentInChildren<ShadowPlayer>();
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
        if (DeviceManager.CurrentPlatform == "PC")
        {
            if (Time.timeScale == 1f)
            {
                int leftClick = 0; int rightClick = 1;

                if (Input.GetMouseButtonDown(rightClick) || Input.GetButtonDown("Circle/B"))
                {
                    PickUp();
                }

                if (Input.GetMouseButtonDown(leftClick) || Input.GetButtonDown("Square/X"))
                {
                    Hit();
                    StartCoroutine(throwing());
                }


                if (Input.GetMouseButtonDown(rightClick) || Input.GetButtonDown("Circle/B"))
                {
                    Throw();
                }

                //solucionar esto
                if (Input.GetMouseButtonDown(leftClick) || Input.GetButtonDown("Square/X"))
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
                lastWeight = currentProp.Weight;
            }
        }
    }

    private void Hit()
    {
        if (currentProp != null)
        {  
            currentProp.HitWithObject();
            isHitting = true;
        }
    }

    private void Throw()
    {
        if (currentProp != null && currentProp.CanThorw)
        {
            isThrowing = true;
            isHitting = false;
            currentProp.ThrowObject(UPDOWNdirection);
            lastWeight = currentProp.Weight;
            currentProp = null;
            StartCoroutine(throwing());
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

        if (currentProp != null)
        {
            //si el prop se rompe, no lo tira
            if (currentProp.durability <= 0) isThrowing = false;
        }

        bool isHoldingHeavy = currentProp != null && currentProp.Weight == 1 && isHitting == false && isThrowing == false;
        bool isHoldingLight = currentProp != null && currentProp.Weight == 0 && isHitting == false && isThrowing == false;


        if (health >= minHealth)
        {
            //animacion running e idle player
            anim.SetBool("Running", isMoving);
            anim.SetBool("Idle", !isMoving);

            //animacion running manos
            rightHandAnim.SetBool("Running", isMoving && currentProp == null && !isThrowing);
            leftHandAnim.SetBool("Running", isMoving && currentProp == null && !isThrowing);

            //animacion idle manos
            rightHandAnim.SetBool("Idle", !isMoving && currentProp == null && !isThrowing);
            leftHandAnim.SetBool("Idle", !isMoving && currentProp == null && !isThrowing);

            //animacion holding heavy object manos
            rightHandAnim.SetBool("Holding_heavy", isHoldingHeavy);
            leftHandAnim.SetBool("Holding_heavy", isHoldingHeavy);

            //animacion holding light object manos
            rightHandAnim.SetBool("Holding_light", isHoldingLight);
            leftHandAnim.SetBool("Holding_light", isHoldingLight);

            //animacion tirar light object manos
            rightHandAnim.SetBool("Throw_light", isThrowing == true && lastWeight == 0);
            leftHandAnim.SetBool("Throw_light", isThrowing == true && lastWeight == 0);

            //animacion golpear con light object manos
            rightHandAnim.SetBool("Hit_light", isHitting == true && lastWeight == 0);
            leftHandAnim.SetBool("Hit_light", isHitting == true && lastWeight == 0);

            //animacion tirar heavy object manos
            rightHandAnim.SetBool("Throw_heavy", isThrowing == true && lastWeight == 1);
            leftHandAnim.SetBool("Throw_heavy", isThrowing == true && lastWeight == 1);

            rightHandAnim.SetBool("Hit_heavy", isHitting == true && lastWeight == 1);
            leftHandAnim.SetBool("Hit_heavy", isHitting == true && lastWeight == 1);

            // agregar animacion de piña 
        }

        else
        {
            // agregar animacion de muerte
        }
    }
    
    private void Animations_Shadow()
    {
        //animacion de sombra idle
        if (rb.velocity.x == 0) shadow.animations(1, transform.localScale, 1);

        //animacion de sombra running
        if (rb.velocity.x != 0) shadow.animations(2, transform.localScale, 2);

        //animacion de sombra lanzamiento
        if (isThrowing == true && rb.velocity.x == 0) shadow.animations(3, transform.localScale, 3);
        if (isThrowing == true && rb.velocity.x != 0) shadow.animations(9, transform.localScale, 3);

        //animacion de sombra sostener objeto idle
        if (currentProp != null && currentProp.Weight == 1 && rb.velocity.x == 0) shadow.animations(1, transform.localScale, 4);
        if (currentProp != null && currentProp.Weight == 0 && rb.velocity.x == 0) shadow.animations(1, transform.localScale, 5);

        //animacion de sombra sostener objeto running
        if (currentProp != null && currentProp.Weight == 1 && rb.velocity.x != 0) shadow.animations(2, transform.localScale, 4);
        if (currentProp != null && currentProp.Weight == 0 && rb.velocity.x != 0) shadow.animations(2, transform.localScale, 5);
    }

    private IEnumerator throwing()
    {
        yield return new WaitForSeconds(0.40f);
        isThrowing = false;
        isHitting = false;
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
