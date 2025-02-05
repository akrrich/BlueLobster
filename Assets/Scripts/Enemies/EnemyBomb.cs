using UnityEngine;

public class EnemyBomb : Enemy
{
    protected override void InitializeValues()
    {
        health = 1;
        damage = 4;
        speed = 3f;
        radius = 5f;

        executeAttackInUpdate = false;
    }

    protected override void AttackPlayer(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            playerController.GetDamage(damage);
            Destroy(gameObject);
        }
    }
}
