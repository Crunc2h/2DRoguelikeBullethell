using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class InvisibleTurret : MonoBehaviour
{
    private LineRenderer lineRendererComp;
    private LayerMask obstacleLayer;
    private RaycastHit2D collisionCheck;
    private Vector3 spawnPoint;
    private Vector3 playerPos;
    private bool lineOfSight = false;
    private bool isCharging = false;
    private bool activateLaser = false;

    private void Awake()
    {
        Definitions();
    }
    private void Update()
    {
        SniperTurretBehaviorManager();
        RenderLaser();
    }

    private void RenderLaser()
    {
        if (activateLaser)
        {
            lineRendererComp.enabled = true;

            if (lineOfSight)
            {
                lineRendererComp.SetPosition(0, GetComponent<BaseAimFunctionality>().weapon.transform.GetChild(0).gameObject.transform.position);
                lineRendererComp.SetPosition(1, playerPos);
            }
            else
            {
                lineRendererComp.SetPosition(0, GetComponent<BaseAimFunctionality>().weapon.transform.GetChild(0).gameObject.transform.position);
                lineRendererComp.SetPosition(1, new Vector3(GetComponent<BaseEnemyLogic>().hitObstacle.point.x, GetComponent<BaseEnemyLogic>().hitObstacle.point.y, -1));
            }
        }
        else
        {
            lineRendererComp.enabled = false;
        }
    }
    private void CalculateSpawningPoint()
    {
        do
        {
            spawnPoint = new Vector3(playerPos.x + Random.Range(-30f, 30f), playerPos.y + Random.Range(-30f, 30f), 0f);
        } while ((playerPos - spawnPoint).magnitude < 15f || CollisionCheck(spawnPoint) == true);
        gameObject.transform.position = spawnPoint;
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
    private void SniperTurretBehaviorManager()
    {
        lineOfSight = GetComponent<BaseEnemyLogic>().clearLineOfSight;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        if (lineOfSight)
        {
            if (!isCharging)
            {
                StartCoroutine(SniperTurretBehavior());
            }
        }
    }
    private IEnumerator SniperTurretBehavior()
    {
        isCharging = true;
        
        //Initial delay
        yield return new WaitForSeconds(0.25f);
        
        //Becomes visible and targettable, starts charging
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BaseAimFunctionality>().weapon.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<AIPath>().canMove = false;
        activateLaser = true;
        
        //Fires the projectile
        yield return new WaitForSeconds(2f);
        GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireProjectileLogic();
        activateLaser = false;
        
        //Becomes invisible and untargettable, teleports to the next spawn point
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BaseAimFunctionality>().weapon.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        CalculateSpawningPoint();
        
        //Cooldown before starting to chase the player
        yield return new WaitForSeconds(3f);
        GetComponent<AIPath>().canMove = true;
        
        isCharging = false;
    }
    private void Definitions()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");
        lineRendererComp = GetComponent<LineRenderer>();
        lineRendererComp.positionCount = 2;
    }
}   