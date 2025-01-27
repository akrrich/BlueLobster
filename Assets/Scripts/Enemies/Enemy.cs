using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected PlayerController playerController;
    [SerializeField] private Transform[] patrolPoints;

    private Rigidbody2D rb;
    private Animator anim;

    private int currentPointIndex = 0;

    private float speed = 2f;
    private float radius = 3f;

    private Vector2 currentTarget;


    void Awake()
    {
        GetComponents();
    }

    void Update()
    {
        CheckEnemyStates();
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        CheckColisionWithPatrolPoints(collider2D);
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        AttackPlayer(collision2D);
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void CheckEnemyStates()
    {
        if (IsPlayerInRangeWithRadius())
        {
            FollowPlayer();
        }

        else
        {
            Patrol();
        }
    }

    private bool IsPlayerInRangeWithRadius()
    {
        if (playerController != null)
        {
            return Vector2.Distance(transform.position, playerController.transform.position) <= radius;
        }

        else
        {
            return false;   
        }
    }

    // Metodo para mover el enemigo hacia un target como punto o el player
    private void MoveEnemyToTarget(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void CheckColisionWithPatrolPoints(Collider2D collider2D)
    {
        if (collider2D.gameObject == patrolPoints[currentPointIndex].gameObject)
        {
            ChangeRandomPatrolPoint();
        }
    }

    private void Patrol()
    {
        currentTarget = patrolPoints[currentPointIndex].position;
        MoveEnemyToTarget(currentTarget);
    }

    private void FollowPlayer()
    {
        currentTarget = playerController.transform.position;
        MoveEnemyToTarget(currentTarget);
    }

    private int ChangeRandomPatrolPoint()
    {
        int newPointIndex;

        do
        {
            newPointIndex = Random.Range(0, patrolPoints.Length);

        } while (newPointIndex == currentPointIndex);

        return currentPointIndex = newPointIndex;
    }

    public abstract void GetDamage(int damage);

    protected abstract void AttackPlayer(Collision2D collision2D);
}
