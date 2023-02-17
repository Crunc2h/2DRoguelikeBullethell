using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    [SerializeField] GameObject basicProjectile;
    private GameObject projectileClone;
    private GameObject weaponRotationOrigin;
    private Vector3 initialProjectilePosition;
    private Quaternion initialProjectileRotation;
    private bool isFiring = false;
    private float fireTimer = 0f;
    private float fireRate = 3f;
    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("WeaponRotationOrigin") != null)
        {
            weaponRotationOrigin = GameObject.FindGameObjectWithTag("WeaponRotationOrigin");
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile();
            isFiring = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            fireTimer = 0f;
            isFiring= false;
        }

        if(isFiring)
        {
            fireTimer += Time.deltaTime;
            if(fireTimer > (1f / fireRate))
            {
                fireTimer = 0f;
                FireProjectile();
            }
        }

        initialProjectilePosition = new Vector3(weaponRotationOrigin.transform.GetChild(1).gameObject.transform.position.x + 0.5f,
                                                weaponRotationOrigin.transform.GetChild(1).gameObject.transform.position.y + 0.15f,
                                                weaponRotationOrigin.transform.GetChild(1).gameObject.transform.position.z);
        initialProjectileRotation = Quaternion.Euler(0f, 0f, weaponRotationOrigin.transform.rotation.z);
    }

    private void FireProjectile()
    {
        //Rotation is the rotation of the weapon rotation origins rotation z and 0-0 for x and y
        //Initial position is 2.5f plus on the x and 0.75 on the y axis for the weapon position
        projectileClone = Instantiate(basicProjectile, initialProjectilePosition, initialProjectileRotation);
    }
}
