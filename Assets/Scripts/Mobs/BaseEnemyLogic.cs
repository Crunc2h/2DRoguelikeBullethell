using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BaseEnemyLogic : MonoBehaviour
{
    [Header("Raycast Variables")]
    [SerializeField] private float raycastRange = 10f;
    public RaycastHit2D hitPlayer;
    public RaycastHit2D hitObstacle;
    private LayerMask playerLayer;
    private LayerMask obstacleLayer;
    private Vector2 weaponDirection;
    

    [Header("Movement Variables")]
    [SerializeField] private float movementSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private float dirChangeTimer = 0f;
    private float distanceToPlayer;
    private float controlChangeDistance = 10f;
    private float dirChangeDur;
    private bool manualMovement = false;
    private bool shiftOnCooldown = false;
    public bool clearLineOfSight = false;
    private void Awake()
    {
        GetComponent<AIDestinationSetter>().target = GameObject.FindGameObjectWithTag("Player").transform;
        obstacleLayer = LayerMask.GetMask("Obstacle");
        playerLayer = LayerMask.GetMask("PlayerLayer");
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        enemySight();
        if(gameObject.name == "BasicTurret")
        {
            //Basic Turret Behavior
            AIvsManualMovementControlManager();
        }
        if(gameObject.name == "SecurityGuard")
        {
            SecurityGuardBehavior();
        }
    }
    private void FixedUpdate()
    {
        if(gameObject.name == "BasicTurret")
        {
            if (manualMovement)
            {
                MovementDirectionChangeManager();
                Move();
            }
        }
    }
    private void SecurityGuardBehavior()
    {
        distanceToPlayer = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude;
        if (distanceToPlayer < 8f)
        {
            GetComponent<AIPath>().maxSpeed = 2;
        }
        else
        {
            GetComponent<AIPath>().maxSpeed = 5;
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
                    if(gameObject.name == "SecurityGuard" || gameObject.name == "BasicTurret")
                    {
                        GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = true;
                    }
                    clearLineOfSight = true;
                }
                else
                {
                    if(gameObject.name == "SecurityGuard" || gameObject.name == "BasicTurret")
                    {
                        GetComponentInChildren<BaseWeaponFunctionalityEnemy>().fireCommand = false;
                    }
                    clearLineOfSight = false;
                }
            }
            else
            {
                if(gameObject.name == "SecurityGuard" || gameObject.name == "BasicTurret")
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
    private void MovementDirectionChangeManager()
    {
        dirChangeTimer += Time.fixedDeltaTime;
        if (dirChangeTimer >= dirChangeDur)
        {
            dirChangeTimer = 0f;
            dirChangeDur = Random.Range(0.5f, 2f);
            CalculateMovementDirection();
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
