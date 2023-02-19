using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    public void TakeDamage()
    {
        health--;
        if(health <= 0)
        {
            Death();
        }
    }
    private void Death()
    {
        Destroy(gameObject);
    }
}
