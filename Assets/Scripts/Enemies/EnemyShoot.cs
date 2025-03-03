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

        canExecuteAttackInUpdate = true;
    }

    protected override void AttackPlayer(Collision2D collision2D)
    {
        if (IsPlayerInRangeWithRadius())
        {
            rb.velocity = Vector2.zero;

            counterForShoot += Time.deltaTime;
            float timeToShoot = 2f;

            if (counterForShoot >= timeToShoot && health >= minHealth)
            {
                BulletEnemyShoot bullet = bulletEnemyShootPool.GetObjectFromPool<BulletEnemyShoot>();
                bullet.BulletActive(bulletEnemyShootPool, transform);
             
                counterForShoot = 0f;
                timeToShoot = 0f;
            }
        }
    }

    protected override void SetPropScriptAndProperties()
    {
        Props props = gameObject.AddComponent<Props>();
        props.SetProperties(damage: 5, velocity: 10, durability: 1, weight: "heavy", radius: 1);
    }
}
