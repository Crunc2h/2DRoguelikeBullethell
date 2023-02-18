using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponFunctionality : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [Header("Projectile Configuration")]
    [SerializeField] private float projectileForce = 1500f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float accuracy = 1f;
    [SerializeField] private float maxAmmo = 20f;
    [SerializeField] private float currentAmmo = 20f;
    [SerializeField] private float reloadDuration = 2f;
    [Header("Camera Shake Configuration")]
    [SerializeField] private float shakeIntensity = 0.07f;
    [SerializeField] private float shakeDuration = 0.02f;
    [Header("SFX")]
    [SerializeField] private AudioSource reloadSFX;
    [SerializeField] private AudioSource emptySFX;
    [SerializeField] private AudioSource pistolFireSFX;
    private GameObject projectileClone;
    private Quaternion initialProjectileRotation;
    private Vector3 projectileSpawnPos;
    private bool lmbDown = false;
    private bool isFiring = false;
    private bool isReloading = false;

 
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lmbDown = true;
            if(isReloading)
            {
                emptySFX.Play();
            }
            if(!isFiring)
            {
                StartCoroutine(automaticFiring());
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lmbDown = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(reloadLogic());
        }
    }
    private IEnumerator automaticFiring()
    {
        isFiring = true;
        while(lmbDown && !isReloading)
        {
            fireProjectileLogic();
            yield return new WaitForSeconds(1 / fireRate);
        }
        isFiring= false;
    }
    private void fireProjectileLogic()
    {
        fireProjectileFX();
        calculateProjectilePositionAndRotation();
        projectileClone = Instantiate(projectile, projectileSpawnPos, initialProjectileRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce((Vector2)(initialProjectileRotation * Vector2.right) * projectileForce);
        StartCoroutine(projectileLifeTime(projectileClone));
        currentAmmo--;
        if (currentAmmo <= 0)
        {
            StartCoroutine(reloadLogic());
        }
    }
    private void calculateProjectilePositionAndRotation()
    {
        if (GameObject.FindGameObjectWithTag("projectileSpawnPoint"))
        {
            projectileSpawnPos = GameObject.FindGameObjectWithTag("projectileSpawnPoint").transform.position;
        }
        initialProjectileRotation = Quaternion.Euler(0f, 0f, GetComponent<BaseAimFunctionality>().weaponRotationAngle + Random.Range(-20f, 20f) * (1 / accuracy));
    }
    private IEnumerator projectileLifeTime(GameObject projectile)
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(projectile);
    }
    private IEnumerator reloadLogic()
    {
        isReloading = true;
        StartCoroutine(reloadFX());
        yield return new WaitForSeconds(reloadDuration);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    private void fireProjectileFX()
    {
        pistolFireSFX.Play();
        GetComponent<Animator>().SetTrigger("recoil");
        StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BasicCamera>().Shake(shakeIntensity, shakeDuration, 0.001f));
        StartCoroutine(muzzleFlash());
        StartCoroutine(bulletTrail());
    }
    private IEnumerator reloadFX()
    {
        reloadSFX.Play();
        GameObject.FindGameObjectWithTag("reloadTimer").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.FindGameObjectWithTag("reloadTimer").transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        GameObject.FindGameObjectWithTag("reloadTimer").transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        GameObject.FindGameObjectWithTag("reloadTimer").transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        GameObject.FindGameObjectWithTag("reloadTimer").GetComponent<Animator>().SetTrigger("reload");
        yield return new WaitForSeconds(reloadDuration);
        GameObject.FindGameObjectWithTag("reloadTimer").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindGameObjectWithTag("reloadTimer").transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindGameObjectWithTag("reloadTimer").transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindGameObjectWithTag("reloadTimer").transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
    private IEnumerator muzzleFlash()
    {
        GameObject.FindGameObjectWithTag("muzzleFlash").GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        GameObject.FindGameObjectWithTag("muzzleFlash").GetComponent<SpriteRenderer>().enabled = false;
    }
    private IEnumerator bulletTrail()
    {
        GameObject.FindGameObjectWithTag("bulletTrail").GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.05f);
        GameObject.FindGameObjectWithTag("bulletTrail").GetComponent<SpriteRenderer>().enabled = false;
    }
}
