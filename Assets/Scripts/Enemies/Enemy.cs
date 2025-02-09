using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected PlayerController playerController;
    [SerializeField] private Transform[] patrolPoints;

    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private int currentPointIndex = 0;
    protected int health;
    protected int minHealth = 1;
    protected int damage;

    protected float speed;
    protected float radius;

    protected bool executeAttackInUpdate;

    protected Vector2 currentTarget;


    void Awake()
    {
        GetComponents();
        InitializeValues();
    }

    void Update()
    {
        CheckEnemyStates();
        DestroyEnemy();
        CheckWhereCanExecuteTheAttack(null);
        Animations();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider2D)
    {
        CheckColisionWithPatrolPoints(collider2D);
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        CheckWhereCanExecuteTheAttack(collision2D);
    }


    public void GetDamage(int damage)
    {
        health -= damage;
    }


    private void GetComponents()
    {
        playerController = FindObjectOfType<PlayerController>();

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();    
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    protected bool IsPlayerInRangeWithRadius()
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

        RotateEnemyDirection(direction);
    }

    protected void RotateEnemyDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
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
        if (health >= minHealth) MoveEnemyToTarget(currentTarget);
    }

    private void FollowPlayer()
    {
        currentTarget = playerController.transform.position;
        if (health >= minHealth) MoveEnemyToTarget(currentTarget);
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

    private void DestroyEnemy()
    {
        if (health < minHealth)
        {     
            Destroy(gameObject, 3f);
        }
    }

    private void CheckWhereCanExecuteTheAttack(Collision2D collision2D)
    {
        if (!executeAttackInUpdate && collision2D != null)
        {
            AttackPlayer(collision2D);
        }

        if (executeAttackInUpdate)
        {
            AttackPlayer(null);
        }
    }

    private void Animations()
    {
        bool isMoving = rb.velocity.sqrMagnitude > 0;
        bool isAlive = health >= minHealth;

        anim.SetBool("Running", isMoving && isAlive);
        anim.SetBool("Idle", !isMoving && isAlive);

        anim.SetBool("Dead", !isAlive);
    }

    protected abstract void InitializeValues();
    protected abstract void AttackPlayer(Collision2D collision2D);
}
