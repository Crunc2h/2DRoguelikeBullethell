using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    public float weaponAngle;
    private Vector2 inputScale;
    private Rigidbody2D rb;
    string currentDirection;

    private SpriteRenderer spriteRen;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRen = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        CalculateInputScale();
        //AnimatorTest();
        
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void CalculateInputScale()
    {
        inputScale = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(inputScale.x != 0 || inputScale.y != 0)
        {
            GetComponent<Animator>().SetBool("isMoving", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isMoving", false);
        }
    }
    private void Move()
    {
        rb.MovePosition((Vector2)transform.position + inputScale * movementSpeed * Time.fixedDeltaTime);
    }

    public void CalculateAimDirection()
    {
        if(weaponAngle <= -60f && weaponAngle >= -120f)
        {
            currentDirection = "south";
            GetComponent<Animator>().SetInteger("direction", 5);
        }
        else if(weaponAngle <= -120f && weaponAngle >= -180f || weaponAngle >= 160f && weaponAngle <= 180f)
        {
            currentDirection = "west";
            GetComponent<SpriteRenderer>().flipX
            GetComponent<Animator>().SetInteger("direction", 4);
        }
        else if(weaponAngle < 160f && weaponAngle >= 100f)
        {
            currentDirection = "northwest";
            GetComponent<Animator>().SetInteger("direction", 3);
        }
        else if(weaponAngle < 100f && weaponAngle >= 80f)
        {
            currentDirection = "north";
            GetComponent<Animator>().SetInteger("direction", 2);
        }
        else if(weaponAngle < 80f && weaponAngle >= 20f)
        {
            currentDirection = "northeast";
            GetComponent<Animator>().SetInteger("direction", 1);
        }
        else if(weaponAngle < 20f && weaponAngle >= 0f || weaponAngle < 0f && weaponAngle > -60f)
        {
            currentDirection = "east";
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
            }
            GetComponent<Animator>().SetInteger("direction", 0);
        }
    }
    /*
    private void AnimatorTest()
    {
        switch(currentDirection)
        {
            case "north":
                {
                    if(isMoving)
                    {
                        Debug.Log("Facing north, moving");
                    }
                    else
                    {
                        Debug.Log("Facing north, stationary");
                    }
                    break;
                }
            case "northeast":
                {
                    if (isMoving)
                    {
                        Debug.Log("Facing northeast, moving");
                    }
                    else
                    {
                        Debug.Log("Facing northeast, stationary");
                    }
                    break;
                }
            case "northwest":
                {
                    if (isMoving)
                    {
                        Debug.Log("Facing northwest, moving");
                    }
                    else
                    {
                        Debug.Log("Facing northwest, stationary");
                    }
                    break;
                }
            case "east":
                {
                    if (isMoving)
                    {
                        Debug.Log("Facing east, moving");
                    }
                    else
                    {
                        Debug.Log("Facing east, stationary");
                    }
                    break;
                }
            case "west":
                {
                    if (isMoving)
                    {
                        Debug.Log("Facing west, moving");
                    }
                    else
                    {
                        Debug.Log("Facing west, stationary");
                    }
                    break;
                }
            case "south":
                {
                    if (isMoving)
                    {
                        Debug.Log("Facing south, moving");
                    }
                    else
                    {
                        Debug.Log("Facing south, stationary");
                    }
                    break;
                }
            default:
                {
                    break;
                }

        }
    }
    */
}
