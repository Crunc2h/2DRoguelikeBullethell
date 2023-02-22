using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class InvisibleTurret : MonoBehaviour
{
    //Spawn in a randomized point within a certain area and away from the player for at least the specified distance
    //If said point is on a wall or an obstacle, move it to the closest point to the player
    private LayerMask obstacleLayer;
    private RaycastHit2D collisionCheck;
    private Vector3 spawnPoint;
    private Vector3 playerPos;
    private bool lineOfSight = false;
    private bool isCharging = false;
    int counter = 0;
    //inactivation
    //define spawn area
    //define spawning point
    //activation
    private void Awake()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");
    }
    private void Update()
    {
        lineOfSight = GetComponent<BaseEnemyLogic>().clearLineOfSight;
        Debug.Log(lineOfSight);
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        if(lineOfSight)
        {
            if(!isCharging)
            {
                StartCoroutine(SniperTurretBehavior());
            }
        }
    }
    private void CalculateSpawningPoint()
    {
        do
        {
            spawnPoint = new Vector3(playerPos.x + Random.Range(-30f, 30f), playerPos.y + Random.Range(-30f, 30f), 0f);
        } while ((playerPos - spawnPoint).magnitude < 15f || CollisionCheck(spawnPoint) == true);
        gameObject.transform.position = spawnPoint;
        GetComponent<AIPath>().canMove = true;
    }
    private bool CollisionCheck(Vector3 spawnPoint)
    {
        collisionCheck = Physics2D.Raycast((Vector2)spawnPoint, Vector2.right, 0.1f, obstacleLayer);
        if(collisionCheck.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator SniperTurretBehavior()
    {
        isCharging = true;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<AIPath>().canMove = false;
        yield return new WaitForSeconds(2f);
        GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireProjectileLogic();
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        CalculateSpawningPoint();
        yield return new WaitForSeconds(3f);
        isCharging = false;
    }
}   
