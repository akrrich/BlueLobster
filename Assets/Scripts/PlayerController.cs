using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    private float XAxis;
    private float YAxis;

    [SerializeField] private float speed;


    void Awake()
    {
        GetComponents();
    }
    
    void Update()
    {
        GetAxis();
    }

    void FixedUpdate()
    {
        PlayerMovement();
    }


    private void GetComponents()
    {
       rb = GetComponent<Rigidbody2D>();
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


        if (rb.velocity.x != 0)
        {
            transform.localScale = new Vector2(XAxis, transform.localScale.y);
        }
    }
}
