using UnityEngine;

public class PlayerColisions
{
    private PlayerController playerController;


    public PlayerColisions(PlayerController playerController)
    {
        this.playerController = playerController;
    }


    public void CheckEnterColisionWithEnemy(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && playerController.CurrentProp == null)
        {
            playerController.CanPunch = true;

            if (playerController.IsPunching)
            {
                Enemy currentEnemy = collision.collider.GetComponent<Enemy>();

                if (currentEnemy != null)
                {
                    currentEnemy.GetDamage(playerController.Damage);
                }

                playerController.IsPunching = false;
            }
        }
    }

    public void CheckStayColisionWithEnemy(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && playerController.CurrentProp == null)
        {
            playerController.CanPunch = true;

            if (playerController.IsPunching)
            {
                Enemy currentEnemy = collision.collider.GetComponent<Enemy>();

                if (currentEnemy != null)
                {
                    currentEnemy.GetDamage(playerController.Damage);
                }

                playerController.IsPunching = false;
            }
        }
    }

    public void CheckExitColisionWithEnemy(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && playerController.CurrentProp == null)
        {
            playerController.CanPunch = false;
        }
    }

    public void CheckEnterColisionAndTriggerWithPropToAEnabledButton(Collider2D collider)
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            if (playerController.CurrentProp == null && collider.CompareTag("Objeto"))
            {
                PlayerEvents.OnEnabledPickUpButton?.Invoke();
            }
        }
    }

    public void CheckExitColisionAndTriggerWithPropToDisabledButton(Collider2D collider)
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            if (playerController.CurrentProp == null && collider.CompareTag("Objeto"))
            {
                PlayerEvents.OnDisabledPickUpButton?.Invoke();
            }
        }
    }
}
