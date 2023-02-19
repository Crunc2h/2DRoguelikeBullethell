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

    [Header("SFX")]
    [SerializeField] private AudioSource pistolFireSFX;
    private GameObject projectileClone;
    private GameObject muzzleFlash;
    private GameObject bulletTrail;
    private Quaternion initialProjectileRotation;
    private Vector3 projectileSpawnPos;
    private bool stopFiring = true;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            stopFiring = !stopFiring;
            StartCoroutine(automaticFiring());
        }
        if (transform.GetChild(2).gameObject.CompareTag("muzzleFlash"))
        {
            muzzleFlash = transform.GetChild(2).gameObject;
        }
        if (transform.GetChild(3).gameObject.CompareTag("bulletTrail"))
        {
            bulletTrail = transform.GetChild(3).gameObject;
        }
    }
    private IEnumerator automaticFiring()
    {
        while (!stopFiring)
        {
            fireProjectileLogic();
            yield return new WaitForSeconds((1 / fireRate) + Random.Range(-0.5f * (1 / fireRate), 0.5f * (1 / fireRate)));
        }
    }
    private void fireProjectileLogic()
    {
        fireProjectileFX();
        calculateProjectilePositionAndRotation();
        projectileClone = Instantiate(projectile, projectileSpawnPos, initialProjectileRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce((Vector2)(initialProjectileRotation * Vector2.right) * projectileForce);
        StartCoroutine(projectileLifeTime(projectileClone));
    }
    private void calculateProjectilePositionAndRotation()
    {
        if (gameObject.transform.GetChild(1).CompareTag("projectileSpawnPoint"))
        {
            projectileSpawnPos = transform.GetChild(1).gameObject.transform.position;
        }
        initialProjectileRotation = Quaternion.Euler(0f, 0f, 
            GameObject.FindGameObjectWithTag("Mob").GetComponent<BaseAimFunctionality>().weaponRotationAngle + Random.Range(-20f, 20f) * (1 / accuracy));
    }
    private IEnumerator projectileLifeTime(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(projectile);
    }
    private void fireProjectileFX()
    {
        pistolFireSFX.Play();
        GetComponent<Animator>().SetTrigger("recoil");
        StartCoroutine(muzzleFlashFX());
        StartCoroutine(bulletTrailFX());
    }

    private IEnumerator muzzleFlashFX()
    {
        muzzleFlash.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.GetComponent<SpriteRenderer>().enabled = false;
    }
    private IEnumerator bulletTrailFX()
    {
        bulletTrail.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.05f);
        bulletTrail.GetComponent<SpriteRenderer>().enabled = false;
    }
}

