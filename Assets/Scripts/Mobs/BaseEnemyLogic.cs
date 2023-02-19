using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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
    private float raycastDistance = 10f;

    private float dirnChangeDur = 0.5f;
    private float dirChangeTimer = 0f;
    private float movementSpeed = 8f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private bool clearLineOfSight = false;
    private bool manualMovement = false;
    private void Awake()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");
        playerLayer = LayerMask.GetMask("PlayerLayer");

        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        enemySight();

        dirChangeTimer += Time.deltaTime;
        if(dirChangeTimer >= dirnChangeDur)
        {
            dirChangeTimer = 0f;
            dirnChangeDur = Random.Range(0.5f, 1.5f);
            CalculateMovementDirection();
        }
        if (((Vector2)transform.position - (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position).magnitude < 10f && clearLineOfSight)
        {
            manualMovement = true;
            gameObject.GetComponent<SimpleSmoothModifier>().enabled = false;
            gameObject.GetComponent<AIPath>().enabled = false;
            gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        }
        else
        {
            StartCoroutine(gearShiftCooldown());
        }
    }
    private void FixedUpdate()
    {
        if(manualMovement)
        {
            Move();
        }
    }

    private IEnumerator gearShiftCooldown()
    {
        yield return new WaitForSeconds(1f);
        manualMovement = false;
        gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        gameObject.GetComponent<AIPath>().enabled = true;
        gameObject.GetComponent<SimpleSmoothModifier>().enabled = true;
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
            clearLineOfSight = true;
        }
        else
        {
            GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = false;
            clearLineOfSight = false;
        }
    }
    private void CalculateMovementDirection()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }
    private void Move()
    {
        rb.MovePosition((Vector2)transform.position + movementDirection * movementSpeed * Time.fixedDeltaTime);
    }
}
