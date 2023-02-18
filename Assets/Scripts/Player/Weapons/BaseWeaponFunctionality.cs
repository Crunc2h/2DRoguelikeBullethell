using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponFunctionality : MonoBehaviour
{
    [SerializeField] GameObject basicProjectile;
    [SerializeField] float projectileForce = 1500f;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float accuracy = 1f;
    [SerializeField] private float projectileLifetime = 2f;
    private GameObject projectileClone;
    private Quaternion initialProjectileRotation;
    private Vector3 projectileSpawnPos;
    private float fireTimer = 0f;
    private bool isFiring = false;
    
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
    }

    private void CalculateProjectilePositionAndRotation()
    {
        if (GameObject.FindGameObjectWithTag("projectileSpawn"))
        {
            projectileSpawnPos = GameObject.FindGameObjectWithTag("projectileSpawn").transform.position;
        }
        initialProjectileRotation = Quaternion.Euler(0f, 0f, 
            GetComponent<BaseAimFunctionality>().weaponRotationAngle + Random.Range(-20f, 20f) * (1 / accuracy));
    }
    private IEnumerator projectileLifeTime(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(projectile);
    }
    private void FireProjectile()
    {
        CalculateProjectilePositionAndRotation();
        projectileClone = Instantiate(basicProjectile, projectileSpawnPos, initialProjectileRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce((Vector2)(initialProjectileRotation * Vector2.right) * projectileForce);
        StartCoroutine(projectileLifeTime(projectileClone));
    }
}
