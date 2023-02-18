using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseAimFunctionality : MonoBehaviour
{
    private Vector2 mouseWorldPosition;
    private Vector2 weaponAimDirection;
    public float weaponRotationAngle = 0f;
    private GameObject weaponRotationOrigin;
    private Animator anim;
    private void Awake()
    {
        weaponRotationOrigin = GameObject.FindGameObjectWithTag("WeaponRotationOrigin");
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        GetMouseWorldPosition();
        CalculateWeaponRotationAngle();
    }
    private void FixedUpdate()
    {
    }
    private void LateUpdate()
    {
        SetWeaponRotation();
    }
    private void SetWeaponRotation()
    {
        weaponRotationOrigin.transform.localRotation = Quaternion.Euler(0f, 0f, weaponRotationAngle);

        if (weaponRotationAngle > 90f && weaponRotationAngle < 180f)
        {
            anim.SetTrigger("rotateXPositive");
        }
        else if (weaponRotationAngle > -180f && weaponRotationAngle < -90f)
        {
            anim.SetTrigger("rotateXPositive");
        }
        else
        {
            anim.SetTrigger("rotateXNegative");
        }

    }
    private void GetMouseWorldPosition()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    }
    private void CalculateWeaponRotationAngle()
    {
        weaponAimDirection = (mouseWorldPosition - (Vector2)transform.position).normalized;
        weaponRotationAngle = Mathf.Atan2(weaponAimDirection.y, weaponAimDirection.x) * Mathf.Rad2Deg;
    }
}
