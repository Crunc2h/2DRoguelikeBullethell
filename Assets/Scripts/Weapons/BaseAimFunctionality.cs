using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseAimFunctionality : MonoBehaviour
{
    private Vector2 targetWorldPosition;
    private Vector2 weaponAimDirection;
    public float weaponRotationAngle = 0f;
    private GameObject weaponRotationOrigin;
    private GameObject weaponSlot;
    private void Awake()
    {
        weaponRotationOrigin = transform.GetChild(0).gameObject;
        weaponSlot = weaponRotationOrigin.transform.GetChild(0).gameObject;
    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        SetWeaponRotation();
    }
    private void SetWeaponRotation()
    {
        CalculateWeaponRotationAngleNTargetPosition();
        weaponRotationOrigin.transform.localRotation = Quaternion.Euler(0f, 0f, weaponRotationAngle);
        
        if (weaponRotationAngle > 90f && weaponRotationAngle < 180f)
        {
            weaponSlot.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
        }
        else if (weaponRotationAngle > -180f && weaponRotationAngle < -90f)
        {
            weaponSlot.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
        }
        else
        {
            weaponSlot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

    }
    private void CalculateWeaponRotationAngleNTargetPosition()
    {
        if(gameObject.CompareTag("Player"))
        {
            targetWorldPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            weaponAimDirection = (targetWorldPosition - (Vector2)transform.position).normalized;
            weaponRotationAngle = Mathf.Atan2(weaponAimDirection.y, weaponAimDirection.x) * Mathf.Rad2Deg;
        }
        else if(gameObject.CompareTag("Mob"))
        {
            targetWorldPosition = (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position;
            weaponAimDirection = (targetWorldPosition - (Vector2)transform.position).normalized;
            weaponRotationAngle = Mathf.Atan2(weaponAimDirection.y, weaponAimDirection.x) * Mathf.Rad2Deg;
        }
    }
}
