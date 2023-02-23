using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponFunctionalityEnemy : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [Header("Projectile Configuration")]
    [SerializeField] private float projectileForce = 1500f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float accuracy = 1f;
    [SerializeField] private float randomizedShootingRateRange = 0.5f;

    [Header("SFX")]
    [SerializeField] private AudioSource pistolFireSFX;
    private GameObject projectileClone;
    private GameObject muzzleFlash;
    private GameObject bulletTrail;
    public float currentWeaponRotation;
    private Quaternion initialProjectileRotation;
    public Vector3 projectileSpawnPos;
    public bool fireCommand = false;
    private bool isFiring = false;

    private void Awake()
    {
        /*
        if (transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.CompareTag("muzzleFlash"))
        {
            muzzleFlash = transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        }
        if (transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.CompareTag("bulletTrail"))
        {
            bulletTrail = transform.GetChild(2).gameObject.transform.GetChild(0).gameObject;
        }
        */
    }
    private void Update()
    {
        if(fireCommand)
        {
            if(!isFiring)
            {
                StartCoroutine(automaticFiring());
            }
        }
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
    public void fireProjectileLogic()
    {
        fireProjectileFX();
        calculateProjectilePositionAndRotation();
        projectileClone = Instantiate(projectile, projectileSpawnPos, initialProjectileRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce((Vector2)(initialProjectileRotation * Vector2.right) * projectileForce);
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
    private IEnumerator projectileLifeTime(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(projectile);
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
}

