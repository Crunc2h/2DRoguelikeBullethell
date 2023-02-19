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
    [SerializeField] private float maxAmmo = 20f;
    [SerializeField] private float currentAmmo = 20f;
    [SerializeField] private float reloadDuration = 2f;
    [Header("SFX")]
    [SerializeField] private AudioSource pistolFireSFX;
    private GameObject projectileClone;
    private Quaternion initialProjectileRotation;
    private Vector3 projectileSpawnPos;
    public bool fireCommand = false;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(automaticFiring());
        }
    }
    private IEnumerator automaticFiring()
    {
        while (fireCommand)
        {
            fireProjectileLogic();
            yield return new WaitForSeconds(1 / fireRate);
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
        if (gameObject.transform.GetChild(1).CompareTag("prjectileSpawnPoint"))
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
        StartCoroutine(muzzleFlash());
        StartCoroutine(bulletTrail());
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
