using System.Collections;
using UnityEngine;

public class Props : MonoBehaviour
{
    private PlayerController playerController;

    private Rigidbody2D rb;
    private AudioSource throwSound;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer alert;
    private SpriteRenderer shadow;
    private BoxCollider2D boxCollider;

    [SerializeField] private int damage;
    [SerializeField] private int durability;
    [SerializeField] private float velocity;
    [SerializeField] private float radius;
    [SerializeField] private string weight;

    private int minDurability = 1;

    private bool canThrow = false;
    private bool hasBeenThrown = false;
    private bool pickedUp = false;
    private bool isHitting = false;

    public string Weight { get => weight; }

    public bool CanThorw { get => canThrow; }
    public bool HasBeenThrown { get => hasBeenThrown; }
    public bool IsHitting { get => isHitting; }


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

    // setear las propiedades al enemigo cuando se le asigna el script
    public void SetProperties(int damage, float velocity, int durability, string weight, float radius)
    {
        this.damage = damage;
        this.velocity = velocity;
        this.durability = durability;
        this.weight = weight;
        this.radius = radius;
    }

    public void PickObject(Transform objectPositionLight, Transform objectPositionHeavy)
    {
        pickedUp = true;

        // verificacion es necesaria por el momento, ya que la sombra no la tiene el enemigo todavia
        if (shadow != null)
        {
            shadow.gameObject.SetActive(false);
        }

        //fisicas

        boxCollider.isTrigger = false; // Para el enemigo cuando se convierte en objeto.
        rb.simulated = false;
        rb.velocity = Vector2.zero;   

        //posicionamiento de objeto en game object vacio

        if (weight == "light") transform.position = objectPositionLight.position;
        if (weight == "heavy") transform.position = objectPositionHeavy.position;

        //posicionamiento respecto a mano derecha

        transform.SetParent(playerController.RightHandAnim.transform);
        if (weight == "light") transform.localPosition = new Vector3(-0.01f, -0.24f, 0);
        if (weight == "heavy") transform.localPosition = new Vector3(-0.011f, -0.232f, 0);

        //animaciones y sorting layer

        spriteRenderer.sortingOrder = 5; // para que quede por ecnima del player el objeto
        Animations();

        StartCoroutine(CanThrowTheObject());
    }

    public void ThrowObject(int direction, Props currentProp)
    {
        pickedUp = false;
        hasBeenThrown = true;

        throwSound.Play();

        //parte de fisicas

        gameObject.layer = 6; // esto para que es?
        rb.isKinematic = false;
        rb.simulated = true;
        transform.SetParent(null);

        //direccion y lanzamiento

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
        Animations();

        StartCoroutine(SetCurrentPropInNull());
    }

    public void HitWithObject()
    {
        isHitting = true;

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.NameToLayer("Everything"));

        foreach (var obj in objectsInRange)
        {
            if (obj.CompareTag("Enemy"))
            {
                Enemy currentEnemy = obj.GetComponent<Enemy>();

                if (currentEnemy != null)
                {
                    currentEnemy.GetDamage(damage);

                    durability--;

                    if (durability < minDurability)
                    {
                        DestroyThisGameObject();
                    }
                }
            }
        }

        StartCoroutine(SetHittingInFalse());
    }

    private void GetComponents()
    {
        playerController = FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();
        throwSound = GetComponent<AudioSource>(); 
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        alert = transform.Find("Alert")?.GetComponent<SpriteRenderer>();
        shadow = transform.Find("shadow")?.GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private IEnumerator CanThrowTheObject()
    {
        yield return null; // Esto espera un frame
        canThrow = true;
    }

    private IEnumerator SetHittingInFalse()
    {
        float waitingTime = 0.4f; // Aca va el tiempo que dura en hacer la animacion de golpe con el objeto
        yield return new WaitForSeconds(waitingTime);
        isHitting = false;
    }

    private IEnumerator SetCurrentPropInNull()
    {
        float waitingTime = 0.35f; // Aca va el tiempo que dura en hacer la animacion de tirar el objeto
        yield return new WaitForSeconds(waitingTime);
        playerController.CurrentProp = null;
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
        if (hasBeenThrown)
        {
            Enemy currentEnemy = collision.collider.GetComponent<Enemy>();

            if (currentEnemy != null)
            {
                currentEnemy.GetDamage(damage);
                DestroyThisGameObject();
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
        // Buscar todos los sprites que tangan los enemigos, como las manos, etc.
        SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var child in childSprites)
        {
            if (child.gameObject != gameObject)
            {
                child.enabled = false;
            }
        }

        if (hasBeenThrown)
        {
            spriteRenderer.enabled = false;
        }

        boxCollider.enabled = false;
        rb.simulated = false;
        rb.isKinematic = true;

        if (throwSound.isPlaying)
        {
            Destroy(gameObject, throwSound.clip.length);
        }

        else
        {
            Destroy(gameObject, 0.225f); // tiempo en terminar el transcurso de la animacion golpe
        }
    }

    private void Animations()
    {
        anim.SetBool("Throw", hasBeenThrown);

        anim.SetBool("Rotate_Light", pickedUp && weight == "light");
        anim.SetBool("Rotate_heavy", pickedUp && weight == "heavy");
    }
}