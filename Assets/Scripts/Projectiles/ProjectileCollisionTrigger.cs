using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameObject.CompareTag("enemyProjectile"))
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<BasePlayerHealth>().TakeDamage();
                }
                Destroy(gameObject);
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
