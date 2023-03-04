using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    private GameObject[] gObjectsInScene;
    private GameObject[] projectiles;
    private Transform[] childObjects;
    public float timeSlowDownFactor = 0.1f;
    public bool isTimeSlowed = false;

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
    }

    private void TimeSlowDown(float timeSlowFactor, float pitchValue)
    {
        if(isTimeSlowed)
        {
            //Projectile speed up after time scale growth
        }
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
