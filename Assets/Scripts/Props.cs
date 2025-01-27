using UnityEngine;

public class Props : MonoBehaviour
{
    private PlayerController playerController;
    private Enemy currentEnemy;

    private Rigidbody2D rb;
    private SpriteRenderer shadow;

    [SerializeField] private int damage;
    [SerializeField] private int velocity;
    [SerializeField] private int durability;

    private bool hasBeenThrown = false;


    void Awake()
    {
        GetComponents();
        InitializeReferences();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenThrown)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                currentEnemy = collision.collider.GetComponent<Enemy>();
                if (currentEnemy != null)
                {
                    currentEnemy.GetDamage(10);
                    Destroy(gameObject);
                }
            }
            else if (collision.collider.CompareTag("Escenario"))
            {
                Destroy(gameObject);
            }
        }
    }


    public static Props FindCurrentProp(Props currentProp, Vector2 position, float detectionRadius, LayerMask detectionLayer)
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(position, detectionRadius, detectionLayer);

        foreach (var obj in objectsInRange)
        {
            if (obj.CompareTag("Objeto"))
            {
                currentProp = obj.GetComponent<Props>();

                if (currentProp != null)
                {
                    return currentProp;
                }
            }
        }

        return null;
    }

    public void PickObject(Transform objectPosition)
    {
        rb.isKinematic = true;
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        shadow.color = Color.green;

        transform.position = objectPosition.position;
        transform.SetParent(playerController.transform);
    }

    public void ThrowObject()
    {
        rb.isKinematic = false;
        rb.simulated = true;
        transform.SetParent(null);

        Vector2 throwDirection = Vector2.zero;

        float zRotation = playerController.transform.eulerAngles.z;

        switch (zRotation)
        {
            case 0:
                if (playerController.transform.localScale.x == 1) throwDirection = Vector2.right;
                else if (playerController.transform.localScale.x == -1) throwDirection = Vector2.left;
                break;
            case 90:
                throwDirection = Vector2.up;
                break;
            case 270:
                throwDirection = Vector2.down;
                break;
        }

        rb.AddForce(throwDirection * velocity, ForceMode2D.Impulse);
        hasBeenThrown = true;
    }


    private void GetComponents()
    {
        playerController = FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();
        shadow = transform.Find("Shadow").GetComponent<SpriteRenderer>();
    }

    private void InitializeReferences()
    {
        rb.isKinematic = true;
        shadow.color = Color.red;
    }
}