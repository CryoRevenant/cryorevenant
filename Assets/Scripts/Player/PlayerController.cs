using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput controls;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform camOffset;
    [SerializeField] private Vector2 offset;
    private float movement;
    private bool isGrounded;
    private bool canJump;

    void Awake()
    {
        isGrounded = false;
        canJump = false;
        controls = gameObject.GetComponent<PlayerInput>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if(xAxis > 0)
        {
            camOffset.position = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, camOffset.position.z);
            sprite.flipX = false;
        }
        if (xAxis < 0)
        {
            camOffset.position = new Vector3(transform.position.x - offset.x, transform.position.y - offset.y, camOffset.position.z);
            sprite.flipX = true;
        }

        //Debug.Log(xAxis);
        movement = xAxis;

        if (controls.currentActionMap.FindAction("Jump").triggered && isGrounded)
        {
            canJump = true;
        }

        float distance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1), -transform.up, distance);

        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - 1), -transform.up * 1f, Color.red, 1);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movement * Time.deltaTime * speed, rb.velocity.y);
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.deltaTime);
            canJump = false;
        }
    }
}
