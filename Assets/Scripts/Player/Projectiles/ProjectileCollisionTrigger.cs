using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionTrigger : MonoBehaviour
{
    public bool projectileCollision = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
