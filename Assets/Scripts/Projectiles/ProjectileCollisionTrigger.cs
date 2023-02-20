using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionTrigger : MonoBehaviour
{
    public bool projectileCollision = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Mob"))
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<BasePlayerHealth>().TakeDamage();
            }
            if(collision.gameObject.CompareTag("Mob"))
            {
                collision.gameObject.GetComponent<BaseEnemyHealth>().TakeDamage();
            }
            Destroy(gameObject);
        }
    }

}
