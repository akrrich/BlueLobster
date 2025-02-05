using UnityEngine;

public class EnemyShoot : Enemy
{
    private ObjectPooler bulletEnemyShootPool;

    private float counterForShoot = 0f;


    protected override void InitializeValues()
    {
        GameObject ObjectPools = GameObject.Find("ObjectPools");
        bulletEnemyShootPool = ObjectPools.transform.Find("BulletEnemyShootPool").GetComponent<ObjectPooler>();

        health = 2; 
        speed = 2.5f;
        radius = 8f;

        executeAttackInUpdate = true;
    }

    protected override void AttackPlayer(Collision2D collision2D)
    {
        if (IsPlayerInRangeWithRadius())
        {
            canMove = false;
            rb.velocity = Vector2.zero;

            counterForShoot += Time.deltaTime;
            float timeToShoot = 2f;

            if (counterForShoot >= timeToShoot)
            {
                BulletEnemyShoot bullet = bulletEnemyShootPool.GetObjectFromPool<BulletEnemyShoot>();
                bullet.BulletActive(bulletEnemyShootPool, transform);
             
                counterForShoot = 0f;
                timeToShoot = 0f;
            }
        }

        else
        {
            canMove = true;
        }
    }
}
