using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    private Vector2 inputScale;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        CalculateInputScale();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void CalculateInputScale()
    {
        inputScale = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private void Move()
    {
        rb.MovePosition((Vector2)transform.position + inputScale * movementSpeed * Time.fixedDeltaTime);
    }
}
