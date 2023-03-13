using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    [SerializeField] private AudioSource timeSlowDownSFX;
    [SerializeField] private AudioSource timeSpeedUpSFX;
    [SerializeField] private GameObject playerAfterImage;
    private GameObject AImageInstance;
    private GameObject[] gObjectsInScene;
    private Vector3[] positionsTTravel = new Vector3[700];
    private Transform[] childObjects;
    public float timeSlowDownFactor = 0.1f;
    public bool isTimeSlowed = false;
    private float timeSlowdownCd = 8f;
    
    private float timeTravelCooldown = 10f;
    private bool timeTravelCdActive = true;
    public bool isTimeTraveling = false;

    private void Awake()
    {
        StartCoroutine(RecordPositionsForTimeTravel());
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!isTimeSlowed && !isTimeTraveling)
            {
                TimeSlowDown(timeSlowDownFactor, 0.2f);
            }
            else
            {
                TimeSlowDown(timeSlowDownFactor, 1f);
            }
        }
        
        if(isTimeSlowed)
        {
            timeSlowdownCd -= Time.deltaTime;
            if(timeSlowdownCd <= 0f)
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
                timeTravelCooldown = 10f;
            }
        }
    }
    private IEnumerator RecordPositionsForTimeTravel()
    {
        for (int i = 0; i < positionsTTravel.Length; i++)
        {
            positionsTTravel[i] = transform.position;
            if (i == positionsTTravel.Length - 1)
            {
                for (int e = 1; e < positionsTTravel.Length; e++)
                {
                    positionsTTravel[e - 1] = positionsTTravel[e];
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
        if(!isTimeSlowed)
        {
            TimeSlowDown(timeSlowDownFactor, 0.2f);
            yield return new WaitForSeconds(1);
        }
        isTimeTraveling = true;
        GetComponent<BasePlayerMovement>().enabled = false;
        GetComponent<BaseAimFunctionality>().enabled = false;
        GetComponent<Animator>().enabled = false;
        GetComponentInChildren<BaseWeaponFunctionalityPlayer>().enabled = false;
        for (int i = positionsTTravel.Length - 1; i > 0; i--)
        {
            transform.position = positionsTTravel[i];           
            
            AImageInstance = Instantiate(playerAfterImage, transform);
            AImageInstance.transform.SetParent(null, true);
            AImageInstance.transform.position = new Vector3(positionsTTravel[i].x, positionsTTravel[i].y, positionsTTravel[i].z);
            StartCoroutine(AfterImage(AImageInstance));
            
            yield return new WaitForSeconds(0.001f);
        }
        GetComponentInChildren<BaseWeaponFunctionalityPlayer>().enabled = true;
        GetComponent<Animator>().enabled = true;
        GetComponent<BaseAimFunctionality>().enabled = true;
        GetComponent<BasePlayerMovement>().enabled = true;
        StartCoroutine(RecordPositionsForTimeTravel());
        if (isTimeSlowed)
        {
            TimeSlowDown(timeSlowDownFactor, 1f);
        }
        isTimeTraveling = false;
        timeTravelCdActive = true;
    }
    private IEnumerator AfterImage(GameObject playerAfterImageInstance)
    {
        int counter = 50;
        while(counter > 0)
        {
            counter--;
            Color AImageColor = playerAfterImageInstance.GetComponent<SpriteRenderer>().color;
            AImageColor.a -= 0.02f;
            playerAfterImageInstance.GetComponent<SpriteRenderer>().color = AImageColor;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(playerAfterImageInstance);
    }
    private IEnumerator AfterImageDuringTimeSlowed()
    {
        while(isTimeSlowed)
        {
            AImageInstance = Instantiate(playerAfterImage, transform);
            StartCoroutine(AfterImage(AImageInstance));
            AImageInstance.transform.SetParent(null, true);
            AImageInstance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
    }
    private void TimeSlowDown(float timeSlowFactor, float pitchValue)
    {
        if(!isTimeSlowed)
        {
            isTimeSlowed = true;
            timeSlowDownSFX.Play();
            StartCoroutine(AfterImage(AImageInstance));
        }
        else
        {
            isTimeSlowed = false;
            timeSlowdownCd = 7f;
            timeSpeedUpSFX.Play();
        }
        gObjectsInScene = FindObjectsOfType<GameObject>();      
        for (int i = 0; i < gObjectsInScene.Length; i++)
        {
            if (gObjectsInScene[i].GetComponent<AudioSource>() != null)
            {
                //Equalize all pitches to the given pitch value for game objects
                gObjectsInScene[i].GetComponent<AudioSource>().pitch = pitchValue;
            }
            
            //Time adjustments on mobs
            if(gObjectsInScene[i].gameObject.CompareTag("Mob"))
            {
                if (isTimeSlowed)
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
            
            //Time adjustments on projectiles          
            if (gObjectsInScene[i].gameObject.CompareTag("enemyProjectile") || gObjectsInScene[i].gameObject.CompareTag("playerProjectile"))
            {
                if(!isTimeSlowed)
                {
                    //Speed up all projectiles
                    if(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentProjectileForce == gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().originalWeaponProjectileForce)
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Rigidbody2D>().AddForce(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceAndDirectionOnProjectile * 0.9f);
                    }
                    else
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Rigidbody2D>().AddForce(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceAndDirectionOnProjectile * 9f);
                    }
                }
                else
                {
                    //Slow down all projectiles
                    if (gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentProjectileForce == gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().originalWeaponProjectileForce)
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Rigidbody2D>().AddForce(-(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceAndDirectionOnProjectile * 0.9f));
                    }
                    else
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Rigidbody2D>().AddForce(-gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceAndDirectionOnProjectile * 9f);
                    }
                        
                }
            }
            
            //Time adjustments on child objects if there are any
            
            childObjects = gObjectsInScene[i].GetComponentsInChildren<Transform>();          
            
            if(childObjects.Length > 0)
            {
                for (int e = 0; e < childObjects.Length; e++)
                {        
                    if (childObjects[e].gameObject.GetComponent<AudioSource>() != null)
                    {
                        //Equalize all pitches to the given pitch value for child objects
                        childObjects[e].gameObject.GetComponent<AudioSource>().pitch = pitchValue;
                    }
                    
                    if (gObjectsInScene[i].gameObject.CompareTag("Mob") && childObjects[e].gameObject.CompareTag("weapon"))
                    {
                        if (isTimeSlowed)
                        {
                            //Slowdown all enemy weapon functionalities
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
                            //Speed up all enemy weapon functionalities
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
                        if (isTimeSlowed)
                        {
                            //Slowdown all player weapon functionalities
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
                            //Speed up all player weapon functionalities
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
    }

}
