using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Props currentProp;

    private Rigidbody2D rb;
    private Transform propPositionHeavy;
    private Transform propPositionLight;
    private Animator anim;

    private GameObject rightHand;
    private GameObject leftHand;

    private Animator rightHandAnim;
    private Animator leftHandAnim;

    private float radius = 1f;
    private int health = 3;
    
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


    public void GetDamage(int damage)
    {
        health -= damage;
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        propPositionHeavy = transform.Find("PropPositionHeavy").GetComponent<Transform>();
        propPositionLight = transform.Find("PropPositionLight").GetComponent<Transform>();

        anim = GetComponent<Animator>();

        rightHand = transform.Find("Right hand")?.gameObject;
        leftHand = transform.Find("Left hand")?.gameObject;

        if (rightHand != null) rightHandAnim = rightHand.GetComponent<Animator>();
        if (leftHand != null) leftHandAnim = leftHand.GetComponent<Animator>();
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

                if (currentProp.weight == 1)
                {
                    rightHandAnim.SetBool("Holding_heavy", true);
                    leftHandAnim.SetBool("Holding_heavy", true);
                }
                else
                {
                    rightHandAnim.SetBool("Holding_light", true);
                    leftHandAnim.SetBool("Holding_light", true);
                }

                rightHandAnim.SetBool("Running", false);
                leftHandAnim.SetBool("Running", false);

                rightHandAnim.SetBool("Idle", false);
                leftHandAnim.SetBool("Idle", false);

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


                rightHandAnim.SetBool("Holding_heavy", false);  
                leftHandAnim.SetBool("Holding_heavy", false);
                rightHandAnim.SetBool("Holding_light", false);
                leftHandAnim.SetBool("Holding_light", false);

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

            if (currentProp == null)
            {
                leftHandAnim.SetBool("Running", true);
                rightHandAnim.SetBool("Running", true);
            }

            leftHandAnim.SetBool("Idle", false);
            rightHandAnim.SetBool("Idle", false);

        }
        else
        {
            anim.SetBool("Running", false);
            anim.SetBool("Idle", true);

            if (currentProp == null)
            {
                leftHandAnim.SetBool("Idle", true);
                rightHandAnim.SetBool("Idle", true);
            }

            leftHandAnim.SetBool("Running", false);
            rightHandAnim.SetBool("Running", false);
        }

        
    }
}
