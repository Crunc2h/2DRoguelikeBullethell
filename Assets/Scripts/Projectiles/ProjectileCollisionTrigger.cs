using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionTrigger : MonoBehaviour
{
    public Vector2 currentForceAndDirectionOnProjectile;
    public float currentProjectileForce;
    public float originalWeaponProjectileForce;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameObject.CompareTag("enemyProjectile"))
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<BasePlayerMovement>().isDashing)
                {
                    //Damage player if they aren't dashing
                    collision.gameObject.GetComponent<BasePlayerHealth>().TakeDamage();
                    Destroy(gameObject);
                }
                else if(collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<BasePlayerMovement>().isDashing 
                    && collision.gameObject.GetComponent<BasePlayerMovement>().dashDuration <= 0f)
                {
                    //Damage player if dash is on cooldown
                    collision.gameObject.GetComponent<BasePlayerHealth>().TakeDamage();
                    Destroy(gameObject);
                }
                else if(collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<BasePlayerMovement>().isDashing
                    && collision.gameObject.GetComponent<BasePlayerMovement>().dashDuration > 0f)
                {
                    //Do nothing while player is dashing
                }
                else
                {
                    //Destroy projectile if it hits a wall
                    Destroy(gameObject);
                }
            }
        }
        else if(gameObject.CompareTag("playerProjectile"))
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Mob"))
            {
                if (collision.gameObject.CompareTag("Mob"))
                {
                    collision.gameObject.GetComponent<BaseEnemyHealth>().TakeDamage();
                }
                Destroy(gameObject);
            }
        }
    }
}
