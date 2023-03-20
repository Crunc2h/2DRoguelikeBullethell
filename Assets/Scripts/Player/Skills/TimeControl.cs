using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    //Audio sources
    [SerializeField] private AudioSource timeSlowDownSFX;
    [SerializeField] private AudioSource timeSpeedUpSFX;

    //Variables regarding after images
    [SerializeField] private GameObject playerAfterImage;
    private GameObject AImageInstance; 

    //Variables regarding time slowdown
    private GameObject[] gObjectsInScene;
    private Transform[] childObjects;
    private float timeSlowdownDuration = 8f;
    private float timeSlowdownCd = 8f;
    public float timeSlowDownFactor = 0.1f;
    private bool timeSlowdownCdActive = false;
    public bool isTimeSlowed = false;

    //Variables regarding time travel
    private GameObject weaponSlotOne;
    private GameObject weaponSlotTwo;
    private float timeTravelCooldown = 10f;
    private bool timeTravelCdActive = true;
    public bool isTimeTraveling = false;
    //Arrays for time travel
    private Vector3[] positionsTTravel = new Vector3[700];
    private Sprite[] spritesTTravel = new Sprite[700];
    private bool[] spriteFlipXTTravel = new bool[700];

    private void Awake()
    {
        Definitions();
        StartCoroutine(RecordPositionsAndSpritesForTimeTravel());
    }
    private void Update()
    {
        AfterImageManager();
        ListenForPlayerInput();
        //Time slowdown duration
        if (isTimeSlowed && !isTimeTraveling)
        {
            timeSlowdownDuration -= Time.deltaTime;
            if(timeSlowdownDuration <= 0f)
            {
                TimeSlowDown(timeSlowDownFactor, 1f);
                timeSlowdownDuration = 7f;
                //Activate time slow down cooldown
                timeSlowdownCdActive = true;
            }
        }

        //Time slowdown cooldown
        if(timeSlowdownCdActive)
        {
            timeSlowdownCd -= Time.deltaTime;
            if(timeSlowdownCd <= 0f)
            {
                timeSlowdownCdActive = false;
                timeSlowdownCd = 8f;
            }
        }
        
        //Time travel cooldown
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

    private void ListenForPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Slow time down if time isn't already slowed down and player isn't time travelling and the cooldown is inactive
            if (!isTimeSlowed && !isTimeTraveling && !timeSlowdownCdActive)
            {
                TimeSlowDown(timeSlowDownFactor, 0.2f);
            }
        }

        //Begin time travel if the player is not already time travelling
        if (Input.GetKeyDown(KeyCode.F) && !timeTravelCdActive && !isTimeTraveling)
        {
            StartCoroutine(TimeTravel());
        }
    }
    
    //Continiously records certain parameters within the last 7(array.length / 100) seconds while time travel is inactive
    private IEnumerator RecordPositionsAndSpritesForTimeTravel()
    {
        //Iterate through the arrays
        for (int i = 0; i < positionsTTravel.Length; i++)
        {
            //Record given parameters - Position, current sprite and its flipX value
            positionsTTravel[i] = transform.position;
            spritesTTravel[i] = GetComponent<SpriteRenderer>().sprite;
            spriteFlipXTTravel[i] = GetComponent<SpriteRenderer>().flipX;

            //If the index reached the end of the array, go through the array and reduce the index of everything by 1 and then reduce the current index by 1 so
            //the previous for loop continues firing indefinetly
            if (i == positionsTTravel.Length - 1)
            {
                for (int e = 1; e < positionsTTravel.Length; e++)
                {
                    positionsTTravel[e - 1] = positionsTTravel[e];
                    spritesTTravel[e - 1] = spritesTTravel[e];
                    spriteFlipXTTravel[e - 1] = spriteFlipXTTravel[e];
                }
                i--;
            }
            yield return new WaitForSeconds(0.01f);         
            
            //Stop recording parameters if time travel is active
            if (isTimeTraveling)
            {
                break;
            }
        }
    }
    private IEnumerator TimeTravel()
    {
        isTimeTraveling = true;
        
        //Slow the time down if it is not already slowed and wait for a second
        if (!isTimeSlowed)
        {
            TimeSlowDown(timeSlowDownFactor, 0.2f);
            //Commence time travel when time is not slowed - Deactivates basic funcitons for the player
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<BasePlayerMovement>().enabled = false;
            GetComponent<BaseAimFunctionality>().enabled = false;
            GetComponent<Animator>().enabled = false;
            GetComponentInChildren<BaseWeaponFunctionalityPlayer>().isFiring = false;
            GetComponentInChildren<BaseWeaponFunctionalityPlayer>().enabled = false;
            yield return new WaitForSeconds(1);
            weaponSlotOne.GetComponent<SpriteRenderer>().enabled = false;
            weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<BaseWeaponFunctionalityPlayer>().gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            //Commence time travel when time slowed - Deactivates basic funcitons for the player
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<BasePlayerMovement>().enabled = false;
            GetComponent<BaseAimFunctionality>().enabled = false;
            GetComponent<Animator>().enabled = false;
            GetComponentInChildren<BaseWeaponFunctionalityPlayer>().isFiring = false;
            GetComponentInChildren<BaseWeaponFunctionalityPlayer>().enabled = false;
            yield return new WaitForSeconds(1);
            weaponSlotOne.GetComponent<SpriteRenderer>().enabled = false;
            weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<BaseWeaponFunctionalityPlayer>().gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
             
        //Activates after images
        AfterImagesWhenTSlowerOrTTraveling();

        //Goes through the arrays that recorded player parameters for the last 7 seconds 
        for (int i = positionsTTravel.Length - 1; i > 0; i--)
        {
            transform.position = positionsTTravel[i];
            GetComponent<SpriteRenderer>().sprite = spritesTTravel[i];
            GetComponent<SpriteRenderer>().flipX = spriteFlipXTTravel[i];
            yield return new WaitForSeconds(0.003f);
        }

        //Reactivate basic player functions
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Animator>().enabled = true;
        GetComponent<BaseAimFunctionality>().enabled = true;
        GetComponent<BasePlayerMovement>().enabled = true;
        GetComponentInChildren<BaseWeaponFunctionalityPlayer>().enabled = true;
        weaponSlotOne.GetComponent<SpriteRenderer>().enabled = true;
        weaponSlotTwo.GetComponent<SpriteRenderer>().enabled = true;
        GetComponentInChildren<BaseWeaponFunctionalityPlayer>().gameObject.GetComponent<SpriteRenderer>().enabled = true;

        //Restart recording player parameters
        StartCoroutine(RecordPositionsAndSpritesForTimeTravel());
        
        //Speed up time regardless of whether it was slowed down or not when time travel began
        if(isTimeSlowed)
        {
            TimeSlowDown(timeSlowDownFactor, 1);
            if(timeSlowdownDuration != 7f)
            {
                timeSlowdownDuration = 7f;
            }
        }
        
        //End time travel and activate cooldown
        isTimeTraveling = false;
        timeTravelCdActive = true;
    }
    
    //Get the current sprite and its flipX value then halve its alpha value - Sets the current after image instance to be used
    private void AfterImageManager()
    {
        playerAfterImage.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        playerAfterImage.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
        Color AImageColor = playerAfterImage.GetComponent<SpriteRenderer>().color;
        AImageColor.a = 0.5f;
        playerAfterImage.GetComponent<SpriteRenderer>().color = AImageColor;
    }
    
    //Instantiates a clone of the current after image instance, deparents it from the player, sets its world position and passes it on to SustainAndDestroy enumerator
    private void CreateAfterImage()
    {
        AImageInstance = Instantiate(playerAfterImage, transform);
        AImageInstance.transform.SetParent(null, true);
        AImageInstance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        StartCoroutine(SustainAndDestroyAfterImage(AImageInstance));
    }
    
    //Takes the after image instance that has the alpha value of 0.5, reduces its alpha value to 0 within 0.25 seconds and then destroys it
    private IEnumerator SustainAndDestroyAfterImage(GameObject playerAfterImageInstance)
    {
        for(int i = 0; i < 25; i++)
        {
            Color AImageColor = playerAfterImageInstance.GetComponent<SpriteRenderer>().color;
            AImageColor.a -= 0.02f;
            playerAfterImageInstance.GetComponent<SpriteRenderer>().color = AImageColor;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(playerAfterImageInstance);
    }
    
    //Continuously creates after images once every 0.01 seconds while time is slowed or the player is time traveling
    private IEnumerator AfterImagesWhenTSlowerOrTTraveling()
    {
        while(isTimeSlowed && !isTimeTraveling)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(0.01f);
        }
        while(isTimeTraveling)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    //Goes through every object in the scene and makes adjustments based on whether time is slowed down or not
    private void TimeSlowDown(float timeSlowFactor, float pitchValue)
    {
        //If time isn't slowed down when it is called, it slows it down or vice versa
        if(!isTimeSlowed)
        {
            isTimeSlowed = true;
            timeSlowDownSFX.Play();
            
            //Activate after images
            StartCoroutine(AfterImagesWhenTSlowerOrTTraveling());
        }
        else
        {
            isTimeSlowed = false;
            timeSpeedUpSFX.Play();
        }
        
        //Returns all game objects within the scene
        gObjectsInScene = FindObjectsOfType<GameObject>();      
        
        //Iterate through all game objects within the array
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
                    //Adjust animation speed for mobs
                    if (gObjectsInScene[i].gameObject.GetComponent<Animator>() != null)
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Animator>().speed *= timeSlowFactor;
                    }
                }
                else
                {
                    //Adjust animation speed for mobs
                    if (gObjectsInScene[i].gameObject.GetComponent<Animator>() != null)
                    {
                        gObjectsInScene[i].gameObject.GetComponent<Animator>().speed *= (1 / timeSlowFactor);
                    }
                }

            }
            
            //Time adjustments on projectiles          
            if (gObjectsInScene[i].gameObject.CompareTag("enemyProjectile") || gObjectsInScene[i].gameObject.CompareTag("playerProjectile"))
            {
                if(!isTimeSlowed)
                {
                    //Speed up all projectiles
                    //Check whether the current projectile force is equal to original - Determines if a projectile was fired when time was slowed or normal
                    if(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentProjectileForce == gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().originalWeaponProjectileForce)
                    {
                        //Fires if the projectile was instantiated while time was at normal speed
                        gObjectsInScene[i].gameObject.GetComponent<Rigidbody2D>().AddForce(gObjectsInScene[i].gameObject.GetComponent<ProjectileCollisionTrigger>().currentForceAndDirectionOnProjectile * 0.9f);
                    }
                    else
                    {
                        //Projectile will need 10 times more force if its projectile force was reduced to 1/10 - fires if the projectile was instantiated while time was slowed
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
            //Returns all of the transforms of the child objects of the current game object in the loop
            childObjects = gObjectsInScene[i].GetComponentsInChildren<Transform>();          
            if(childObjects.Length > 0)
            {
                //Iterate through all of the child objects
                for (int e = 0; e < childObjects.Length; e++)
                {        
                    if (childObjects[e].gameObject.GetComponent<AudioSource>() != null)
                    {
                        //Equalize all pitches to the given pitch value for child objects
                        childObjects[e].gameObject.GetComponent<AudioSource>().pitch = pitchValue;
                    }
                    
                    //Adjustments on weapons belonging to mobs
                    if (gObjectsInScene[i].gameObject.CompareTag("Mob") && childObjects[e].gameObject.CompareTag("weapon"))
                    {
                        if (isTimeSlowed)
                        {
                            //Slowdown all enemy weapon functionalities
                            if(childObjects[e].gameObject.GetComponent<Animator>() != null)
                            {
                                childObjects[e].gameObject.GetComponent<Animator>().speed *= timeSlowFactor;
                            }
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileForce *= timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().fireRate *= timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().randomizedShootingRateRange *= timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileLifetime *= (1 / timeSlowFactor);
                        }
                        else
                        {
                            //Speed up all enemy weapon functionalities
                            if(childObjects[e].gameObject.GetComponent<Animator>() != null)
                            {
                                childObjects[e].gameObject.GetComponent<Animator>().speed = childObjects[e].gameObject.GetComponent<Animator>().speed * (1 / timeSlowFactor);
                            }
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileForce *= (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().fireRate *= (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().randomizedShootingRateRange *= (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityEnemy>().projectileLifetime *= timeSlowFactor;
                        }                     
                    }

                    //Adjustments on weapons belonging to player
                    if (gObjectsInScene[i].gameObject.CompareTag("Player") && childObjects[e].gameObject.CompareTag("weapon"))
                    {
                        if (isTimeSlowed)
                        {
                            //Slowdown all player weapon functionalities
                            childObjects[e].gameObject.GetComponent<Animator>().speed *= timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileForce *= timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().fireRate *= timeSlowFactor;
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileLifetime *= (1 / timeSlowFactor);
                        }
                        else
                        {
                            //Speed up all player weapon functionalities
                            childObjects[e].gameObject.GetComponent<Animator>().speed *= (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileForce *= (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().fireRate *= (1 / timeSlowFactor);
                            childObjects[e].gameObject.GetComponent<BaseWeaponFunctionalityPlayer>().projectileLifetime *= timeSlowFactor;
                        }
                    }                  
                }
            }
        }
    }
    private void Definitions()
    {
        weaponSlotOne = transform.GetChild(0).gameObject;
        weaponSlotTwo = transform.GetChild(1).gameObject;
    }

}
