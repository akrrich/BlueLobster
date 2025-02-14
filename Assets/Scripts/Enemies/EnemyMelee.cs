using System.Collections;
using UnityEngine;

public class EnemyMelee : Enemy
{
    private int damageCooldownTime = 2; // aca tendria que ir el tiempo que tarda el enemigo en realizar la animacion de ataque

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

        canExecuteAttackInUpdate = false;
    }

    protected override void AttackPlayer(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DamageCooldown());
        }   
    }

    protected override void SetPropScriptAndProperties()
    {
        Props props = gameObject.AddComponent<Props>();
        props.SetProperties(damage: 5, velocity: 10, durability: 1, weight: 1, radius: 1);
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
