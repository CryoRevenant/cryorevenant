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
    [SerializeField] private Transform camOffset;
    [SerializeField] private Vector3 offset;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float vcamMoveXSpeed;
    [SerializeField] private float vcamMoveYSpeed;
    [SerializeField] private float vcamMoveYawSpeed;
    [SerializeField] private float reverseSpeed;
    [SerializeField] private float camCenterTimer;
    [Header("Velocity")]
    [SerializeField] private float lowJumpForce;
    [SerializeField] private float heighJumpForce;
    [SerializeField] private float jumpDistance;
    [SerializeField] private float jumpAnalogImpact;
    // la velocitySpeed est la vitesse à laquelle le personnage atteint sa vitesse max
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDistance;
    [SerializeField] private float accelSpeed;
    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float inertia;
    private float currJumpForce;
    private float currdashForce;
    private float maxCamCenterTimer;
    private float movement;
    private float timer = 0;
    private float curVelocitySpeed;
    private float result;
    private float yAxis;

    private bool isGrounded;
    private bool canJump;
    private bool canDash;
    private bool canResetCurMoveSpeed;
    private bool canReverse;

    void Awake()
    {
        isGrounded = false;
        canJump = false;
        canDash = false;
        canReverse = true;
        controls = gameObject.GetComponent<PlayerInput>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        maxCamCenterTimer = camCenterTimer;
        curVelocitySpeed = 0;
        currdashForce = dashForce;

        if (!playerSprite.flipX)
        {
            result = transform.position.x + offset.x;
            camOffset.position = new Vector3(transform.position.x + offset.x, offset.y, camOffset.position.z);
            leftDustSprite.enabled = true;
            rightDustSprite.enabled = false;
        }

        if (playerSprite.flipX)
        {
            canReverse = true;
            result = transform.position.x - offset.x;
            camOffset.position = new Vector3(transform.position.x + offset.x, offset.y, camOffset.position.z);
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = true;
        }

        canResetCurMoveSpeed = false;
    }

    private void Update()
    {
        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if (xAxis > 0)
        {
            canReverse = true;
            if (canResetCurMoveSpeed)
            {
                curVelocitySpeed = 0;
                canResetCurMoveSpeed = false;
            }
            result = transform.position.x + offset.x;
            playerSprite.flipX = false;
            leftDustSprite.enabled = true;
            rightDustSprite.enabled = false;
            curVelocitySpeed += accelCurve.Evaluate(Time.deltaTime * accelSpeed);
            if (curVelocitySpeed >= moveSpeed)
            {
                curVelocitySpeed = moveSpeed;
            }
        }
        if (xAxis < 0)
        {
            canReverse = true;
            if (canResetCurMoveSpeed)
            {
                curVelocitySpeed = 0;
                canResetCurMoveSpeed = false;
            }
            result = transform.position.x - offset.x;
            playerSprite.flipX = true;
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = true;
            curVelocitySpeed += accelCurve.Evaluate(Time.deltaTime * accelSpeed);
            if (curVelocitySpeed >= moveSpeed)
            {
                curVelocitySpeed = moveSpeed;
            }
        }

        if(controls.currentActionMap.FindAction("DashRight").triggered)
        {
            playerSprite.flipX = false;
            dashForce = currdashForce;
            canDash = true;
        }
        else if (controls.currentActionMap.FindAction("DashLeft").triggered)
        {
            playerSprite.flipX = true;
            dashForce = -currdashForce;
            canDash = true;
        }

        //Debug.Log(movement);

        movement = xAxis;

        if (controls.currentActionMap.FindAction("Jump").triggered)
        {
            //Debug.Log("saut normal");
            canJump = true;
        }

        currJumpForce = lowJumpForce * yAxis;
        yAxis += jumpCurve.Evaluate(controls.currentActionMap.FindAction("Jump").ReadValue<float>() * Time.deltaTime * jumpAnalogImpact);

        if (!canJump)
        {
            timer = 0;
            yAxis = 0;
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
            canResetCurMoveSpeed = true;
            curVelocitySpeed -= accelCurve.Evaluate(Time.deltaTime*accelSpeed);
            if(curVelocitySpeed <= 0)
            {
                curVelocitySpeed = 0;
            }
            camCenterTimer -= Time.deltaTime;
            //Debug.Log((int)camCenterTimer);
            if (camCenterTimer <= 0)
            {
                //Debug.Log("center");
                canReverse = false;
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 3;
                camOffset.position = new Vector3(transform.position.x, offset.y, camOffset.position.z);
                camCenterTimer = 0;
            }
        }
        else
        {
            vcam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0.5f;
            camCenterTimer = maxCamCenterTimer;
        }
    }

    void FixedUpdate()
    {
        // accel
        Vector3 nextPosition = new Vector3(transform.position.x + movement, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * curVelocitySpeed);
        //Debug.Log((int)curVelocitySpeed);

        // dash
        if (canDash)
        {
            Vector3 nextDashPos = new Vector3(transform.position.x + dashForce, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, nextDashPos, Time.deltaTime * dashDistance);
            canDash = false;
        }

        // mouvement de la caméra sur l'axe x lors que le personnage se tourne
        if (canReverse)
        {
            Vector3 nextReverse = new Vector3(result, offset.y, camOffset.position.z);
            camOffset.position = Vector3.Lerp(camOffset.position, nextReverse, Time.deltaTime * reverseSpeed);
        }

        // jump
        if (canJump)
        {
            //Debug.Log(yAxis);
            //Debug.Log(Mathf.Clamp(currJumpForce, lowJumpForce, heighJumpForce));
            timer += Time.deltaTime;
            if (timer > 0.1f && isGrounded)
            {
                //Debug.Log(currJumpForce);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(currJumpForce, lowJumpForce, heighJumpForce) * Time.deltaTime);
                canJump = false;
            }
        }
    }
}
