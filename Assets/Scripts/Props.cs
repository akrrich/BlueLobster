using UnityEngine;

public class Props : MonoBehaviour
{
    private PlayerController playerController;

    private Rigidbody2D rb;

    [SerializeField] private int damage;
    [SerializeField] private int velocity;
    [SerializeField] private int durability;

    private Vector3 objectposition;
    [SerializeField] private Vector3 relativePosition;


    void Awake()
    {
        FindPlayerController();
        GetComponents();
    }


    public void PickObject()
    {
        rb.isKinematic = true;
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
                throwDirection = Vector2.right;
                break;
            case 180:
                throwDirection = Vector2.left;
                break;
            case 90:
                throwDirection = Vector2.up;
                break;
            case 270:
                throwDirection = Vector2.down;
                break;
        }

        rb.AddForce(throwDirection * velocity, ForceMode2D.Impulse);
    }

    public void DropObject()
    {
        rb.isKinematic = true;
        rb.simulated = true;
        transform.SetParent(null);
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
}