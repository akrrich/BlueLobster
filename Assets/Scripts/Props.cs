using UnityEngine;

public class Props : MonoBehaviour
{
    private PlayerController playerController;

    private Rigidbody2D rb;
    private AudioSource throwSound;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer alert;
    private BoxCollider2D boxCollider;

    [SerializeField] private int damage;
    [SerializeField] private float velocity;
    [SerializeField] private int durability;
    [SerializeField] private int weight;
    [SerializeField] private float radius;

    private int minDurability = 1;

    private bool hasBeenThrown = false;

    public int Weight { get => weight; }


    void Awake()
    {
        GetComponents();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnterCollisions(collision);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
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

    public void SetProperties(int damage, float velocity, int durability, int weight, float radius)
    {
        this.damage = damage;
        this.velocity = velocity;
        this.durability = durability;
        this.weight = weight;
        this.radius = radius;
    }

    public void PickObject(Transform objectPositionLight, Transform objectPositionHeavy)
    {
        boxCollider.isTrigger = false;
        rb.simulated = false;
        rb.velocity = Vector2.zero;

        if (weight == 1) transform.position = objectPositionHeavy.position;
        if (weight == 0) transform.position = objectPositionLight.position;

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

    public void HitWithObject()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.NameToLayer("Everything"));

        foreach (var obj in objectsInRange)
        {
            if (obj.CompareTag("Enemy"))
            {
                Enemy currentEnemy = obj.GetComponent<Enemy>();

                if (currentEnemy != null)
                {
                    durability--;

                    if (durability < minDurability)
                    {
                        DestroyThisGameObject();
                    }

                    currentEnemy.GetDamage(damage);
                }
            }
        }
    }


    private void GetComponents()
    {
        playerController = FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();
        throwSound = GetComponent<AudioSource>(); 
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        alert = transform.Find("Alert")?.GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void HandleEnterCollisions(Collision2D collision)
    {
        string collisionTag = collision.collider.tag;

        switch (collisionTag)
        {
            case "Enemy":
                CheckEnterEnemyColision(collision);
                break;

            default: 
                CheckEnterSceneryColisions(collision);
                break;
        }
    }

    private void CheckEnterEnemyColision(Collision2D collision)
    {
        if (hasBeenThrown && collision.collider.CompareTag("Enemy"))
        {
            Enemy currentEnemy = collision.collider.GetComponent<Enemy>();

            if (currentEnemy != null)
            {
                DestroyThisGameObject();
                currentEnemy.GetDamage(damage);
            }
        }
    }

    private void CheckEnterSceneryColisions(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            DestroyThisGameObject();
        }
    }

    private void DestroyThisGameObject()
    {
        SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (var child in childSprites)
        {
            child.enabled = false;
        }

        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        rb.simulated = false;
        rb.isKinematic = true;

        if (throwSound.isPlaying)
        {
            Destroy(gameObject, throwSound.clip.length);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Animations()
    {
        anim.SetBool("Throw", hasBeenThrown);
    }
}