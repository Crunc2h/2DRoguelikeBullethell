using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    private bool isFiring = false;
    private Vector2 mouseWorldPosition;
    private Vector2 weaponAimDirection;
    private float firingTimer = 0f;
    private float fireRate = 0.1f;
    private float weaponRotationAngle = 0f;
    private GameObject weaponRotationOrigin;
    private GameObject weapon;
    [SerializeField] private GameObject basicProjectile;
    private GameObject projectileClone;
    private void Awake()
    {
        weaponRotationOrigin = GameObject.FindGameObjectWithTag("WeaponRotationOrigin");
        weapon = GameObject.FindGameObjectWithTag("WeaponRotationOrigin").transform.GetChild(1).gameObject;
    }
    private void Update()
    {
        GetMouseWorldPosition();
        
        CalculateWeaponRotationAngle();
        
        if(Input.GetMouseButtonDown(0))
        {
            isFiring = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            firingTimer = 0f;
            isFiring = false;
        }

        if(isFiring)
        {
            firingTimer += Time.deltaTime;
            if(firingTimer >= fireRate)
            {
                firingTimer = 0f;
                Fire();
            }
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
    private void Fire()
    {
        projectileClone = Instantiate(basicProjectile, weapon.transform.position, weapon.transform.localRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce(weaponAimDirection * 2000f);

    }

    //1 - Rotates the "hand" and weapon towards the mouse icon - partially done
    //2 -Spawns the projectile when left mouse button is clicked, if the button is held down it keeps spawning projectiles with a certain delay in between
    //The delay in between the projectiles will be randomized to a certain degree also the angle of the spawned projectile will be randomized within a range aswell, this is
    //done to avoid perfect accuracy
    //Rotation of the spawned projectile will be 90 to the mouse, and it will move in a certain speed towards that direction
    //It will disappear once it hits an enemy or a wall

    //Spawning the projectile-----------
    //Spawns just a bit in front of the muzzle
    //Projectile should face the same rotation with the weapon

    //Add initial speed to the projectile

    //Destrol the projectile if it exceeds its lifetime or hits a wall
}
