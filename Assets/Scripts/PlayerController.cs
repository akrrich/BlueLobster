using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    private float XAxis;
    private float YAxis;

    [SerializeField] private float speed;
    [SerializeField] private int detectionRadius = 5;
    [SerializeField] private LayerMask detectionLayer;

    Props currentProp;

    private bool pickObject = false;

    public Action interactionP;


    void Awake()
    {
        GetComponents();
        SubscribeToGameManagerEvents();
    }
    
    // Simulacion de Update
    void UpdatePlayerController()
    {
        GetAxis();
        CheckNearbyObjects();
        ThrowObject();
    }

    // Simulacion de FixedUpdate
    void FixedUpdatePlayerController()
    {
        PlayerMovement();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnGameStatePlaying -= UpdatePlayerController;
        GameManager.Instance.OnGameStatePlayingFixedUpdate -= FixedUpdatePlayerController;
    }


    private void CheckNearbyObjects()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectionLayer);

        foreach (var obj in objectsInRange)
        {
            if (obj.CompareTag("Objeto"))
            {
                
                if (Input.GetKeyDown(KeyCode.Z) && pickObject == false)
                {
                    currentProp = obj.GetComponent<Props>();
                    if (currentProp != null)
                    {
                        interactionP += () => currentProp.PickObject(transform);
                        ProduceEvent();
                        interactionP -= () => currentProp.PickObject(transform);
                        pickObject = true;
                    }
                }
            }
        }
    }

    // Para visualizar el área de detección (opcional)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    

    private void GetComponents()
        {
           rb = GetComponent<Rigidbody2D>();
        }

    private void ProduceEvent()
    {
        interactionP?.Invoke();
    }

    private void SubscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStatePlaying += UpdatePlayerController;
        GameManager.Instance.OnGameStatePlayingFixedUpdate += FixedUpdatePlayerController;
    }

    private void GetAxis()
    {
        XAxis = Input.GetAxisRaw("Horizontal");
        YAxis = Input.GetAxisRaw("Vertical");
    }

    private void PlayerMovement()
    {
        Vector2 PlayerVelocity = new Vector2(XAxis, YAxis);
        rb.velocity = PlayerVelocity.normalized * speed;

        if (rb.velocity.x != 0 && rb.velocity.y == 0)
        {
            transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            transform.localScale = new Vector2(XAxis, transform.localScale.y);
        }
        else
        {
            if(rb.velocity.y != 0)
            {
                float angle = (YAxis > 0) ? 90f : -90f; // 90° arriba, -90° abajo
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                transform.localScale = new Vector2(1, 1);
            }
        }
    }

    private void ThrowObject()
    {
        if(pickObject == true && Input.GetKeyDown(KeyCode.Z))
        {
            if (transform.localScale.x == 1)
            {
                interactionP += () => currentProp.ThrowObject(Vector2.right);
                ProduceEvent();
                interactionP -= () => currentProp.ThrowObject(Vector2.right);

            }
            else if (transform.localScale.x == -1)
            {
                interactionP += () => currentProp.ThrowObject(Vector2.left);
                ProduceEvent();
                interactionP -= () => currentProp.ThrowObject(Vector2.left);
            }
            else if (transform.rotation.z == 90)
            {
                interactionP += () => currentProp.ThrowObject(Vector2.up);
                ProduceEvent();
                interactionP -= () => currentProp.ThrowObject(Vector2.up);
            }
            else if (transform.rotation.z == -90)
            {
                interactionP += () => currentProp.ThrowObject(Vector2.down);
                ProduceEvent();
                interactionP -= () => currentProp.ThrowObject(Vector2.down);
            }
        }
    }
}
