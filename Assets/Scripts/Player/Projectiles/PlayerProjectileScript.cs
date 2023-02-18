using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    [SerializeField] GameObject basicProjectile;
    [SerializeField] float projectileForce = 1500f;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float accuracy = 1f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private float maxAmmo = 20f;
    [SerializeField] private float reloadDuration = 2f;
    private float currentAmmo = 20f;
    private GameObject projectileClone;
    private Quaternion initialProjectileRotation;
    private Vector3 projectileSpawnPos;
    private float fireTimer = 0f;
    private bool isFiring = false;
    private bool isReloading = false;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!isReloading)
            {
                fireProjectile();
            }
            isFiring = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            fireTimer = 0f;
            isFiring= false;
        }

        if(Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(reload());
        }

        if(isFiring && !isReloading)
        {
            fireTimer += Time.deltaTime;
            if(fireTimer > (1f / fireRate))
            {
                fireTimer = 0f;
                fireProjectile();
            }
        }     
    }

    private void calculateProjectilePositionAndRotation()
    {
        if (GameObject.FindGameObjectWithTag("projectileSpawn"))
        {
            projectileSpawnPos = GameObject.FindGameObjectWithTag("projectileSpawn").transform.position;
        }
        initialProjectileRotation = Quaternion.Euler(0f, 0f, 
            GetComponent<PlayerWeaponScript>().weaponRotationAngle + Random.Range(-20f, 20f) * (1 / accuracy));
    }
    private IEnumerator projectileLifeTime(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(projectile);
    }
    private IEnumerator reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDuration);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    private void fireProjectile()
    {
        calculateProjectilePositionAndRotation();
        projectileClone = Instantiate(basicProjectile, projectileSpawnPos, initialProjectileRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce((Vector2)(initialProjectileRotation * Vector2.right) * projectileForce);
        StartCoroutine(projectileLifeTime(projectileClone));
        currentAmmo--;
        if(currentAmmo <= 0)
        {
            StartCoroutine(reload());
        }
    }
}
//Ammo / Reload logic
//VFX - Camera shake, recoil animation, muzzle flash, reload slider /animation?
//SFX - Fire sfx, empty sfx, reload sfx