using UnityEngine;

public class Props : MonoBehaviour
{
    private PlayerController playerController;

    private Rigidbody2D rb;
    private AudioSource throwSound;
    private Animator anim;
    private SpriteRenderer alert;

    [SerializeField] private int damage;
    [SerializeField] private int velocity;
    [SerializeField] private int durability;
    [SerializeField] private int weight;

    private bool hasBeenThrown = false;

    public int Weight { get => weight; }


    void Awake()
    {
        GetComponents();
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
                    if (currentProp.alert != null)
                    {
                        currentProp.alert.gameObject.SetActive(false);
                    }
                    return currentProp;
                }
            }
        }

        return null;
    }

    public void SetProperties(int damage, int velocity, int durability, int weight)
    {
        this.damage = damage;
        this.velocity = velocity;
        this.durability = durability;
        this.weight = weight;
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
        if (throwSound != null)
        {
            throwSound.Play();
        }

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


    private void GetComponents()
    {
        playerController = FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();
        throwSound = GetComponent<AudioSource>(); 
        anim = GetComponent<Animator>();
        alert = transform.Find("Alert")?.GetComponent<SpriteRenderer>();
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
                Destroy(gameObject);
                currentEnemy.GetDamage(damage);
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

    private void Animations()
    {
        anim.SetBool("Throw", hasBeenThrown);   
    }
}