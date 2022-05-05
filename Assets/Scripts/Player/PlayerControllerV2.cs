using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerControllerV2 : MonoBehaviour
{
    [Header("Player")]
    private PlayerInput controls;
    private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float yRaycastGrounded;
    [SerializeField] private float yRaycastSize;
    [SerializeField] private float xRaycastOffset;
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
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float inertia;
    [SerializeField] private AnimationCurve accelCurve;
    [Header("Dash")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    [SerializeField] private RectMask2D dashUI;

    private Vector2 curSpeed;
    private Vector2 curDashSpeed;

    private float curJumpForce;
    private float curVelocitySpeed;
    private float curVcamMoveYSpeed;
    private float curGravity;
    private float curPosY;
    private float lastPosY;
    private float maxCamCenterTimer;
    private float movement;
    private float timer = 0;
    private float timerDash;
    private float result;
    private float yAxis;
    private float dashValue;
    [HideInInspector] public float dashTime;
    private float goDownCooldown;
    private float jumpBufferTimer = 0;
    private float raycastDir;
    private float raycastDir2;

    [HideInInspector] public bool isGrounded;
    private bool isDashing;
    private bool isBuffing;
    private bool canJump;
    private bool canDash;
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

        dashUI.padding = new Vector4(0, 0, 0, 134);
        timerDash = dashCooldown;

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

            animator.SetFloat("Movement", xAxis);

            if (xAxis > 0)
            {
                if (canResetCurMoveSpeed)
                {
                    curVelocitySpeed = 0;
                    canResetCurMoveSpeed = false;
                }
                playerSprite.flipX = false;
                raycastDir = -xRaycastOffset;
                raycastDir2 = xRaycastOffset;
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
                raycastDir = xRaycastOffset;
                raycastDir2 = -xRaycastOffset;
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
                leftDustSprite.enabled = false;
                rightDustSprite.enabled = false;
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
        timerDash -= Time.deltaTime;
        //Debug.Log(timerDash);

        if (!canDash)
        {
            dashValue = controls.currentActionMap.FindAction("Dash").ReadValue<float>();
            if(dashValue != 0)
            {
                dashValue = Mathf.Sign(dashValue);
            }
            Debug.Log(dashValue);

            if (dashValue > 0 && timerDash <= 0)
            {
                if (isDashing)
                {
                    canDash = true;
                    gameObject.GetComponent<PlayerHP>().canDie = false;
                    dashTime = dashDistance / dashSpeed;
                    dashUI.padding = new Vector4(0, 0, 0, 134);
                    timerDash = dashCooldown;
                    isDashing = false;
                }
                canReverse = true;
            }

            if (dashValue < 0 && timerDash <= 0)
            {
                if (isDashing)
                {
                    canDash = true;
                    gameObject.GetComponent<PlayerHP>().canDie = false;
                    dashTime = dashDistance / dashSpeed;
                    dashUI.padding = new Vector4(0, 0, 0, 134);
                    timerDash = dashCooldown;
                    isDashing = false;
                }
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

        #region isGrounded Left
        float distance = 1f;
        float xPos = transform.position.x + raycastDir;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(xPos, transform.position.y - yRaycastGrounded), -transform.up, distance);

        Debug.DrawRay(new Vector2(xPos, transform.position.y - yRaycastGrounded), -transform.up * yRaycastSize, Color.red, 1);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Ground"))
            {
                //vcamMoveYSpeed = curVcamMoveYSpeed;
                curPosY = rb.position.y;
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

        #region isGrounded Right
        float distance2 = 1f;
        float xPos2 = transform.position.x + raycastDir2;
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(xPos2, transform.position.y - yRaycastGrounded), -transform.up, distance2);

        Debug.DrawRay(new Vector2(xPos2, transform.position.y - yRaycastGrounded), -transform.up * yRaycastSize, Color.red, 1);
        if (hit2)
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit2.collider.CompareTag("Ground"))
            {
                //vcamMoveYSpeed = curVcamMoveYSpeed;
                curPosY = rb.position.y;
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

        #region idle vcam offset
        if (canReverse)
        {
            switch (playerSprite.flipX)
            {
                case true:
                    result = transform.position.x - offset.x;
                    raycastDir = xRaycastOffset;
                    raycastDir2 = -xRaycastOffset;
                    break;
                case false:
                    result = transform.position.x + offset.x;
                    raycastDir = -xRaycastOffset;
                    raycastDir2 = xRaycastOffset;
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
        #region accel
        float curseurAccel = 1;
        if (movement == 0)
        {
            curseurAccel = Time.deltaTime * inertia;
        }
        curSpeed = Vector2.Lerp(curSpeed, new Vector2(curVelocitySpeed, curSpeed.y) * movement, curseurAccel);
        Vector3 nextPosition = new Vector3(transform.position.x + curSpeed.x * Time.deltaTime, transform.position.y, transform.position.z);
        rb.position = nextPosition;
        //Debug.Log((int)curVelocitySpeed);
        #endregion

        #region dash
        float curseurDash = 1;
        if (dashValue == 0)
        {
            curseurDash = Time.deltaTime * inertia;
        }

        if (canDash && dashTime>0)
        {
            //Debug.Log("is dashing");
            curGravity = 6;
            isBuffing = false;

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

        dashUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(dashUI.padding.w - dashCooldown, 82, 134));

        //Debug.Log(curseurDash);
        #endregion

        #region vcam X Axis
        // mouvement de la caméra sur l'axe x lors que le personnage se tourne
        if (canReverse)
        {
            Vector3 nextReverse = new Vector3(Mathf.Clamp(result, xCameraDeadZone, result), camOffset.position.y, camOffset.position.z);
            camOffset.position = Vector3.Lerp(camOffset.position, nextReverse, Time.deltaTime * reverseSpeed);
        }
        #endregion

        #region jump
        if (canJump)
        {
            if (!canDash)
            {
                curGravity = 10;
            }
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
        #endregion

        #region gravité
        //Debug.Log(rb.velocity);
        rb.gravityScale = curGravity;
        #endregion

        #region vcam Y Axis
        StartCoroutine(VcamStartMove());
        //Debug.Log("isGrounded = " + isGrounded);

        if (canJump)
        {
            //Debug.Log("Stop");
            vcamMoveYSpeed = 5;
        }

        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = vcamMoveYSpeed;
        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping = vcamMoveYawSpeed;
        #endregion
    }

    /// <summary>
    /// Coyote Time for isGrounded
    /// </summary>
    void Grounded()
    {
        isGrounded = false;
    }

    /// <summary>
    /// vcam Y Axis when player move
    /// </summary>
    /// <returns></returns>
    IEnumerator VcamStartMove()
    {
        yield return new WaitForSeconds(1.2f);

        lastPosY = rb.position.y;

        while (vcamMoveYSpeed > 0 && !canJump && isGrounded || canJump && movement != 0)
        {
            //Debug.Log("Play");
            float timer = Time.deltaTime * 25;
            vcamMoveYSpeed = Mathf.Lerp(vcamMoveYSpeed, 0, timer);
            yield break;
        }

        while(vcamMoveYSpeed > 0 && !canJump && rb.velocity.y < -1 || canDash)
        {
            //Debug.Log("Fall");
            float timer = Time.deltaTime * 50;
            vcamMoveYSpeed = Mathf.Lerp(vcamMoveYSpeed, 0, timer);
            yield break;
        }
    }
}
