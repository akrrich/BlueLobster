using System.Collections;
using UnityEngine;

public class EnemyMelee : Enemy
{
    // aca tendria que ir el tiempo que tarda el enemigo en realizar la animacion de ataque
    private int damageCooldownTime = 2;

    private bool isCollidingWithPlayer = false;


    void OnCollisionStay2D(Collision2D collision2D)
    {
        CheckStayColisionWithPlayer(collision2D);
    }

    void OnCollisionExit2D(Collision2D collision2D)
    {
        CheckExitColisionWithPlayer(collision2D);
    }


    protected override void InitializeValues()
    {
        health = 1;
        damage = 1;
        speed = 2f;
        radius = 3f;

        executeAttackInUpdate = false;
    }

    protected override void AttackPlayer(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DamageCooldown());
        }   
    }


    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldownTime);
        
        if (isCollidingWithPlayer)
        {
            playerController.GetDamage(damage);
        }
    }

    private void CheckStayColisionWithPlayer(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    private void CheckExitColisionWithPlayer(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }
}
