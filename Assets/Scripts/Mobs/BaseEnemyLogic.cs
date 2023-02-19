using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyLogic : MonoBehaviour
{
    //Aims at the player
    //Walks towards the player, but uses pathfinding to move around obstacles
    //Stops when it is within a certain distance and starts shooting (only if it has clear line of sight, it keeps coming closer if it doesn't)

    private RaycastHit2D hitPlayer;
    private RaycastHit2D hitObstacle;
    private LayerMask playerLayer;
    private LayerMask obstacleLayer;
    private Vector2 weaponDirection;
    [SerializeField] private float raycastDistance = 50f;
    private void Awake()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");
        playerLayer = LayerMask.GetMask("PlayerLayer");
    }
    private void Update()
    {
        enemySight();
    }

    private void enemySight()
    {
        weaponDirection = GetComponent<BaseAimFunctionality>().weaponAimDirection;
        hitPlayer = Physics2D.Raycast((Vector2)transform.position, weaponDirection, raycastDistance, playerLayer);
        if (hitPlayer.collider != null)
        {
            Debug.DrawLine(transform.position, hitPlayer.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, ((Vector2)transform.position + weaponDirection * raycastDistance), Color.green);
        }
        hitObstacle = Physics2D.Raycast((Vector2)transform.position, weaponDirection, raycastDistance, obstacleLayer);
        if (hitObstacle.collider != null)
        {
            Debug.DrawLine(transform.position, hitObstacle.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, ((Vector2)transform.position + weaponDirection * raycastDistance), Color.green);
        }

        if(hitPlayer.collider != null && hitObstacle.collider == null)
        {
            GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = true;
        }
        else
        {
            GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = false;
        }
    }
}
