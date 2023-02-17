using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    private GameObject projectileClone;
    private GameObject weaponRotationOrigin;
    private Vector3 projectileSpawnPos;
    private Quaternion initialProjectileRotation;
    private bool isFiring = false;
    [SerializeField] GameObject basicProjectile;
    [SerializeField] float projectileForce;
    [SerializeField] private float fireRate = 3f;
    private float fireTimer = 0f;
    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("WeaponRotationOrigin") != null)
        {
            weaponRotationOrigin = GameObject.FindGameObjectWithTag("WeaponRotationOrigin");
        }
    }
    private void Update()
    {
        CalculateProjectilePositionAndRotation();
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


        
    }

    private void CalculateProjectilePositionAndRotation()
    {
        if (GameObject.FindGameObjectWithTag("projectileSpawn"))
        {
            projectileSpawnPos = GameObject.FindGameObjectWithTag("projectileSpawn").transform.position;
        }
        initialProjectileRotation = Quaternion.Euler(0f, 0f, weaponRotationOrigin.transform.rotation.z * 180f);
    }

    private void FireProjectile()
    {
        //Rotation is the rotation of the weapon rotation origins rotation z and 0-0 for x and y
        //Initial position is 2.5f plus on the x and 0.75 on the y axis for the weapon position
        projectileClone = Instantiate(basicProjectile, projectileSpawnPos, initialProjectileRotation);
        Debug.Log("WeaponRotationOriginAngle");
        Debug.Log(weaponRotationOrigin.transform.localRotation.z); //WHY IS ORIGIN ROTATION Z AND WEAPON ROTATION ANGLE ARE UNEQUAL???
        Debug.Log("WeaponRotationAngle");
        Debug.Log(GetComponent<PlayerWeaponScript>().weaponRotationAngle);

        //projectileClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(initialProjectileRotation.z), Mathf.Sin(initialProjectileRotation.z)) * projectileForce); 
    }
}
