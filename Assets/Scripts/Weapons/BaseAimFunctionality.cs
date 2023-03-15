using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class BaseAimFunctionality : MonoBehaviour
{
    public GameObject weapon;
    public Vector2 weaponAimDirection;
    private Vector3[] playerPositions = new Vector3[700];
    public float weaponRotationAngle = 0f;
    public GameObject weaponSlotOne;
    public GameObject weaponSlotTwo;
    public Vector2 targetWorldPosition;
    private bool isAimCalcDurTSActive = false;
    private void Awake()
    {
        Definitions();
    }
    private void Update()
    {
        if(gameObject.CompareTag("Player"))
        {
            CalculateWeaponRotationAngleNTargetPosition();
        }
    }
    private void FixedUpdate()
    {
        SetWeaponRotation();
    }
    private void SetWeaponRotation()
    {
        CalculateWeaponRotationAngleNTargetPosition();
        if(gameObject.CompareTag("Player") || gameObject.name == "SecurityGuard")
        {
            if (weaponRotationAngle >= -90f && weaponRotationAngle <= 90f)
            {
                weaponSlotOne.transform.localRotation = Quaternion.Euler(0f, 0f, weaponRotationAngle);
                if (weapon.transform.parent.gameObject != weaponSlotOne)
                {
                    weapon.transform.SetParent(weaponSlotOne.transform, false);
                    if(gameObject.CompareTag("Player"))
                    {
                        GetComponent<BasePlayerMovement>().SpriteManager(0);
                    }
                }
                if (weaponRotationAngle <= 90f && weaponRotationAngle >= 0f)
                {
                    weapon.GetComponent<SpriteRenderer>().sortingOrder = 0;
                }
                else if (weaponRotationAngle < 0f && weaponRotationAngle >= -90f)
                {
                    weapon.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
            }
            else if (weaponRotationAngle > 90f || weaponRotationAngle < -90f)
            {
                weaponSlotTwo.transform.localRotation = Quaternion.Euler(180f, 0f, -weaponRotationAngle);
                if (weapon.transform.parent.gameObject != weaponSlotTwo)
                {
                    weapon.transform.SetParent(weaponSlotTwo.transform, false);
                    if (gameObject.CompareTag("Player"))
                    {
                        GetComponent<BasePlayerMovement>().SpriteManager(1);
                    }
                }
                if (weaponRotationAngle > 90f && weaponRotationAngle <= 180f)
                {
                    weapon.GetComponent<SpriteRenderer>().sortingOrder = 0;
                }
                else if (weaponRotationAngle >= -180f && weaponRotationAngle < -90f)
                {
                    weapon.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
            }
        }
        else if(gameObject.name == "BasicTurret" || gameObject.name == "InvisibleSniperTurret")
        {
            if(weaponRotationAngle >= -180f && weaponRotationAngle <= 0f)
            {
                weaponSlotOne.transform.localRotation = Quaternion.Euler(0f, 0f, weaponRotationAngle);
                weapon.GetComponent<SpriteRenderer>().sortingOrder = 2;
                if (weapon.transform.parent.gameObject != weaponSlotOne)
                {
                    weapon.transform.SetParent(weaponSlotOne.transform, false);
                }
            }
            else
            {
                weaponSlotTwo.transform.localRotation = Quaternion.Euler(180f, 0f, -weaponRotationAngle);
                weapon.GetComponent<SpriteRenderer>().sortingOrder = 0;
                if (weapon.transform.parent.gameObject != weaponSlotTwo)
                {
                    weapon.transform.SetParent(weaponSlotTwo.transform, false);
                }
            }
        }
    }
    private void CalculateWeaponRotationAngleNTargetPosition()
    {
        if(gameObject.CompareTag("Player"))
        {
            targetWorldPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            weaponAimDirection = (targetWorldPosition - (Vector2)transform.position).normalized;
            weaponRotationAngle = Mathf.Atan2(weaponAimDirection.y, weaponAimDirection.x) * Mathf.Rad2Deg;
            GetComponent<BasePlayerMovement>().weaponAngle = weaponRotationAngle;
            if(!GetComponent<BasePlayerMovement>().isDashing || GetComponent<BasePlayerMovement>().isDashing 
                && GetComponent<BasePlayerMovement>().dashDuration <= 0f)
            {
                GetComponent<BasePlayerMovement>().CalculateAimDirection();
            }
        }
        else if(gameObject.CompareTag("Mob"))
        {
            if(!GameObject.FindGameObjectWithTag("Player").GetComponent<TimeControl>().isTimeSlowed)
            {
                targetWorldPosition = (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position;
                weaponAimDirection = (targetWorldPosition - (Vector2)transform.position).normalized;
                weaponRotationAngle = Mathf.Atan2(weaponAimDirection.y, weaponAimDirection.x) * Mathf.Rad2Deg;
                GetComponentInChildren<BaseWeaponFunctionalityEnemy>().currentWeaponRotation = weaponRotationAngle;
            }
            else
            {
                if(!isAimCalcDurTSActive)
                {
                    StartCoroutine(AimCalculationDuringTimeSlowdown());
                }
            }
        }
    }
    private IEnumerator AimCalculationDuringTimeSlowdown()
    {
        isAimCalcDurTSActive = true;
        int counter = 0;
        int index = 0;
        for(int i = 0; i < playerPositions.Length; i++)
        {
            counter++;
            playerPositions[i] = GameObject.FindGameObjectWithTag("Player").transform.position;
            if(counter == 5)
            {
                counter = 0;
                targetWorldPosition = playerPositions[index];
                weaponAimDirection = (targetWorldPosition - (Vector2)transform.position).normalized;
                weaponRotationAngle = Mathf.Atan2(weaponAimDirection.y, weaponAimDirection.x) * Mathf.Rad2Deg;
                GetComponentInChildren<BaseWeaponFunctionalityEnemy>().currentWeaponRotation = weaponRotationAngle;
                index++;
            }
            yield return new WaitForSeconds(0.01f);
        }
        isAimCalcDurTSActive = false;
    }
    private void Definitions()
    {
        weaponSlotOne = transform.GetChild(0).gameObject;
        weaponSlotTwo = transform.GetChild(1).gameObject;
        if (GetComponentInChildren<BaseWeaponFunctionalityPlayer>() != null || GetComponentInChildren<BaseWeaponFunctionalityEnemy>() != null)
        {
            if (GetComponentInChildren<BaseWeaponFunctionalityPlayer>() != null)
            {
                weapon = GetComponentInChildren<BaseWeaponFunctionalityPlayer>().gameObject;
            }
            else
            {
                weapon = GetComponentInChildren<BaseWeaponFunctionalityEnemy>().gameObject;
            }
        }
    }
   
}
