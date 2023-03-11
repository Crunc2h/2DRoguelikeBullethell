using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponFunctionalityEnemy : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [Header("Projectile Configuration")]
    [SerializeField] public float projectileForce = 1500f;
    [SerializeField] public float projectileLifetime = 2f;
    [SerializeField] public float fireRate = 3f;
    [SerializeField] public float randomizedShootingRateRange = 0.5f;
    [SerializeField] private float accuracy = 1f;

    [Header("SFX")]
    [SerializeField] private AudioSource pistolFireSFX;
    
    private GameObject projectileClone;
    private GameObject muzzleFlash;
    private GameObject bulletTrail;
    private Quaternion initialProjectileRotation;
    public Vector3 projectileSpawnPos;
    public float currentWeaponRotation;
    public float originalProjectileForce;
    public bool fireCommand = false;
    private bool isFiring = false;

    private void Awake()
    {
        Definitions();       
    }
    private void Update()
    {
        FiringManager();
    }
    private void FiringManager()
    {
        if (fireCommand)
        {
            if (!isFiring)
            {
                StartCoroutine(automaticFiring());
            }
        }
    }
    public void fireProjectileLogic()
    {
        fireProjectileFX();
        calculateProjectilePositionAndRotation();
        projectileClone = Instantiate(projectile, projectileSpawnPos, initialProjectileRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce((Vector2)(initialProjectileRotation * Vector2.right) * projectileForce);
        projectileClone.GetComponent<ProjectileCollisionTrigger>().currentForceAndDirectionOnProjectile = (Vector2)(initialProjectileRotation * Vector2.right) * projectileForce;
        projectileClone.GetComponent<ProjectileCollisionTrigger>().currentProjectileForce = projectileForce;
        projectileClone.GetComponent<ProjectileCollisionTrigger>().originalWeaponProjectileForce = originalProjectileForce;
        StartCoroutine(projectileLifeTime(projectileClone));
        if (projectileClone.GetComponent<Animator>() != null)
        {
            projectileClone.GetComponent<Animator>().SetTrigger("enemyProjectile");
        }
    }
    private void calculateProjectilePositionAndRotation()
    {
        projectileSpawnPos = transform.GetChild(0).gameObject.transform.position;
        initialProjectileRotation = Quaternion.Euler(0f, 0f, currentWeaponRotation + Random.Range(-20f, 20f) * (1 / accuracy));
    }
    private void fireProjectileFX()
    {
        if (pistolFireSFX.loop)
        {
            if (!pistolFireSFX.isPlaying)
            {
                pistolFireSFX.Play();
            }
        }
        else
        {
            pistolFireSFX.Play();
        }
        GetComponent<Animator>().SetTrigger("recoil");
        StartCoroutine(muzzleFlashFX());
        StartCoroutine(bulletTrailFX());
    }
    private IEnumerator automaticFiring()
    {
        isFiring = true;
        while (fireCommand)
        {
            fireProjectileLogic();
            yield return new WaitForSeconds((1 / fireRate) + Random.Range(-randomizedShootingRateRange * (1 / fireRate), randomizedShootingRateRange * (1 / fireRate)));
        }
        if (pistolFireSFX.loop)
        {
            pistolFireSFX.Stop();
        }
        isFiring = false;
    }
    private IEnumerator projectileLifeTime(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(projectile);
    }
    private IEnumerator muzzleFlashFX()
    {
        if(muzzleFlash != null)
        {
            muzzleFlash.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.1f);
            muzzleFlash.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    private IEnumerator bulletTrailFX()
    {
        if(bulletTrail != null)
        {
            bulletTrail.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
            bulletTrail.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    private void Definitions()
    {
        originalProjectileForce = projectileForce;
        Transform[] childComponents = GetComponentsInChildren<Transform>();
        for(int i = 0; i < childComponents.Length; i++)
        {
            if(childComponents[i].gameObject.name == "muzzleFlash")
            {
                muzzleFlash = childComponents[i].gameObject;
            }
            else if (childComponents[i].gameObject.name == "bulletTrail")
            {
                muzzleFlash = childComponents[i].gameObject;
            }
        }
    }
}

