using UnityEngine;

public class Props : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int velocity;
    [SerializeField] private int durability;


    private Vector3 objectposition;
    [SerializeField] private Vector3 relativePosition;
    [SerializeField] private Quaternion rotation;

    private Rigidbody2D rb;


    public void PickObject(Transform playerPosition)
    {
        objectposition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        //ajustar posicion de objeto segun direccion de jugador
        float pruebax = relativePosition.x * playerPosition.localScale.x;
        relativePosition = new Vector3(pruebax, relativePosition.y, relativePosition.z);

        // Obtener el ángulo de rotación del jugador en grados
        float zRotation = playerPosition.eulerAngles.z;

        if (zRotation != 90 && zRotation != 270) 
        {
            objectposition = playerPosition.position + relativePosition;
        }
        else
        {
            if (zRotation == 90)
            {
                objectposition = new Vector3(playerPosition.position.x, playerPosition.position.y + 1.2f, playerPosition.position.z);
            }
            else if (zRotation == 270) 
            {
                objectposition = new Vector3(playerPosition.position.x, playerPosition.position.y - 1.2f, playerPosition.position.z);
            }
        }

        transform.SetParent(playerPosition);
        transform.position = objectposition;
        
    }

    public void ThrowObject(Vector2 direction)
    {
        rb.AddForce(direction.normalized * velocity, ForceMode2D.Impulse);
    }
}