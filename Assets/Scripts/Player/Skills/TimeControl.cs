using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    private GameObject[] gObjectsInScene;
    private Vector3[] positions = new Vector3[700];
    private Transform[] childObjects;
    public float timeSlowDownFactor = 0.1f;
    private float timeTravelCooldown = 14f;
    private bool timeTravelCdActive = true;
    public bool isTimeSlowed = false;
    private bool isTimeTraveling = false;

    private void Awake()
    {
        StartCoroutine(RecordPositions());
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!isTimeSlowed)
            {
                TimeSlowDown(timeSlowDownFactor, 0.2f);
            }
            else
            {
                TimeSlowDown(timeSlowDownFactor, 1f);
            }
        }

        if(Input.GetKeyDown(KeyCode.F) && !timeTravelCdActive && !isTimeTraveling)
        {
            StartCoroutine(TimeTravel());
        }
        if(timeTravelCdActive)
        {
            timeTravelCooldown -= Time.unscaledDeltaTime;
            if(timeTravelCooldown <= 0f)
            {
                timeTravelCdActive = false;
                timeTravelCooldown = 14f;
            }
        }
    }
    private IEnumerator RecordPositions()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = transform.position;
            if (i == positions.Length - 1)
            {
                for (int e = 1; e < positions.Length; e++)
                {
                    positions[e - 1] = positions[e];
                }
                i--;
            }
            yield return new WaitForSeconds(0.01f);            
            if (isTimeTraveling)
            {
                break;
            }
        }
    }
    private IEnumerator TimeTravel()
    {
        isTimeTraveling = true;
        for (int i = positions.Length - 1; i > 0; i--)
        {
            transform.position = positions[i];
            yield return new WaitForSeconds(0.002f);
        }
        StartCoroutine(RecordPositions());
        isTimeTraveling = false;
        timeTravelCdActive = true;
    }
    private void TimeSlowDown(float timeSlowFactor, float pitchValue)
    {
        gObjectsInScene = FindObjectsOfType<GameObject>();
        for(int i = 0; i < gObjectsInScene.Length; i++)
        {
            if (gObjectsInScene[i].GetComponent<AudioSource>() != null)
            {
                gObjectsInScene[i].GetComponent<AudioSource>().pitch = pitchValue;
            }
            if(gObjectsInScene[i].gameObject.CompareTag("Mob"))
            {
                if(!isTimeSlowed)
                {
                    if (gObjectsInScene[i].gameObject.GetComponent<Animator>() != null)
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Animator>().speed = gObjectsInScene[i].gameObject.GetComponent<Animator>().speed * timeSlowFactor;
                    }
                }
                else
                {
                    if (gObjectsInScene[i].gameObject.GetComponent<Animator>() != null)
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Animator>().speed = gObjectsInScene[i].gameObject.GetComponent<Animator>().speed * (1 / timeSlowFactor);
                    }
                }

            }
            if (gObjectsInScene[i].gameObject.CompareTag("Player"))
            {
                //what to do within player object
            }
            if (gObjectsInScene[i].gameObject.CompareTag("enemyProjectile") || gObjectsInScene[i].gameObject.CompareTag("playerProjectile"))
            {
                if(isTimeSlowed)
                {                   
                    gObjectsInScene[i].gameObject.GetComponent<Rigidbody2D>().AddForce(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceOnProjectile * 9);
                    gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceOnProjectile = gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceOnProjectile * 10;                
                }
                else
                {
                    gObjectsInScene[i].gameObject.GetComponent<Rigidbody2D>().AddForce(-(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceOnProjectile * 0.9f));
                    gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceOnProjectile = gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceOnProjectile * (1 / 10);

                }
            }
            childObjects = gObjectsInScene[i].GetComponentsInChildren<Transform>();          
            if(childObjects.Length > 0)
            {
                for (int e = 0; e < childObjects.Length; e++)
                {        
                    if (childObjects[e].gameObject.GetComponent<AudioSource>() != null)
                    {
                        childObjects[e].gameObject.GetComponent<AudioSource>().pitch = pitchValue;
                    }
                    if (gObjectsInScene[i].gameObject.CompareTag("Mob") && childObjects[e].gameObject.CompareTag("weapon"))
                    {
                        if(!isTimeSlowed)
                        {
                            if(childObjects[e].gameObject.GetComponent<Animator>() != null)
                            {
                                childObjects[e].gameObject.GetComponent<Animator>().speed = childObjects[e].gameObject.GetComponent<Animator>().speed * timeSlowFactor;
                            }
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileForce =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileForce * timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().fireRate =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().fireRate * timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().randomizedShootingRateRange =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().randomizedShootingRateRange * timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileLifetime =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileLifetime * (1 / timeSlowFactor);
                        }
                        else
                        {
                            if(childObjects[e].gameObject.GetComponent<Animator>() != null)
                            {
                                childObjects[e].gameObject.GetComponent<Animator>().speed = childObjects[e].gameObject.GetComponent<Animator>().speed * (1 / timeSlowFactor);
                            }
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileForce =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileForce * (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().fireRate =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().fireRate * (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().randomizedShootingRateRange =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().randomizedShootingRateRange * (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileLifetime =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileLifetime * timeSlowFactor;
                        }                     
                    }
                    if (gObjectsInScene[i].gameObject.CompareTag("Player") && childObjects[e].gameObject.CompareTag("weapon"))
                    {
                        if (!isTimeSlowed)
                        {
                            childObjects[e].gameObject.GetComponent<Animator>().speed = childObjects[e].gameObject.GetComponent<Animator>().speed * timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileForce =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileForce * timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().fireRate =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().fireRate * timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileLifetime =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileLifetime * (1 / timeSlowFactor);

                        }
                        else
                        {
                            childObjects[e].gameObject.GetComponent<Animator>().speed = childObjects[e].gameObject.GetComponent<Animator>().speed * (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileForce =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileForce * (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().fireRate =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().fireRate * (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileLifetime =
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileLifetime * timeSlowFactor;
                        }
                    }                  
                }
            }
        }
        isTimeSlowed = !isTimeSlowed;
    }

}
