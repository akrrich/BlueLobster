using UnityEngine;

public class Props : MonoBehaviour
{
    private PlayerController playerController;

    private Rigidbody2D rb;
    private Animator anim;
    
    [SerializeField] private GameObject alert;

    private AudioSource throwSound; // sonido de prueba para testear

    [SerializeField] private int damage;
    [SerializeField] private int velocity;
    [SerializeField] private int durability;
    [SerializeField] public int weight;

    private bool hasBeenThrown = false;


    void Awake()
    {
        GetComponents();
        InitializeReferences();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollisions(collision);
    }


    public static Props FindCurrentProp(Props currentProp, Vector2 position, float radius, LayerMask detectionLayer)
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(position, radius, detectionLayer);

        foreach (var obj in objectsInRange)
        {
            if (obj.CompareTag("Objeto"))
            {
                currentProp = obj.GetComponent<Props>();

                if (currentProp != null)
                {
                    currentProp.alert.SetActive(false);
                    return currentProp;
                }
                
            }
        }

        return null;
    }


    private void GetComponents()
    {
        playerController = FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();
        throwSound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    private void InitializeReferences()
    {
        rb.isKinematic = true;
        
    }

    private void HandleCollisions(Collision2D collision)
    {
        string collisionTag = collision.collider.tag;

        switch (collisionTag)
        {
            case "Enemy":
                CheckEnemyColision(collision);
                break;

            default: 
                CheckSceneryColisions(collision);
                break;
        }
    }

    private void CheckEnemyColision(Collision2D collision)
    {
        if (hasBeenThrown && collision.collider.CompareTag("Enemy"))
        {
            Enemy currentEnemy = collision.collider.GetComponent<Enemy>();

            if (currentEnemy != null)
            {
                currentEnemy.GetDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void CheckSceneryColisions(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    public void PickObject(Transform objectPositionLight, Transform objectPositionHeavy)
    {
        rb.isKinematic = true;
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (weight == 1)
        {
            transform.position = objectPositionHeavy.position;
        }
        else if (weight == 0) transform.position = objectPositionLight.position;
        transform.SetParent(playerController.transform);
    }

    public void ThrowObject(int direction)
    {
        throwSound.Play();

        rb.isKinematic = false;
        rb.simulated = true;
        transform.SetParent(null);

        Vector2 throwDirection = Vector2.zero;

        

        switch (direction)
        {
            case 0:
                if (playerController.transform.localScale.x == 1) throwDirection = Vector2.right;
                else if (playerController.transform.localScale.x == -1) throwDirection = Vector2.left;
                break;
            case 1:
                throwDirection = Vector2.up;
                break;
            case -1:
                throwDirection = Vector2.down;
                break;
        }

        rb.AddForce(throwDirection * velocity, ForceMode2D.Impulse);
        hasBeenThrown = true;
        Animations();

    }

    private void Animations()
    {
        if(hasBeenThrown == true)
        {
            anim.SetBool("Throw", true);
        }
    }
}