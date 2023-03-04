using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BaseEnemyLogic : MonoBehaviour
{
    [Header("Raycast Variables")]
    [SerializeField] private float raycastRange = 10f;
    private RaycastHit2D collisionCheck;
    public RaycastHit2D hitPlayer;
    public RaycastHit2D hitObstacle;
    private LayerMask playerLayer;
    private LayerMask obstacleLayer;
    private Vector2 weaponDirection;
    

    [Header("Movement Variables")]
    public float movementSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementDirection = new Vector2(1, 1);
    private Vector3 nextPosition;
    private float dirChangeTimer = 0f;
    private float distanceToPlayer;
    private float controlChangeDistance = 10f;
    private float dirChangeDur = 0.3f;
    private bool manualMovement = false;
    private bool movementSlowed = false;
    private bool shiftOnCooldown = false;
    public bool clearLineOfSight = false;
    private void Awake()
    {
        Definitions();
    }
    private void Update()
    {
        enemySight();
        EnemyTypeManager();
    }
    private void FixedUpdate()
    {
        if(gameObject.name == "BasicTurret")
        {
            BasicTurretMovementManager();
        }
    }
    
    //Common Enemy Behavior
    private void EnemyTypeManager()
    {
        if (gameObject.name == "BasicTurret")
        {
            //Speed management for time slowdown
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().isTimeSlowed && !movementSlowed)
            {
                movementSlowed = true;
                GetComponent<AIPath>().maxSpeed = 5 * GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().timeSlowDownFactor;
                movementSpeed = movementSpeed * GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().timeSlowDownFactor;
            }
            else if(!GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().isTimeSlowed && movementSlowed)
            {
                movementSlowed= false;
                GetComponent<AIPath>().maxSpeed = 5;
                movementSpeed = movementSpeed * (1 / GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().timeSlowDownFactor);
            }
            //Basic Turret Behavior
            AIvsManualMovementControlManager();
        }
        else if (gameObject.name == "SecurityGuard")
        {
            SecurityGuardBehavior();
        }
    }
    private void SecurityGuardBehavior()
    {
        distanceToPlayer = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude;
        if (distanceToPlayer < 8f)
        {
            if(GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().isTimeSlowed && !movementSlowed)
            {
                GetComponent<AIPath>().maxSpeed = 2 * GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().timeSlowDownFactor;
            }
            else if(!GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().isTimeSlowed && movementSlowed)
            {
                GetComponent<AIPath>().maxSpeed = 2;
            }
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().isTimeSlowed && !movementSlowed)
            {
                GetComponent<AIPath>().maxSpeed = 5 * GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().timeSlowDownFactor;
            }
            else if(!GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().isTimeSlowed && movementSlowed)
            {
                GetComponent<AIPath>().maxSpeed = 5;
            }
        }
    }
    private void enemySight()
    {
        weaponDirection = GetComponent<BaseAimFunctionality>().weaponAimDirection;

        hitPlayer = Physics2D.Raycast((Vector2)transform.position, weaponDirection, raycastRange, playerLayer);
        hitObstacle = Physics2D.Raycast((Vector2)transform.position, weaponDirection, raycastRange, obstacleLayer);

        if (hitPlayer.collider != null && hitObstacle.collider == null)
        {
            if (gameObject.name == "SecurityGuard" || gameObject.name == "BasicTurret")
            {
                GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = true;
            }
            clearLineOfSight = true;
        }
        else
        {
            if (hitObstacle.collider != null && hitPlayer.collider != null)
            {
                if (((Vector2)transform.position - hitObstacle.point).magnitude > ((Vector2)transform.position - hitPlayer.point).magnitude)
                {
                    if (gameObject.name == "SecurityGuard" || gameObject.name == "BasicTurret")
                    {
                        GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = true;
                    }
                    clearLineOfSight = true;
                }
                else
                {
                    if (gameObject.name == "SecurityGuard" || gameObject.name == "BasicTurret")
                    {
                        GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = false;
                    }
                    clearLineOfSight = false;
                }
            }
            else
            {
                if (gameObject.name == "SecurityGuard" || gameObject.name == "BasicTurret")
                {
                    GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = false;
                }
                clearLineOfSight = false;
            }
        }
        //DEBUG//DEBUG//DEBUG//DEBUG//DEBUG
        if (hitPlayer.collider != null)
        {
            Debug.DrawLine(transform.position, hitPlayer.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, ((Vector2)transform.position + weaponDirection * raycastRange), Color.green);
        }
        //DEBUG//DEBUG//DEBUG//DEBUG//DEBUG
        if (hitObstacle.collider != null)
        {
            Debug.DrawLine(transform.position, hitObstacle.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, ((Vector2)transform.position + weaponDirection * raycastRange), Color.green);
        }
        //DEBUG//DEBUG//DEBUG//DEBUG//DEBUG
    }

    
    //Basic Turret Behavior
    private void BasicTurretMovementManager()
    {
        if (manualMovement)
        {
            MovementDirectionChangeManager();
            Move();
        }
    }
    private void AIvsManualMovementControlManager()
    {
        distanceToPlayer = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude;
        if (distanceToPlayer < controlChangeDistance && clearLineOfSight && !manualMovement)
        {
            manualMovement = true;
            gameObject.GetComponent<SimpleSmoothModifier>().enabled = false;
            gameObject.GetComponent<AIPath>().enabled = false;
            gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        }
        else if(!clearLineOfSight || distanceToPlayer > controlChangeDistance)
        {
            if(manualMovement && !shiftOnCooldown)
            {
                StartCoroutine(MovementShiftCooldown());
            }
        }
    }
    private void MovementDirectionChangeManager()
    {
        dirChangeTimer += Time.fixedDeltaTime;
        if (dirChangeTimer >= dirChangeDur)
        {
            dirChangeTimer = 0f;
            dirChangeDur = Random.Range(0.5f, 0.8f);
            CalculateMovementDirection();
        }
    }
    private void CalculateMovementDirection()
    {
        if(distanceToPlayer > 5f)
        {
           
            do
            {
                movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                nextPosition = (Vector2)transform.position + movementDirection * movementSpeed;
            } while ((nextPosition - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude 
            > (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude || CollisionCheck(nextPosition) == true);
        }
        else
        {
            do
            {
                movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                nextPosition = (Vector2)transform.position + movementDirection * movementSpeed;
            } while ((nextPosition - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude < 
            (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude || CollisionCheck(nextPosition) == true);
           
        }
           

    }
    private void Move()
    {
        rb.MovePosition((Vector2)transform.position + movementDirection * movementSpeed * Time.fixedDeltaTime);
    }
    private bool CollisionCheck(Vector3 nextDestination)
    {
        collisionCheck = Physics2D.Raycast((Vector2)nextDestination, Vector2.right, 0.1f, obstacleLayer);
        if (collisionCheck.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private IEnumerator MovementShiftCooldown()
    {
        shiftOnCooldown = true;
        yield return new WaitForSeconds(1f);
        shiftOnCooldown = false;
        manualMovement = false;
        gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        gameObject.GetComponent<AIPath>().enabled = true;
        gameObject.GetComponent<SimpleSmoothModifier>().enabled = true;
    }
    
    
    private void Definitions()
    {
        GetComponent<AIDestinationSetter>().target = GameObject.FindGameObjectWithTag("Player").transform;
        obstacleLayer = LayerMask.GetMask("Obstacle");
        playerLayer = LayerMask.GetMask("PlayerLayer");
        rb = GetComponent<Rigidbody2D>();
    }
}
