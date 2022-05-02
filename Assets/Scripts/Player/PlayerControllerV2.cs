using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerV2 : MonoBehaviour
{
    [Header("Player")]
    private PlayerInput controls;
    private Rigidbody2D rb;
    [SerializeField] private float yRaycastGrounded;
    [SerializeField] private float yRaycastSize;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private SpriteRenderer leftDustSprite;
    [SerializeField] private SpriteRenderer rightDustSprite;
    [Header("MainCamera")]
    public Vector3 offset;
    [SerializeField] private float vcamMoveYSpeed;
    [SerializeField] private float vcamMoveYawSpeed;
    [SerializeField] private float reverseSpeed;
    [SerializeField] private float camCenterTimer;
    [SerializeField] private float xCameraDeadZone;
    [SerializeField] private Transform camOffset;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [Header("Jump")]
    [SerializeField] private float lowJumpForce;
    [SerializeField] private float heighJumpForce;
    [SerializeField] private float jumpAnalogImpact;
    [SerializeField] private float jumpBufferCooldown;
    [SerializeField] private AnimationCurve jumpCurve;
    [Header("Movement")]
    // la velocitySpeed est la vitesse à laquelle le personnage atteint sa vitesse max
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float inertia;
    [SerializeField] private AnimationCurve accelCurve;
    [Header("Dash")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;

    private Vector2 curSpeed;
    private Vector2 curDashSpeed;

    private float curJumpForce;
    private float curVelocitySpeed;
    private float curVcamMoveYSpeed;
    private float curGravity;
    private float maxCamCenterTimer;
    private float movement;
    private float timer = 0;
    private float result;
    private float yAxis;
    private float dashValue;
    private float dashTime;
    private float goDownCooldown;
    private float jumpBufferTimer = 0;

    public bool isGrounded;
    private bool isDashing;
    private bool isBuffing;
    private bool canJump;
    public bool canDash;
    private bool canResetCurMoveSpeed;
    private bool canReverse;
    private bool canGoDown;

    void Awake()
    {
        isGrounded = false;
        canJump = false;
        canDash = false;
        canReverse = true;
        canResetCurMoveSpeed = false;
        canGoDown = false;
        isBuffing = true;

        curSpeed = Vector2.zero;
        curVcamMoveYSpeed = vcamMoveYSpeed;

        controls = gameObject.GetComponent<PlayerInput>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        curGravity = rb.gravityScale;

        // défini maxCamCenterTimer à la valeur camCenterTimer dans l'inspecteur
        maxCamCenterTimer = camCenterTimer;
        // la vitesse actuelle du player est à zéro au début, car le player n'a pas encore bougé
        curVelocitySpeed = 0;

        // rotation de la caméra et des sprites en fonction de l'orientation du player
        #region Sprites & Cam : setup
        if (!playerSprite.flipX)
        {
            result = transform.position.x + offset.x;
            camOffset.position = new Vector3(transform.position.x + offset.x, camOffset.position.y, camOffset.position.z);
            leftDustSprite.enabled = true;
            rightDustSprite.enabled = false;
        }

        if (playerSprite.flipX)
        {
            canReverse = true;
            result = transform.position.x - offset.x;
            camOffset.position = new Vector3(transform.position.x + offset.x, camOffset.position.y, camOffset.position.z);
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = true;
        }
        #endregion
    }

    private void Update()
    {
        #region le déplacement
        if (!canDash)
        {
            float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

            if (xAxis > 0)
            {
                if (canResetCurMoveSpeed)
                {
                    curVelocitySpeed = 0;
                    canResetCurMoveSpeed = false;
                }
                playerSprite.flipX = false;
                leftDustSprite.enabled = true;
                rightDustSprite.enabled = false;
                curVelocitySpeed += accelCurve.Evaluate(Time.deltaTime * accelSpeed);
                if (curVelocitySpeed >= moveSpeed)
                {
                    curVelocitySpeed = moveSpeed;
                }

                canReverse = true;
            }
            if (xAxis < 0)
            {
                if (canResetCurMoveSpeed)
                {
                    curVelocitySpeed = 0;
                    canResetCurMoveSpeed = false;
                }
                playerSprite.flipX = true;
                leftDustSprite.enabled = false;
                rightDustSprite.enabled = true;
                curVelocitySpeed += accelCurve.Evaluate(Time.deltaTime * accelSpeed);
                if (curVelocitySpeed >= moveSpeed)
                {
                    curVelocitySpeed = moveSpeed;
                }

                canReverse = true;
            }

            //Debug.Log(movement);

            movement = xAxis;

            if (xAxis == 0 && dashValue == 0)
            {
                canResetCurMoveSpeed = true;
                curVelocitySpeed -= accelCurve.Evaluate(Time.deltaTime * accelSpeed);
                if (curVelocitySpeed <= 0)
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
                    camOffset.position = new Vector3(transform.position.x, camOffset.position.y, camOffset.position.z);
                    camCenterTimer = 0;
                }
            }
            else
            {
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0.5f;
                camCenterTimer = maxCamCenterTimer;
            }
        }
        #endregion
        
        //Debug.Log(result);
        
        #region le dash
        if (!canDash)
        {
            dashValue = controls.currentActionMap.FindAction("Dash").ReadValue<float>();
            if(dashValue != 0)
            {
                dashValue = Mathf.Sign(dashValue);
            }
            //Debug.Log(dashValue);

            if (dashValue > 0)
            {
                if (isDashing)
                {
                    canDash = true;
                    gameObject.GetComponent<PlayerHP>().canDie = false;
                    dashTime = dashDistance / dashSpeed;
                    isDashing = false;
                }

                leftDustSprite.enabled = true;
                rightDustSprite.enabled = false;
                canReverse = true;
            }

            if (dashValue < 0)
            {
                if (isDashing)
                {
                    canDash = true;
                    gameObject.GetComponent<PlayerHP>().canDie = false;
                    dashTime = dashDistance / dashSpeed;
                    isDashing = false;
                }

                leftDustSprite.enabled = false;
                rightDustSprite.enabled = true;
                canReverse = true;
            }

            if (dashValue == 0)
            {
                isDashing = true;
            }
        }
        #endregion

        #region saut
        if (controls.currentActionMap.FindAction("Jump").triggered)
        {
            //Debug.Log("saut normal");
            if (isBuffing)
            {
                canJump = true;
                isBuffing = false;
            }
        }

        curJumpForce = lowJumpForce * yAxis;
        yAxis += jumpCurve.Evaluate(controls.currentActionMap.FindAction("Jump").ReadValue<float>() * Time.deltaTime * jumpAnalogImpact);

        if (!canJump)
        {
            timer = 0;
            yAxis = 0;
        }

        jumpBufferTimer += Time.deltaTime;
        if (jumpBufferTimer >= jumpBufferCooldown)
        {
            isBuffing = true;
            jumpBufferTimer = 0;
        }

        //Debug.Log(jumpForce);

        #endregion

        #region isGrounded
        float distance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - yRaycastGrounded), -transform.up, distance);

        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - yRaycastGrounded), -transform.up * yRaycastSize, Color.red, 1);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Ground"))
            {
                //vcamMoveYSpeed = curVcamMoveYSpeed;
                isGrounded = true;
            }
        }
        else
        {
            //vcamMoveYSpeed = 20;
            if (rb.velocity.y < 0)
            {
                Invoke("Grounded", 0.2f);
            }
            else
            {
                isGrounded = false;
            }
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = false;
        }

        //Debug.Log(rb.velocity.y);

        #endregion

        #region vcam set Speed
        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = vcamMoveYSpeed;
        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping = vcamMoveYawSpeed;
        #endregion

        #region idle vcam offset
        if (canReverse)
        {
            switch (playerSprite.flipX)
            {
                case true:
                    result = transform.position.x - offset.x;
                    break;
                case false:
                    result = transform.position.x + offset.x;
                    break;
            }
        }
        #endregion

        #region goDown
        if (controls.currentActionMap.FindAction("Down").triggered)
        {
            canGoDown = true;
            //Debug.Log("input down");
        }

        //Debug.Log(canGoDown);

        if (!canGoDown && gameObject.layer != 6)
        {
            gameObject.layer = 0;
        }
        else
        {
            goDownCooldown += Time.deltaTime;
            if (goDownCooldown >= 1f)
            {
                goDownCooldown = 0.25f;
                canGoDown = false;
            }
        }

        if (canGoDown)
        {
            //Debug.Log("ignore layer");
            gameObject.layer = 8;
        }
        #endregion
    }

    void FixedUpdate()
    {
        // accel
        float curseurAccel = 1;
        if (movement == 0)
        {
            curseurAccel = Time.deltaTime * inertia;
        }
        curSpeed = Vector2.Lerp(curSpeed, new Vector2(curVelocitySpeed, curSpeed.y) * movement, curseurAccel);
        Vector3 nextPosition = new Vector3(transform.position.x + curSpeed.x * Time.deltaTime, transform.position.y, transform.position.z);
        rb.position = nextPosition;
        //Debug.Log((int)curVelocitySpeed);

        // dash
        float curseurDash = 1;
        if (dashValue == 0)
        {
            curseurDash = Time.deltaTime * inertia;
        }

        if (canDash && dashTime>0)
        {
            //Debug.Log("is dashing");
            curGravity = 10;

            dashTime -= Time.deltaTime;
        }

        if (canDash)
        {
            curDashSpeed = Vector2.Lerp(curDashSpeed, new Vector2(dashSpeed, curDashSpeed.y) * dashValue, curseurDash);
            Vector3 nextDashPos = new Vector3(transform.position.x + curDashSpeed.x * Time.deltaTime, transform.position.y, transform.position.z);
            rb.position = nextDashPos;
        }

        if (dashTime <= 0 && isGrounded)
        {
            canDash = false;
            curGravity = 5;
            gameObject.GetComponent<PlayerHP>().canDie = true;
        }

        Debug.Log(curseurDash);

        // mouvement de la caméra sur l'axe x lors que le personnage se tourne
        if (canReverse)
        {
            Vector3 nextReverse = new Vector3(Mathf.Clamp(result, xCameraDeadZone, result), camOffset.position.y, camOffset.position.z);
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
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(curJumpForce, lowJumpForce, heighJumpForce) * Time.deltaTime);
                canJump = false;
            }
        }

        //Debug.Log(rb.velocity);
        rb.gravityScale = curGravity;
    }

    void Grounded()
    {
        isGrounded = false;
    }
}
