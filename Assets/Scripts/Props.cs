using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Props : MonoBehaviour
{
    private PlayerController playerController;

    private Rigidbody2D rb;

    [SerializeField] private int damage;
    [SerializeField] private int velocity;
    [SerializeField] private int durability;

    //private Vector3 objectposition;
    [SerializeField] private Vector3 relativePosition;

    private bool hasBeenThrown = false;
    private Enemy currentEnemy;


    void Awake()
    {
        FindPlayerController();
        GetComponents();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(hasBeenThrown)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                Enemy enemy = collision.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    currentEnemy = enemy;
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

    public void PickObject( Transform objectPosition)
    {
        rb.isKinematic = true;
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position =objectPosition.position;
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
                if(playerController.transform.localScale.x == 1) throwDirection = Vector2.right;
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

    private void FindPlayerController()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }

    /* rb.isKinematic = true;
         rb.simulated = false;
         rb.velocity = Vector2.zero;
         rb.angularVelocity = 0f;

         objectposition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

         //ajustar posicion de objeto segun direccion de jugador
         float pruebax = relativePosition.x * playerController.transform.localScale.x;
         relativePosition = new Vector3(pruebax, relativePosition.y, relativePosition.z);

         // Obtener el ángulo de rotación del jugador en grados
         float zRotation = playerController.transform.eulerAngles.z;

         if (zRotation != 90 && zRotation != 270) 
         {
             objectposition = playerController.transform.position + relativePosition;
         }

         else
         {
             if (zRotation == 90)
             {
                 objectposition = new Vector3(playerController.transform.position.x, playerController.transform.position.y + 1.2f, playerController.transform.position.z);
             }
             else if (zRotation == 270) 
             {
                 objectposition = new Vector3(playerController.transform.position.x, playerController.transform.position.y - 1.2f, playerController.transform.position.z);
             }
         }

         transform.SetParent(playerController.transform);*/
}