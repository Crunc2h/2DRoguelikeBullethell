using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    private bool leftMouseButtonDown = false;
    private Vector2 mouseWorldPosition;
    private Vector2 weaponAimDirection;
    private float weaponRotationAngle = 0f;
    private GameObject weaponRotationOrigin;
    private void Awake()
    {
        weaponRotationOrigin = GameObject.FindGameObjectWithTag("WeaponRotationOrigin");
    }
    private void Update()
    {
        GetMouseWorldPosition();
        CalculateWeaponRotationAngle();
        if(Input.GetMouseButtonDown(0))
        {
            leftMouseButtonDown = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            leftMouseButtonDown = false;
        }
    }
    private void FixedUpdate()
    {
        SetWeaponRotation();
    }
    private void SetWeaponRotation()
    {
        weaponRotationOrigin.transform.eulerAngles = new Vector3(0f, 0f, weaponRotationAngle);
        
        if (weaponRotationAngle > 90f && weaponRotationAngle < 180f)
        {
            weaponRotationOrigin.transform.GetChild(1).gameObject.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
        }
        else if(weaponRotationAngle > -180f && weaponRotationAngle < -90f)
        {
            weaponRotationOrigin.transform.GetChild(1).gameObject.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
        }
        else
        {
            weaponRotationOrigin.transform.GetChild(1).gameObject.transform.localRotation = Quaternion.Euler(0f,0f,0f);
        }

    }
    private void GetMouseWorldPosition()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    }
    private void CalculateWeaponRotationAngle()
    {
        weaponAimDirection = (mouseWorldPosition - (Vector2)transform.position).normalized;
        weaponRotationAngle = Mathf.Atan2(weaponAimDirection.y, weaponAimDirection.x) * Mathf.Rad2Deg;
    }

    //Rotates the "hand" and weapon towards the mouse icon
    //Spawns the projectile when left mouse button is clicked, if the button is held down it keeps spawning projectiles with a certain delay in between
    //The delay in between the projectiles will be randomized to a certain degree also the angle of the spawned projectile will be randomized within a range aswell, this is
    //done to avoid perfect accuracy
    //Rotation of the spawned projectile will be 90 to the mouse, and it will move in a certain speed towards that direction
    //It will disappear once it hits an enemy or a wall

 
}
