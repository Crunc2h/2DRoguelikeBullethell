using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerMovement : MonoBehaviour
{
    public float movementSpeed = 15f;
    public float weaponAngle;
    private Vector2 inputScale;
    private Rigidbody2D rb;
    private SpriteRenderer playerSpriteRend;
    private GameObject weaponSlotOne;
    private GameObject weaponSlotTwo;
    public bool isDashing = false;
    public float dashDuration = 0.33f;
    public float dashCooldown = 1f;
    private Vector2 dashDirection;
    private void Awake()
    {
        Definitions();
    }
    private void Update()
    {
        ListenForPlayerInput();
        DashManager();
        CalculateInputScaleNCheckMovement();
    }
    private void FixedUpdate()
    {
        MoveNDash();
    }
    private void ListenForPlayerInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isDashing && dashCooldown > 0f)
            {
                isDashing = true;
            }
        }
    }
    private void DashManager()
    {    
        if(isDashing)
        {
            dashCooldown -= Time.unscaledDeltaTime;
            dashDuration -= Time.unscaledDeltaTime;
            
            if(!GetComponent<Animator>().GetBool("dash") && dashDuration > 0f)
            {
                dashDirection = GetComponent<BaseAimFunctionality>().weaponAimDirection;
                GetComponent<Animator>().SetBool("dash", true);
                GetComponentInChildren<BaseWeaponFunctionalityPlayer>().isFiring = false;
                GetComponentInChildren<BaseWeaponFunctionalityPlayer>().gameObject.GetComponent<SpriteRenderer>().enabled = false;
                GetComponentInChildren<BaseWeaponFunctionalityPlayer>().enabled = false;
                weaponSlotOne.GetComponent<SpriteRenderer>().enabled = false;
                weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = false;
            }
            else if(GetComponent<Animator>().GetBool("dash") && dashDuration <= 0f)
            {
                weaponSlotOne.GetComponent<SpriteRenderer>().enabled = true;
                weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = true;
                GetComponentInChildren<BaseWeaponFunctionalityPlayer>().enabled = true;
                GetComponentInChildren<BaseWeaponFunctionalityPlayer>().gameObject.GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<Animator>().SetBool("dash", false);
            }
            
            if(dashCooldown <= 0f)
            {
                dashCooldown = 1f;
                dashDuration = 0.33f;
                isDashing = false;
            }
        }
    }
    private void CalculateInputScaleNCheckMovement()
    {
        inputScale = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (inputScale.x != 0 || inputScale.y != 0)
        {
            GetComponent<Animator>().SetBool("isMoving", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isMoving", false);
        }

    }
    private void MoveNDash()
    {
        if (isDashing)
        {
            if (dashDuration > 0f)
            {
                rb.MovePosition((Vector2)transform.position + dashDirection * 30f * Time.fixedDeltaTime);
            }
            else
            {
                rb.MovePosition((Vector2)transform.position + inputScale * movementSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            rb.MovePosition((Vector2)transform.position + inputScale * movementSpeed * Time.fixedDeltaTime);
        }
    }
    public void CalculateAimDirection()
    {
            if (weaponAngle <= -60f && weaponAngle >= -120f)
            {
                GetComponent<Animator>().SetInteger("direction", 5);
            }
            else if (weaponAngle <= -120f && weaponAngle >= -180f || weaponAngle >= 160f && weaponAngle <= 180f)
            {
                SpriteManager(1);
                GetComponent<Animator>().SetInteger("direction", 4);
            }
            else if (weaponAngle < 160f && weaponAngle >= 100f)
            {
                SpriteManager(1);
                GetComponent<Animator>().SetInteger("direction", 3);
            }
            else if (weaponAngle < 100f && weaponAngle >= 80f)
            {
                GetComponent<Animator>().SetInteger("direction", 2);
            }
            else if (weaponAngle < 80f && weaponAngle >= 20f)
            {
                SpriteManager(0);
                GetComponent<Animator>().SetInteger("direction", 1);
            }
            else if (weaponAngle < 20f && weaponAngle >= 0f || weaponAngle < 0f && weaponAngle > -60f)
            {
                SpriteManager(0);
                GetComponent<Animator>().SetInteger("direction", 0);
            }
    }
    public void SpriteManager(int mode)
    {
        if (mode == 0)
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
        }
        else
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
        }
    }
    private void Definitions()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSpriteRend = GetComponent<SpriteRenderer>();
        weaponSlotOne = transform.GetChild(0).gameObject;
        weaponSlotTwo = transform.GetChild(1).gameObject;
    }
}