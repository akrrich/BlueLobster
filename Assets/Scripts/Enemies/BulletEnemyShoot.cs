using UnityEngine;

public class BulletEnemyShoot : MonoBehaviour
{
    private PlayerController playerController;
    private ObjectPooler bulletEnemyShootPool;

    private Rigidbody2D rb;

    private int damage = 2;

    private float speed = 5f;


    void Awake()
    {
        GetComponents();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        ReturnBulletToPoolWithColision(collider);
    }

                                                                // posicion del arma del enemigo
    public void BulletActive(ObjectPooler bulletEnemyShootPool, Transform bulletPosition)
    {
        this.bulletEnemyShootPool = bulletEnemyShootPool;

        transform.position = bulletPosition.position;
        transform.rotation = bulletPosition.rotation;

        Vector2 direction = (playerController.transform.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }


    private void GetComponents()
    {
        playerController = FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void ReturnBulletToPoolWithColision(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            bulletEnemyShootPool.ReturnObjectToPool(this);
        }
    }
}
