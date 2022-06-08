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
    [SerializeField] private Transform camOffset;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [Header("Jump")]
    [SerializeField] private float lowJumpForce;
    [SerializeField] private float heighJumpForce;
    [SerializeField] private float jumpAnalogImpact;
    [SerializeField] private float jumpBufferCooldown;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private RectMask2D jumpUI;
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
    [SerializeField] private GameObject dashVFX;
    [Header("Dodge")]
    [SerializeField] private float dodgeDistance;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private float dodgeCooldown;
    [SerializeField] private RectMask2D dodgeUI;

    private Vector2 curSpeed;
    private Vector2 curDashSpeed;
    private Vector2 curDodgeSpeed;

    private float curJumpForce;
    private float curVelocitySpeed;
    private float curGravity;
    private float curPosY;
    private float camOffsetPosY;
    private float curCamOffsetPosY;
    private float maxCamCenterTimer;
    private float movement;
    private float timer = 0;
    private float timerDash;
    private float timerDodge;
    private float result;
    private float yAxis;
    private float dashValue;
    private float dodgeValue;
    [HideInInspector] public float dashTime;
    private float dodgeTime;
    private float goDownCooldown;
    private float jumpBufferTimer = 0;
    private float raycastDir;
    private float paddingSpeedUI;
    private float raycastDir2;
    private int playerForward;
    private int playerBackward;

    [HideInInspector] public bool isGroundedL;
    [HideInInspector] public bool isGroundedR;
    [HideInInspector] public bool isDashUIStarted;
    private bool isDashing;
    private bool isDodging;
    private bool isBuffing;
    private bool canJump;
    private bool canDash;
    private bool canDodge;
    private bool canResetCurMoveSpeed;
    private bool canReverse;
    private bool canGoDown;
    private bool canResetCamY;
    private bool isPlayingJumpAnim;
    private bool isOnBox;
    private bool isPlayingStopAnim;
    private bool canDoSFX_Run;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isGroundedL = false;
        isGroundedR = false;
        canJump = false;
        canDash = false;
        canDodge = false;
        canReverse = true;
        canResetCurMoveSpeed = false;
        canGoDown = false;
        isBuffing = true;
        canResetCamY = false;
        isPlayingJumpAnim = false;
        isOnBox = false;
        isPlayingStopAnim = false;
        canDoSFX_Run = false;

        dashUI.padding = new Vector4(0, 0, 0, 4.6f);
        dodgeUI.padding = new Vector4(0, 0, 0, 4.6f);
        jumpUI.padding = new Vector4(0, 0, 0, 109);
        timerDash = dashCooldown;
        timerDodge = dodgeCooldown;

        curSpeed = Vector2.zero;

        controls = gameObject.GetComponent<PlayerInput>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        curGravity = rb.gravityScale;

        // d�fini maxCamCenterTimer � la valeur camCenterTimer dans l'inspecteur
        maxCamCenterTimer = camCenterTimer;
        // la vitesse actuelle du player est de zero au debut, car le player n'a pas encore bouge
        curVelocitySpeed = 0;

        // la vitesse de recharge (pour les UI de dash et de dodge)
        paddingSpeedUI = 25;

        // rotation de la cam�ra et des sprites en fonction de l'orientation du player
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

        camOffsetPosY = camOffset.localPosition.y;
        dashVFX.SetActive(false);
        #endregion
    }

    private void Update()
    {
        #region le deplacement
        if (!canDash && !canDodge)
        {
            float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();
            //Debug.Log(xAxis);

            animator.SetFloat("Movement", xAxis);

            if (xAxis > 0)
            {
                xAxis = 1;
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
                xAxis = -1;
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
                canDoSFX_Run = true;

                //Debug.Log("cut walk audio");
                AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

                for (int i = 0; i < audioS.Length; i++)
                {
                    if(audioS[i].clip.name == "Boots_Run")
                    {
                        audioS[i].Stop();
                    }
                }

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

        if (canDoSFX_Run && movement != 0 && (isGroundedL && isGroundedR))
        {
            //Debug.Log("walk sfx L" + isGroundedL + "walk sfx R" + isGroundedR);

            AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

            for (int i = 0; i < audioS.Length; i++)
            {
                if (audioS[i].clip.name == "Boots_Run")
                {
                    audioS[i].loop = true;
                }
            }
            FindObjectOfType<AudioManager>().Play("bootsRun");
            canDoSFX_Run = false;
        }

        //Debug.Log(rb.velocity.y);

        if(movement != 0 && (!isGroundedL && !isGroundedR) && rb.velocity.y > 1)
        {
            //Debug.Log("can walk sfx");
            canDoSFX_Run = true;

            AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

            for (int i = 0; i < audioS.Length; i++)
            {
                if (audioS[i].clip.name == "Boots_Run")
                {
                    audioS[i].Stop();
                }
            }
        }
        #endregion

        //Debug.Log(result);

        #region gachette de droite
        timerDash -= Time.deltaTime;
        //Debug.Log(timerDash);
        //Debug.Log("isDashing" + isDashUIStarted);

        if (!canDash)
        {
            dashValue = controls.currentActionMap.FindAction("Dash").ReadValue<float>();
            if (dashValue != 0)
            {
                dashValue = Mathf.Sign(dashValue);
            }
            //Debug.Log(dashValue);

            if (dashValue > 0 && !canDodge && !gameObject.GetComponent<PlayerAttack>().IsSpiking() && !gameObject.GetComponent<PlayerAttack>().IsWalling())
            {
                switch (playerSprite.flipX)
                {
                    case true:
                        if (timerDodge <= 0)
                        {
                            float random = Random.value;
                            if (random <= 0.5f)
                            {
                                FindObjectOfType<AudioManager>().Play("iceWoosh");
                            }
                            else if (random > 0.5f)
                            {
                                FindObjectOfType<AudioManager>().Play("iceWoosh2");
                            }

                            //Debug.Log("dodge");
                            if (isDashing)
                            {
                                canDodge = true;
                                gameObject.GetComponent<PlayerHP>().canDie = false;
                                animator.SetTrigger("Dodge");
                                dodgeTime = dodgeDistance / dodgeSpeed;
                                dashUI.padding = new Vector4(0, 0, 0, 4.6f);
                                paddingSpeedUI = 10;
                                timerDodge = dodgeCooldown;
                                isDashing = false;
                            }
                            canReverse = true;
                        }
                        break;
                    case false:
                        if (timerDash <= 0)
                        {
                            float random = Random.value;
                            if (random <= 0.5f)
                            {
                                FindObjectOfType<AudioManager>().Play("iceWoosh");
                            }
                            else if (random > 0.5f)
                            {
                                FindObjectOfType<AudioManager>().Play("iceWoosh2");
                            }

                            isDashUIStarted = true;
                            if (isDashing)
                            {
                                canDash = true;
                                Physics2D.IgnoreLayerCollision(0, 3, true);
                                gameObject.GetComponent<PlayerHP>().canDie = false;
                                animator.SetTrigger("Dash");
                                dashTime = dashDistance / dashSpeed;
                                dashUI.padding = new Vector4(0, 0, 0, 4.6f);
                                paddingSpeedUI = 4.5f;
                                timerDash = dashCooldown;
                                isDashing = false;
                            }
                            canReverse = true;
                        }
                        break;
                }
            }

            if (dashValue == 0)
            {
                isDashing = true;
            }
        }

        if (isDashUIStarted)
        {
            Invoke("IsDashUIStopped", 0.25f);
        }
        #endregion

        #region gachette de gauche
        timerDodge -= Time.deltaTime;
        //Debug.Log(timerDodge);

        if (!canDodge)
        {

            dodgeValue = controls.currentActionMap.FindAction("Dodge").ReadValue<float>();
            if (dodgeValue != 0)
            {
                dodgeValue = Mathf.Sign(dodgeValue);
            }
            //Debug.Log(dodgeValue);

            if (dodgeValue > 0 && !canDash && !gameObject.GetComponent<PlayerAttack>().IsSpiking() && !gameObject.GetComponent<PlayerAttack>().IsWalling())
            {
                switch (playerSprite.flipX)
                {
                    case true:
                        if (timerDash <= 0)
                        {
                            FindObjectOfType<AudioManager>().Play("iceWoosh");

                            isDashUIStarted = true;
                            if (isDodging)
                            {
                                canDash = true;
                                Physics2D.IgnoreLayerCollision(0, 3, true);
                                gameObject.GetComponent<PlayerHP>().canDie = false;
                                animator.SetTrigger("Dash");
                                dashTime = dashDistance / dashSpeed;
                                dodgeUI.padding = new Vector4(0, 0, 0, 4.6f);
                                paddingSpeedUI = 4.5f;
                                timerDash = dashCooldown;
                                isDodging = false;
                            }
                            canReverse = true;
                        }
                        break;
                    case false:
                        if (timerDodge <= 0)
                        {
                            FindObjectOfType<AudioManager>().Play("iceWoosh");

                            //Debug.Log("dodge");
                            if (isDodging)
                            {
                                canDodge = true;
                                gameObject.GetComponent<PlayerHP>().canDie = false;
                                animator.SetTrigger("Dodge");
                                dodgeTime = dodgeDistance / dodgeSpeed;
                                dodgeUI.padding = new Vector4(0, 0, 0, 4.6f);
                                paddingSpeedUI = 10;
                                timerDodge = dodgeCooldown;
                                isDodging = false;
                            }
                            canReverse = true;
                        }
                        break;
                }

            }

            if (dodgeValue == 0)
            {
                isDodging = true;
            }
        }
        #endregion

        #region saut
        if (controls.currentActionMap.FindAction("Jump").triggered && (isGroundedR || isGroundedL))
        {
            //Debug.Log("saut normal");
            if (isBuffing)
            {
                FindObjectOfType<AudioManager>().Play("Jump");

                canJump = true;
                jumpUI.padding = new Vector4(0, 0, 0, 109);
                animator.SetTrigger("Jump");
                isPlayingJumpAnim = true;

                isBuffing = false;
            }
        }

        if (isGroundedL && isGroundedR && !canJump)
        {
            curPosY = rb.position.y;
            isBuffing = true;
            jumpBufferTimer = 0;
            //Debug.Log("can jump");
        }

        if (isGroundedL || isGroundedR)
        {
            animator.SetBool("isGrounded", true);
        }

        if (!isGroundedL && !isGroundedR)
        {
            animator.SetBool("isGrounded", false);
            //Debug.Log("can buff");

            jumpBufferTimer += Time.deltaTime;
            if (jumpBufferTimer >= jumpBufferCooldown && controls.currentActionMap.FindAction("Jump").triggered)
            {
                //Debug.Log("buff");
                FindObjectOfType<AudioManager>().Play("Jump");

                canJump = true;
                jumpUI.padding = new Vector4(0, 0, 0, 109);
                animator.SetTrigger("Jump");

                jumpBufferTimer = 0;
            }
        }

        curJumpForce = lowJumpForce * yAxis;
        yAxis += jumpCurve.Evaluate(controls.currentActionMap.FindAction("Jump").ReadValue<float>() * Time.deltaTime * jumpAnalogImpact);

        if (!canJump)
        {
            yAxis = 0;
        }

        //Debug.Log(jumpForce);
        #endregion

        #region fall

        if ((isGroundedL || isGroundedR) && rb.velocity.y < -5 && rb.position.y < curPosY && !isPlayingStopAnim && !canJump)
        {
            StartCoroutine(StopAnim());
            isPlayingJumpAnim = false;
            //Debug.Log("isGrounded");
        }

        if (!isGroundedL && !isGroundedR)
        {
            isPlayingJumpAnim = true;
        }

        if (!isPlayingJumpAnim && !canDodge)
        {
            animator.Play("Yuki_Fall");
            //Debug.Log("fall");
        }

        //Debug.Log(canDodge);
        //Debug.Log(isPlayingJumpAnim);
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
                //Debug.Log("isGrounded Left : "+isGroundedL);
                //vcamMoveYSpeed = curVcamMoveYSpeed;
                isGroundedL = true;
            }
        }
        else
        {
            //vcamMoveYSpeed = 20;
            if (rb.velocity.y < 0)
            {
                //Debug.Log("Y < 0");
                Invoke("GroundedL", 0.2f);
            }
            else
            {
                //Debug.Log("Y = 0");
                isGroundedL = false;
            }
            leftDustSprite.enabled = false;
            rightDustSprite.enabled = false;
        }

        //Debug.Log("isGroundedL" + isGroundedL + "isGroundedR" + isGroundedR);
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
                //Debug.Log("isGrounded Right : " + isGroundedR);
                //vcamMoveYSpeed = curVcamMoveYSpeed;
                isGroundedR = true;
            }
        }
        else
        {
            //vcamMoveYSpeed = 20;
            if (rb.velocity.y < 0)
            {
                //Debug.Log("Y < 0");
                Invoke("GroundedR", 0.2f);
            }
            else
            {
                //Debug.Log("Y = 0");
                isGroundedR = false;
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
                    playerForward = -1;
                    playerBackward = 1;
                    result = transform.position.x - offset.x;
                    raycastDir = xRaycastOffset;
                    raycastDir2 = -xRaycastOffset;
                    break;
                case false:
                    playerForward = 1;
                    playerBackward = -1;
                    result = transform.position.x + offset.x;
                    raycastDir = -xRaycastOffset;
                    raycastDir2 = xRaycastOffset;
                    break;
            }
        }
        #endregion

        #region goDown
        if (controls.currentActionMap.FindAction("Down").triggered && isOnBox)
        {
            canGoDown = true;
            animator.Play("Yuki_Fall");
            if (!isPlayingStopAnim)
            {
                StartCoroutine(StopAnim());
            }

            //Debug.Log("input down");
        }

        //Debug.Log(isOnBox);

        if (hit && hit2)
        {
            if(hit.collider.gameObject.layer == 9 && hit2.collider.gameObject.layer == 9)
            {
                //Debug.Log(hit.collider.gameObject.GetComponent<PlatformEffector2D>().colliderMask);

                if (hit.collider.gameObject.GetComponent<PlatformEffector2D>())
                {
                    if(hit.collider.gameObject.GetComponent<PlatformEffector2D>().colliderMask == 767)
                    {
                        isOnBox = true;
                    }
                }
            }

            if (hit.collider.gameObject.layer != 9 && hit2.collider.gameObject.layer != 9)
            {
                isOnBox = false;
            }
        }
        else
        {
            isOnBox = false;
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

        #region vcam Y Axis
        //Debug.Log("isGrounded = " + isGrounded);
        float dist = curPosY - rb.position.y;

        // si le player saute pas
        if (isGroundedL && isGroundedR && dist < 0.1f)
        {
            //Debug.Log("speed");
            canResetCamY = true;
            if (movement != 0 && canJump)
            {
                float timer = Time.deltaTime * 30;
                vcamMoveYSpeed = Mathf.Lerp(vcamMoveYSpeed, 0, timer);
            }
            else if (movement == 0)
            {
                float timer = Time.deltaTime * 5;
                vcamMoveYSpeed = Mathf.Lerp(vcamMoveYSpeed, 0, timer);
            }
            float timer2 = Time.deltaTime * 3;
            float newPos = Mathf.Lerp(camOffset.localPosition.y, camOffsetPosY, timer2);
            camOffset.localPosition = new Vector2(camOffset.localPosition.x, newPos);
            //Debug.Log(timer);
        }

        //Debug.Log(curVelocitySpeed);

        // si le player saute
        if (rb.position.y > curPosY && rb.velocity.y > -15 || canJump && isGroundedL & isGroundedR)
        {
            //Debug.Log("slow");
            if (canResetCamY)
            {
                camOffset.localPosition = new Vector2(camOffset.localPosition.x, camOffsetPosY);
                canResetCamY = false;
            }
            float timer = Time.deltaTime;
            float timer2 = Time.deltaTime * 10;
            float newPos = Mathf.Lerp(camOffset.localPosition.y, camOffsetPosY, timer2);
            camOffset.localPosition = new Vector2(camOffset.localPosition.x, newPos);
            //Debug.Log("rb.velocity.y " + rb.velocity.y);
            vcamMoveYSpeed = Mathf.Lerp(vcamMoveYSpeed, 20, timer);
        }

        // si le player tombe
        if (rb.velocity.y < -10)
        {
            //Debug.Log("fall");
            float timer = Time.deltaTime * (1.25f * curCamOffsetPosY);
            float timer2 = Time.deltaTime * (7 / curCamOffsetPosY);
            float newPos = Mathf.Lerp(camOffset.localPosition.y, camOffsetPosY - (-rb.velocity.y / 1.5f), timer2);
            camOffset.localPosition = new Vector2(camOffset.localPosition.x, newPos);
            //Debug.Log("newPos " + newPos);
            //Debug.Log(timer);
            vcamMoveYSpeed = Mathf.Lerp(vcamMoveYSpeed, 0, timer);
        }

        curCamOffsetPosY = camOffset.localPosition.y;
        //Debug.Log(rb.position.y);
        //Debug.Log(curPosY);

        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = vcamMoveYSpeed;
        vcam.GetCinemachineComponent<CinemachineTransposer>().m_YawDamping = vcamMoveYawSpeed;
        #endregion

        #region vfx pour dash et dodge

        if (!canDash && !canDodge)
        {
            dashVFX.SetActive(false);
        }

        if (canDash || canDodge)
        {
            dashVFX.SetActive(true);
        }
        #endregion
    }

    void FixedUpdate()
    {
        #region accel
        float curseurAccel = 1;
        //if (movement == 0 && !canDash && !canDodge)
        //{
        //    curseurAccel = Time.deltaTime / inertia;
        //}
        curSpeed = Vector2.Lerp(curSpeed, new Vector2(curVelocitySpeed, curSpeed.y) * movement, curseurAccel);
        Vector3 nextPosition = new Vector3(transform.position.x + curSpeed.x * Time.deltaTime, transform.position.y, transform.position.z);
        rb.position = nextPosition;
        //Debug.Log((int)curVelocitySpeed);
        #endregion

        #region dash
        float curseurDash = 1;

        if (canDash && dashTime > 0)
        {
            //Debug.Log("is dashing");
            curGravity = 6;
            isBuffing = false;

            dashTime -= Time.deltaTime;
        }

        if (canDash)
        {
            canJump = false;
            curDashSpeed = Vector2.Lerp(curDashSpeed, new Vector2(dashSpeed, curDashSpeed.y) * playerForward, curseurDash);
            Vector3 nextDashPos = new Vector3(transform.position.x + curDashSpeed.x * Time.deltaTime, transform.position.y, transform.position.z);
            rb.position = nextDashPos;
        }

        if (dashTime <= 0 && (isGroundedL || isGroundedR) && canDash)
        {
            canDash = false;
            curGravity = 5;
            gameObject.GetComponent<PlayerHP>().canDie = true;
            Physics2D.IgnoreLayerCollision(0, 3, false);
        }

        dashUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(dashUI.padding.w - (dashCooldown * (Time.deltaTime* paddingSpeedUI)), 0.3f, 4.6f));
        if(dashUI.padding.w <= 0.3f)
        {
            //Debug.Log("full");
            dashUI.gameObject.SetActive(false);
        }
        else
        {
            dashUI.gameObject.SetActive(true);
        }

        //Debug.Log(curseurDash);
        #endregion

        #region dodge
        float curseurDodge = 1;

        if (canDodge && dodgeTime > 0)
        {
            //Debug.Log("is dodging");
            curGravity = 6;
            isBuffing = false;

            dodgeTime -= Time.deltaTime;
        }

        if (canDodge)
        {
            canJump = false;
            Physics2D.IgnoreLayerCollision(0, 3, true);
            curDodgeSpeed = Vector2.Lerp(curDodgeSpeed, new Vector2(dodgeSpeed, curDodgeSpeed.y) * playerBackward, curseurDodge);
            Vector3 nextDodgePos = new Vector3(transform.position.x + curDodgeSpeed.x * Time.deltaTime, transform.position.y, transform.position.z);
            rb.position = nextDodgePos;
        }

        if (dodgeTime <= 0 && (isGroundedL || isGroundedR) && canDodge)
        {
            canDodge = false;
            curGravity = 5;
            gameObject.GetComponent<PlayerHP>().canDie = true;
            Physics2D.IgnoreLayerCollision(0, 3, false);
        }

        dodgeUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(dodgeUI.padding.w - (dashCooldown * (Time.deltaTime * paddingSpeedUI)), 0.3f, 4.6f));
        if (dodgeUI.padding.w <= 0.3f)
        {
            //Debug.Log("full");
            dodgeUI.gameObject.SetActive(false);
        }
        else
        {
            dodgeUI.gameObject.SetActive(true);
        }

        //Debug.Log(curseurDodge);
        #endregion

        #region vcam X Axis
        // mouvement de la cam�ra sur l'axe x lors que le personnage se tourne
        if (canReverse)
        {
            Vector3 nextReverse = new Vector3(result, camOffset.position.y, camOffset.position.z);
            camOffset.position = Vector3.Lerp(camOffset.position, nextReverse, Time.deltaTime * reverseSpeed);
        }
        #endregion

        #region jump
        if (canJump)
        {
            if (!canDash && !canDodge)
            {
                //Debug.Log("add Gravity");
                curGravity = 8;
            }
            //Debug.Log(yAxis);
            //Debug.Log(Mathf.Clamp(currJumpForce, lowJumpForce, heighJumpForce));
            timer += Time.deltaTime;
            if (timer > 0.1f && (isGroundedL || isGroundedR))
            {
                //Debug.Log(curJumpForce);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(curJumpForce, lowJumpForce, heighJumpForce) * Time.deltaTime);
                timer = 0;
                canJump = false;
            }
        }

        jumpUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(jumpUI.padding.w - 10f, 0, 109));

        #endregion

        #region gravit�
        //Debug.Log(rb.velocity);
        rb.gravityScale = curGravity;
        #endregion
    }

    /// <summary>
    /// Coyote Time for isGrounded L
    /// </summary>
    void GroundedL()
    {
        if (isGroundedL)
        {
            isGroundedL = false;
            //Debug.Log(isGroundedL);
        }
    }

    /// <summary>
    /// Coyote Time for isGrounded R
    /// </summary>
    void GroundedR()
    {
        if (isGroundedR)
        {
            //Debug.Log(isGroundedR);
            isGroundedR = false;
        }
    }

    /// <summary>
    /// Return true if player is dashing and false if not
    /// </summary>
    /// <returns></returns>
    public bool IsDashing()
    {
        if (canDash)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// stop la boolean isDashUIStarted
    /// </summary>
    void IsDashUIStopped()
    {
        isDashUIStarted = false;
    }

    /// <summary>
    /// stop fall animation
    /// </summary>
    /// <returns></returns>
    IEnumerator StopAnim()
    {
        isPlayingStopAnim = true;
        yield return new WaitForSeconds(0.25f);
        animator.SetTrigger("isFalling");
        isPlayingStopAnim = false;
        yield break;
    }
}
