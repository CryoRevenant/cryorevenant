using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerControllerV2 : MonoBehaviour
{
    private PlayerInput controls;
    private Rigidbody2D rb;
    [Header("Player")]
    [SerializeField] private float yRaycastGrounded;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private SpriteRenderer leftDustSprite;
    [SerializeField] private SpriteRenderer rightDustSprite;
    [Header("MainCamera")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float vcamMoveXSpeed;
    [SerializeField] private float vcamMoveYSpeed;
    [SerializeField] private float vcamMoveYawSpeed;
    [SerializeField] private float camCenterTimer;
    [Header("Velocity")]
    [SerializeField] private float lowJumpForce;
    [SerializeField] private float heighJumpForce;
    [SerializeField] private float jumpDistance;
    // la velocitySpeed est la vitesse à laquelle le personnage atteint sa vitesse max
    [SerializeField] private float moveSpeed;
    [SerializeField] private AnimationCurve accelSpeed;
    [SerializeField] private float inertia;
    private float currJumpForce;
    private float maxCamCenterTimer;
    private float movement;
    private float timer = 0;
    private float curVelocitySpeed;
    private bool isGrounded;
    private bool canJump;

    void Awake()
    {
        isGrounded = false;
        canJump = false;
        controls = gameObject.GetComponent<PlayerInput>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        maxCamCenterTimer = camCenterTimer;
        curVelocitySpeed = 0;

        if (!playerSprite.flipX)
        {
            vcam.transform.position = new Vector3(transform.position.x + offset.x, offset.y, offset.z);
            leftDustSprite.enabled = true;
            rightDustSprite.enabled = false;
        }

        if (playerSprite.flipX)
        {
            vcam.transform.position = new Vector3(transform.position.x - offset.x, offset.y, offset.z);
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = true;
        }
    }

    private void Update()
    {
        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if (xAxis > 0)
        {
            vcam.transform.position = new Vector3(transform.position.x + offset.x, offset.y, offset.z);
            playerSprite.flipX = false;
            leftDustSprite.enabled = true;
            rightDustSprite.enabled = false;
            curVelocitySpeed += accelSpeed.Evaluate(Time.deltaTime);
            if (curVelocitySpeed >= moveSpeed)
            {
                curVelocitySpeed = moveSpeed;
            }
        }
        if (xAxis < 0)
        {
            vcam.transform.position = new Vector3(transform.position.x - offset.x, offset.y, offset.z);
            playerSprite.flipX = true;
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = true;
            curVelocitySpeed += accelSpeed.Evaluate(Time.deltaTime);
            if (curVelocitySpeed >= moveSpeed)
            {
                curVelocitySpeed = moveSpeed;
            }
        }

        //Debug.Log(movement);

        movement = xAxis;

        controls.currentActionMap.FindAction("HeighJump").performed += _ => PerformHighJump();

        controls.currentActionMap.FindAction("Jump").started += _ => PerformJump();

        if (!canJump)
        {
            timer = 0;
        }

        //Debug.Log(jumpForce);

        float distance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - yRaycastGrounded), -transform.up, distance, 1 << 6);

        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - yRaycastGrounded), -transform.up * 1f, Color.red, 1);
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
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = false;
        }

        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = vcamMoveYSpeed;
        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping = vcamMoveYawSpeed;

        if (xAxis == 0)
        {
            curVelocitySpeed -= accelSpeed.Evaluate(Time.deltaTime/2);
            if(curVelocitySpeed <= 0)
            {
                curVelocitySpeed = 0;
            }
            camCenterTimer -= Time.deltaTime;
            //Debug.Log((int)camCenterTimer);
            if (camCenterTimer <= 0)
            {
                //Debug.Log("center");
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 3;
                vcam.transform.position = new Vector3(transform.position.x, offset.y, offset.z);
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
        Vector3 nextPosition = new Vector3(transform.position.x + movement, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * curVelocitySpeed);
        Debug.Log((int)curVelocitySpeed);

        if (canJump)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f && isGrounded)
            {
                //Debug.Log(currJumpForce);
                rb.velocity = new Vector2(rb.velocity.x, currJumpForce * Time.deltaTime);
                canJump = false;
            }
        }
    }

    void PerformHighJump()
    {
        //Debug.Log("double saut");
        currJumpForce = heighJumpForce;
    }

    void PerformJump()
    {
        //Debug.Log("saut normal");
        currJumpForce = lowJumpForce;
        canJump = true;
    }
}
