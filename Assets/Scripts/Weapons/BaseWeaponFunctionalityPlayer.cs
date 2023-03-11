using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponFunctionalityPlayer : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [Header("Projectile Configuration")]
    [SerializeField] public float projectileForce = 1500f;
    [SerializeField] public float projectileLifetime = 2f;
    [SerializeField] public float fireRate = 3f;
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
    private GameObject reloadTimer;
    private GameObject muzzleFlash;
    private GameObject bulletTrail;
    private Quaternion initialProjectileRotation;
    private Vector3 projectileSpawnPos;
    public float originalProjectileForce;
    private bool lmbDown = false;
    private bool isFiring = false;
    private bool isReloading = false;

    private void Awake()
    {
        Definitions();
    }
    private void Update()
    {
        ListenForPlayerInput();
    }
    private void ListenForPlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lmbDown = true;
            if (isReloading)
            {
                emptySFX.Play();
            }
            if (!isFiring)
            {
                StartCoroutine(automaticFiring());
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (pistolFireSFX.loop)
            {
                pistolFireSFX.Stop();
            }
            lmbDown = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(reloadLogic());
        }
    }
    private void fireProjectileLogic()
    {
        fireProjectileFX();
        
        //Spawn projectile and add force to it
        calculateProjectilePositionAndRotation();
        projectileClone = Instantiate(projectile, projectileSpawnPos, initialProjectileRotation);
        projectileClone.GetComponent<Rigidbody2D>().AddForce((Vector2)(initialProjectileRotation * Vector2.right) * projectileForce);
        projectileClone.GetComponent<ProjectileCollisionTrigger>().currentForceAndDirectionOnProjectile = (Vector2)(initialProjectileRotation * Vector2.right) * projectileForce;
        projectileClone.GetComponent<ProjectileCollisionTrigger>().currentProjectileForce = projectileForce;
        projectileClone.GetComponent<ProjectileCollisionTrigger>().originalWeaponProjectileForce = originalProjectileForce;
        //Checks and plays if projectile has a spawn animation
        if (projectileClone.GetComponent<Animator>() != null)
        {
            projectileClone.GetComponent<Animator>().SetTrigger("playerProjectile");
        }
        
        StartCoroutine(projectileLifeTime(projectileClone));
        currentAmmo--;
        
        //Automatic reload
        if (currentAmmo <= 0)
        {
            StartCoroutine(reloadLogic());
        }
    }
    private void calculateProjectilePositionAndRotation()
    {
        projectileSpawnPos = transform.GetChild(0).gameObject.transform.position;
        initialProjectileRotation = Quaternion.Euler(0f, 0f, GameObject.FindGameObjectWithTag("Player").GetComponent<BaseAimFunctionality>().weaponRotationAngle 
            + Random.Range(-30f, 30f) * (1 / accuracy));
    }
    private void fireProjectileFX()
    {
        //Adjust according to sfx being a looping or a single shot mp3
        if(pistolFireSFX.loop)
        {
            if(!pistolFireSFX.isPlaying)
            {
                pistolFireSFX.Play();
            }
        }
        else
        {
            pistolFireSFX.Play();
        }
        
        //Commence animation + other effects
        GetComponent<Animator>().SetTrigger("recoil");
        StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BasicCamera>().Shake(shakeIntensity, shakeDuration, 0.001f));
        StartCoroutine(muzzleFlashFX());
        StartCoroutine(bulletTrailFX());
    }
    private IEnumerator automaticFiring()
    {
        isFiring = true;

        while (lmbDown && !isReloading)
        {
            fireProjectileLogic();
            yield return new WaitForSeconds(1 / fireRate);
        }

        isFiring = false;
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
    private IEnumerator reloadFX()
    {
        reloadSFX.Play();
        
        //Enable reload timer sprites
        reloadTimer.GetComponent<SpriteRenderer>().enabled = true;
        reloadTimer.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        reloadTimer.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        reloadTimer.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        
        //Reload animation
        reloadTimer.GetComponent<Animator>().SetTrigger("reload");
        yield return new WaitForSeconds(reloadDuration);
        
        //Disable reload timer sprites
        reloadTimer.GetComponent<SpriteRenderer>().enabled = false;
        reloadTimer.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
        reloadTimer.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = false;
        reloadTimer.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().enabled = false;
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
        for (int i = 0; i < childComponents.Length; i++)
        {
            if (childComponents[i].gameObject.name == "muzzleFlash")
            {
                muzzleFlash = childComponents[i].gameObject;
            }
            else if (childComponents[i].gameObject.name == "bulletTrail")
            {
                muzzleFlash = childComponents[i].gameObject;
            }
        }
        if (GameObject.FindGameObjectWithTag("reloadTimer"))
        {
            reloadTimer = GameObject.FindGameObjectWithTag("reloadTimer");
        }
    }
}
