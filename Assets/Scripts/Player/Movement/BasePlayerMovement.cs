using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    public float weaponAngle;
    private Vector2 inputScale;
    private Rigidbody2D rb;
    private SpriteRenderer playerSpriteRend;
    private GameObject weaponSlotOne;
    private GameObject weaponSlotTwo;
    private Vector3 previousPosition;
    private float positionUpdateTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSpriteRend = GetComponent<SpriteRenderer>();
        weaponSlotOne = transform.GetChild(0).gameObject;
        weaponSlotTwo = transform.GetChild(1).gameObject;
    }
    private void Update()
    {
        CalculateInputScale();
        CheckMovement();
        //AnimatorTest();

    }
    private void FixedUpdate()
    {
        Move();
    }
    private void CheckMovement()
    {

        if (transform.position != previousPosition)
        {
            GetComponent<Animator>().SetBool("isMoving", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isMoving", false);
        }
    }
    private void CalculateInputScale()
    {
        inputScale = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        /*
        if(inputScale.x != 0 || inputScale.y != 0)
        {
            GetComponent<Animator>().SetBool("isMoving", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isMoving", false);
        }
        */
    }
    private void Move()
    {
        rb.MovePosition((Vector2)transform.position + inputScale * movementSpeed * Time.fixedDeltaTime);
        previousPosition = transform.position;
    }

    public void CalculateAimDirection()
    {
        if(weaponAngle <= -60f && weaponAngle >= -120f)
        {
            GetComponent<Animator>().SetInteger("direction", 5);
        }
        else if(weaponAngle <= -120f && weaponAngle >= -180f || weaponAngle >= 160f && weaponAngle <= 180f)
        {
            if (!playerSpriteRend.flipX)
            {
                playerSpriteRend.flipX = true;
            }
            if (weaponSlotOne.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotOne.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (!weaponSlotTwo.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = true;
            }
            GetComponent<Animator>().SetInteger("direction", 4);
        }
        else if(weaponAngle < 160f && weaponAngle >= 100f)
        {
            if (!playerSpriteRend.flipX)
            {
                playerSpriteRend.flipX = true;
            }
            if (weaponSlotOne.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotOne.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (!weaponSlotTwo.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = true;
            }
            GetComponent<Animator>().SetInteger("direction", 3);
        }
        else if(weaponAngle < 100f && weaponAngle >= 80f)
        {
            GetComponent<Animator>().SetInteger("direction", 2);
        }
        else if(weaponAngle < 80f && weaponAngle >= 20f)
        {
            if (playerSpriteRend.flipX)
            {
                playerSpriteRend.flipX = false;
            }
            if (!weaponSlotOne.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotOne.GetComponent<SpriteRenderer>().enabled = true;
            }
            if (weaponSlotTwo.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = false;
            }
            GetComponent<Animator>().SetInteger("direction", 1);
        }
        else if(weaponAngle < 20f && weaponAngle >= 0f || weaponAngle < 0f && weaponAngle > -60f)
        {
            if(playerSpriteRend.flipX)
            {
                playerSpriteRend.flipX = false;
            }
            if(!weaponSlotOne.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotOne.GetComponent<SpriteRenderer>().enabled = true;
            }
            if(weaponSlotTwo.GetComponent<SpriteRenderer>().enabled)
            {
                weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = false;
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
