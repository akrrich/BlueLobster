using System.Collections;
using UnityEngine;

public class EnemyMelee : Enemy
{
    private int damageCooldownTime = 1;

    private bool isCollidingWithPlayer = false;


    void OnCollisionStay2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
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
            Destroy(playerController.gameObject);
        }
    }
}
