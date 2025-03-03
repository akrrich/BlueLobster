using UnityEngine;
using System.Collections;

public class EnemyDash : Enemy
{
    // HAY QUE AGREGAR QUE SI ESTA POR DETECTAR UNA COLISION EN UN RADIO
    // MUY CHIQUITO QUE EL TRIGGER DEL COLLIDER SE PONGA EN FALSE

    private float counterForDash = 0f;

    private bool isDashing = false;

    private Vector2 dashDirection;


    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        base.OnTriggerEnter2D(collider2D);

        CheckEnterColisionWithPlayer(collider2D);
        CheckColisionWithScenary(collider2D);
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        CheckExitColisionWithPlayer(collider2D);
    }


    protected override void InitializeValues()
    {
        health = 2;
        damage = 2;
        speed = 2.5f;
        radius = 8f;

        canExecuteAttackInUpdate = true;
    }

    protected override void AttackPlayer(Collision2D collision2D)
    {
        if (IsPlayerInRangeWithRadius() && !isDashing)
        {
            rb.velocity = Vector2.zero;
            boxCollider.isTrigger = true;
            isDashing = true;
            counterForDash = 0f;

            dashDirection = (playerController.transform.position - transform.position).normalized;
        }

        if (isDashing)
        {
            rb.velocity = Vector2.zero;

            counterForDash += Time.deltaTime;
            float timeToDash = 0.25f;

            if (counterForDash >= timeToDash)
            {
                float dashImpulse = 25f;
                rb.velocity = dashDirection * dashImpulse;

                Invoke(nameof(GradualStop), 0.5f);
            }
        }
    }

    protected override void SetPropScriptAndProperties()
    {
        Props props = gameObject.AddComponent<Props>();
        props.SetProperties(damage: 5, velocity: 10, durability: 1, weight: "light", radius: 1);
    }


    private void GradualStop()
    {
        StartCoroutine(SlowDown());
    }

    private IEnumerator SlowDown()
    {
        float duration = 0.5f;
        float timeElapsed = 0f;
        float friction = 0.9f;

        while (timeElapsed < duration)
        {
            rb.velocity *= friction; 
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        boxCollider.isTrigger = false;
    }

    private void CheckColisionWithScenary(Collider2D collider2D)
    {
        if (!collider2D.gameObject.CompareTag("Player"))
        {
            boxCollider.isTrigger = false;
        }
    }

    private void CheckEnterColisionWithPlayer(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag("Player"))
        {
            playerController.GetDamage(damage);
        }
    }

    private void CheckExitColisionWithPlayer(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag("Player"))
        {
            boxCollider.isTrigger = false;
        }
    }
}
