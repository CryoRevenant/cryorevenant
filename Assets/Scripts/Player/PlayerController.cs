using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput controls;
    private Rigidbody2D rb;
    [Header("Player")]
    [SerializeField] private float lowJumpForce;
    [SerializeField] private float heighJumpForce;
    // la velocitySpeed est la vitesse à laquelle le personnage atteint sa vitesse max
    [SerializeField] private float velocitySpeed;
    [SerializeField] private float velocity;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private SpriteRenderer leftDustSprite;
    [SerializeField] private SpriteRenderer rightDustSprite;
    [Header("MainCamera")]
    [SerializeField] private Transform camOffset;
    [SerializeField] private Vector2 offset;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float vcamMoveXSpeed;
    [SerializeField] private float vcamMoveYSpeed;
    [SerializeField] private float vcamMoveYawSpeed;
    [SerializeField] private float camCenterTimer;
    private float maxJumpForce;
    private float maxheighJumpForce;
    private float maxCamCenterTimer;
    private float movement;
    private float timer = 0;
    private bool isGrounded;
    private bool canJump;

    void Awake()
    {
        isGrounded = false;
        canJump = false;
        controls = gameObject.GetComponent<PlayerInput>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        maxCamCenterTimer = camCenterTimer;
        maxJumpForce = lowJumpForce;
        maxheighJumpForce = heighJumpForce;

        if (!playerSprite.flipX)
        {
            camOffset.position = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, camOffset.position.z);
            leftDustSprite.enabled = true;
            rightDustSprite.enabled = false;
        }

        if (playerSprite.flipX)
        {
            camOffset.position = new Vector3(transform.position.x - offset.x, transform.position.y + offset.y, camOffset.position.z);
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = true;
        }
    }

    private void Update()
    {
        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if (xAxis > 0)
        {
            camOffset.position = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, camOffset.position.z);
            playerSprite.flipX = false;
            leftDustSprite.enabled = true;
            rightDustSprite.enabled = false;
        }
        if (xAxis < 0)
        {
            camOffset.position = new Vector3(transform.position.x - offset.x, transform.position.y + offset.y, camOffset.position.z);
            playerSprite.flipX = true;
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = true;
        }

        //Debug.Log(movement);

        movement = xAxis;

        if (controls.currentActionMap.FindAction("HighJump").triggered)
        {
            Debug.Log("double saut");
            lowJumpForce = maxheighJumpForce;
        }

        if (controls.currentActionMap.FindAction("Jump").triggered && isGrounded)
        {
            Debug.Log("saut normal");
            lowJumpForce = maxJumpForce;
            canJump = true;
        }

        if (!canJump)
        {
            timer = 0;
        }

        //Debug.Log(jumpForce);

        float distance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1), -transform.up, distance, 1 << 6);

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

        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = vcamMoveYSpeed;
        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping = vcamMoveYawSpeed;

        if (xAxis == 0)
        {
            camCenterTimer -= Time.deltaTime;
            //Debug.Log((int)camCenterTimer);
            if (camCenterTimer <= 0)
            {
                //Debug.Log("center");
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 3;
                camOffset.position = new Vector3(transform.position.x, transform.position.y + offset.y, camOffset.position.z);
                camCenterTimer = 0;
            }
        }
        else
        {
            vcam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = vcamMoveXSpeed;
            camCenterTimer = maxCamCenterTimer;
        }
    }

    void FixedUpdate()
    {
        velocity = rb.velocity.magnitude;

        if (movement == 1)
        {
            rb.velocity = new Vector2(velocitySpeed * Time.deltaTime, rb.velocity.y);
        }
        if (movement == -1)
        {
            rb.velocity = new Vector2(-velocitySpeed * Time.deltaTime, rb.velocity.y);
        }

        if(isGrounded)
        {
            rb.AddForceAtPosition(new Vector2(movement, rb.velocity.y), transform.position);
        }
        else if (!isGrounded)
        {
            rb.AddForceAtPosition(new Vector2(movement, rb.velocity.y), transform.position);
        }

        if (canJump)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x, lowJumpForce * Time.deltaTime);
                canJump = false;
            }
        }
    }
}
