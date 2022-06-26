using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Transform attackPos;
    [SerializeField] private Vector2 attackPosDistance;
    [SerializeField] private int damage;
    private float attackRange;
    [SerializeField] private float firstAttackRange;
    [SerializeField] private float secondAttackRange;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineVirtualCamera bcam;
    [SerializeField] private LayerMask mask;

    [Header("IceBar")]
    [SerializeField] private int iceToAdd;
    [SerializeField] GameObject bulletIce;
    [Header("Slash")]
    [SerializeField] private RectMask2D attackUI;
    //[SerializeField] private GameObject slashEffect;
    //[SerializeField] private GameObject slashEffect2;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Animator attackAnim;
    [SerializeField] float timerStopAttack;
    public bool isAttacking { get; private set; }
    [Header("Wall")]
    [SerializeField] private RectMask2D wallUI;
    [SerializeField] private GameObject wallEffect;
    [SerializeField] private GameObject wallEffect2;
    [SerializeField] private GameObject wall;
    [SerializeField] private float wallCooldown;
    [Header("Spike")]
    [SerializeField] private RectMask2D spikeUI;
    [SerializeField] private GameObject spike;
    [SerializeField] private float spikeCooldown;
    [SerializeField] private float spikeSpeed;
    [SerializeField] private AnimationCurve spikeSpeedCurve;
    private float curSpikeSpeed;
    [Header("VFX")]
    [SerializeField] private GameObject fullBarVFX;
    private GameObject jumpFullBarVFX_instance;
    private GameObject wallFullBarVFX_instance;
    private GameObject spikeFullBarVFX_instance;

    private PlayerInput controls;
    private float timerAttack;
    private float timerSpike;
    private float timerWall;
    private float timerSheathe = 1f;
    private int slashOrder;
    private Vector2 spikeLerp;
    private GameObject instance;
    private GameObject instance2;
    private bool lastFlip;
    private bool isSpiking;
    private bool isWalling;
    private bool canSpawnAttackfullBarVFX;
    private bool canSpawnWallfullBarVFX;
    private bool canSpawnSpikefullBarVFX;
    private bool hasAttacked;

    private void Awake()
    {
        #region cooldowns
        timerAttack = attackCooldown;
        timerWall = wallCooldown;
        timerSpike = spikeCooldown;
        wallUI.padding = new Vector4(0, 0, 0, 0);
        canSpawnWallfullBarVFX = true;
        spikeUI.padding = new Vector4(0, 0, 0, 4);
        canSpawnSpikefullBarVFX = true;
        attackUI.padding = new Vector4(0, 0, 0, 3);
        canSpawnAttackfullBarVFX = true;
        #endregion

        #region get
        controls = gameObject.GetComponent<PlayerInput>();
        #endregion

        attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);
        slashOrder = 2;

        #region wall set
        wallEffect2.SetActive(false);
        wallEffect2.GetComponent<Animator>().SetBool("Recover", false);
        wallEffect.SetActive(false);
        wallEffect.GetComponent<Animator>().SetBool("Recover", false);
        #endregion
    }

    private void Update()
    {
        //Debug.Log(timerDamage);

        #region attack for sprites and ice bar : with attackCooldown

        if (playerSprite.flipX != lastFlip)
        {
            //Debug.Log("reset slash anim");
            slashOrder = 2;
        }

        if (hasAttacked)
        {
            timerSheathe -= Time.deltaTime;

            if (timerSheathe <= 0)
            {
                SheatheSword();
            }
        }

        timerAttack -= Time.deltaTime;
        if (controls.currentActionMap.FindAction("Attack").triggered && timerAttack <= 0)
        {
            isAttacking = true;
            Invoke("StopAttack", timerStopAttack);
            for (int i = 0; i < 3; i++)
            {
                CreateBullet();
            }
            hasAttacked = true;
            timerSheathe = 0.2f;
            switch (playerSprite.flipX)
            {
                case true:
                    lastFlip = playerSprite.flipX;
                    switch (slashOrder)
                    {
                        case 1:
                            attackRange = secondAttackRange;
                            FindObjectOfType<AudioManager>().Play("iceSword");

                            slashOrder = 2;

                            attackAnim.SetTrigger("isAttacking");
                            attackAnim.SetInteger("attackIndex", slashOrder);

                            break;
                        case 2:
                            attackRange = firstAttackRange;
                            FindObjectOfType<AudioManager>().Play("iceSword2");

                            slashOrder = 1;

                            attackAnim.SetTrigger("isAttacking");
                            attackAnim.SetInteger("attackIndex", slashOrder);

                            break;
                    }
                    break;
                case false:
                    lastFlip = playerSprite.flipX;
                    //slashEffect.SetActive(true);
                    switch (slashOrder)
                    {
                        case 1:
                            attackRange = secondAttackRange;
                            FindObjectOfType<AudioManager>().Play("iceSword");

                            slashOrder = 2;

                            attackAnim.SetTrigger("isAttacking");
                            attackAnim.SetInteger("attackIndex", slashOrder);

                            break;
                        case 2:
                            attackRange = firstAttackRange;
                            FindObjectOfType<AudioManager>().Play("iceSword2");

                            slashOrder = 1;

                            attackAnim.SetTrigger("isAttacking");
                            attackAnim.SetInteger("attackIndex", slashOrder);

                            break;
                    }
                    break;
            }

            //Debug.Log("stop dashing");
            attackUI.padding = new Vector4(0, 0, 0, 113);
            canSpawnAttackfullBarVFX = true;

            timerAttack = attackCooldown;
        }

        if (controls.currentActionMap.FindAction("Attack").triggered && timerAttack > 0)
        {
            StartCoroutine(CannotPlaceWallFeedback(attackUI));
        }
        #endregion

        #region attack for sprites and ice bar and instance : with wallCooldown
        timerWall -= Time.deltaTime;

        if (controls.currentActionMap.FindAction("Wall").triggered && timerWall <= 0 && (gameObject.GetComponent<PlayerControllerV2>().isGroundedL || gameObject.GetComponent<PlayerControllerV2>().isGroundedR) && gameObject.GetComponent<Rigidbody2D>().velocity.y == 0 && !gameObject.GetComponent<PlayerControllerV2>().isDashUIStarted && !IsSpiking())
        {
            switch (playerSprite.flipX)
            {
                case true:
                    Debug.DrawRay(new Vector3(transform.position.x - 4, transform.position.y - 1f, transform.position.z), Vector2.down * 1f, Color.blue, 1);
                    RaycastHit2D[] colGrounded = Physics2D.RaycastAll(new Vector3(transform.position.x - 4, transform.position.y - 1f, transform.position.z), Vector2.down, 1f);

                    //Debug.Log(colGrounded.Length);

                    for (int i = 0; i < colGrounded.Length; i++)
                    {
                        if (colGrounded[i].collider.gameObject.layer == 6 || colGrounded[i].collider.gameObject.layer == 9)
                        {
                            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector2.left * 4f, Color.blue, 1);
                            RaycastHit2D colWallFacing = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector2.left, 4f, mask);

                            //Debug.Log("canLeft");
                            //Debug.Log("colGrounded " + colGrounded[i].collider.name);

                            if (!colWallFacing)
                            {
                                hasAttacked = true;
                                timerSheathe = 0.2f;
                                attackAnim.Play("Yuki_1st_Attack_iceWall");

                                isWalling = true;

                                for (int b = 0; b < 3; b++)
                                {
                                    CreateBullet();
                                }
                                //Debug.Log("ice wall");

                                //Debug.Log(colGrounded[i].collider.name);

                                wallEffect2.SetActive(true);
                                wallEffect2.GetComponent<Animator>().SetBool("Build", true);
                                GameObject instance = Instantiate(wall, new Vector3(transform.position.x - 3, transform.position.y + 0.1f, transform.position.z), Quaternion.identity);
                                instance.transform.rotation = Quaternion.Euler(0, 180, 0);


                                float random = Random.value;
                                if (random <= 0.5f)
                                {
                                    FindObjectOfType<AudioManager>().Play("iceWall");
                                }
                                else if (random > 0.5f)
                                {
                                    FindObjectOfType<AudioManager>().Play("iceWall2");
                                }

                                StartCoroutine(StopWallAnim());
                                wallUI.padding = new Vector4(0, 0, 0, 99);
                                canSpawnWallfullBarVFX = true;

                                timerWall = wallCooldown;
                                break;
                            }
                            else
                            {
                                Debug.Log("can't be placed!");
                                StartCoroutine(CannotPlaceWallFeedback(wallUI));
                            }
                        }
                    }

                    if (colGrounded.Length == 0)
                    {
                        Debug.Log("can't be placed!");
                        StartCoroutine(CannotPlaceWallFeedback(wallUI));
                    }

                    break;
                case false:
                    Debug.DrawRay(new Vector3(transform.position.x + 4, transform.position.y - 1f, transform.position.z), Vector2.down * 1f, Color.blue, 1);
                    RaycastHit2D[] colGrounded2 = Physics2D.RaycastAll(new Vector3(transform.position.x + 4, transform.position.y - 1f, transform.position.z), Vector2.down, 1f);

                    //Debug.Log(colGrounded2.Length);

                    for (int i = 0; i < colGrounded2.Length; i++)
                    {
                        if (colGrounded2[i].collider.gameObject.layer == 6 || colGrounded2[i].collider.gameObject.layer == 9)
                        {
                            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector2.right * 4f, Color.blue, 1);
                            RaycastHit2D colWallFacing2 = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector2.right, 4f, mask);

                            //Debug.Log("canRight");
                            //Debug.Log("colGrounded2 " + colGrounded2[i].collider.name);

                            if (!colWallFacing2)
                            {
                                hasAttacked = true;
                                timerSheathe = 0.2f;
                                attackAnim.Play("Yuki_1st_Attack_iceWall");

                                isWalling = true;

                                for (int b = 0; b < 3; b++)
                                {
                                    CreateBullet();
                                }
                                //Debug.Log("ice wall");

                                //Debug.Log(colGrounded2[i].collider.name);

                                wallEffect.SetActive(true);
                                wallEffect.GetComponent<Animator>().SetBool("Build", true);
                                GameObject instance2 = Instantiate(wall, new Vector3(transform.position.x + 3, transform.position.y + 0.1f, transform.position.z), Quaternion.identity);
                                instance2.transform.rotation = Quaternion.Euler(0, 0, 0);

                                float random2 = Random.value;
                                if (random2 <= 0.5f)
                                {
                                    FindObjectOfType<AudioManager>().Play("iceWall");
                                }
                                else if (random2 > 0.5f)
                                {
                                    FindObjectOfType<AudioManager>().Play("iceWall2");
                                }

                                StartCoroutine(StopWallAnim());
                                wallUI.padding = new Vector4(0, 0, 0, 99);
                                canSpawnWallfullBarVFX = true;

                                timerWall = wallCooldown;
                                break;
                            }
                            else
                            {
                                Debug.Log("can't be placed!");
                                StartCoroutine(CannotPlaceWallFeedback(wallUI));
                            }
                        }
                    }

                    if (colGrounded2.Length == 0)
                    {
                        Debug.Log("can't be placed!");
                        StartCoroutine(CannotPlaceWallFeedback(wallUI));
                    }

                    break;
            }
        }

        if (isWalling)
        {
            Invoke("StopWalling", 0.25f);
        }

        if (controls.currentActionMap.FindAction("Wall").triggered && timerWall > 0)
        {
            StartCoroutine(CannotPlaceWallFeedback(wallUI));
        }
        #endregion

        #region attack for sprites and ice bar and instance : with spikeCooldown
        timerSpike -= Time.deltaTime;
        //Debug.Log(spikeUI.padding.w);

        if (controls.currentActionMap.FindAction("Spike").triggered && timerSpike <= 0 && !gameObject.GetComponent<PlayerControllerV2>().isDashUIStarted && !IsWalling())
        {
            isSpiking = true;

            for (int i = 0; i < 3; i++)
            {
                CreateBullet();
            }
            //Debug.Log("ice spike");

            switch (playerSprite.flipX)
            {
                case true:
                    curSpikeSpeed = 0;
                    instance = Instantiate(spike, new Vector2(transform.position.x - 2, transform.position.y - 0f), Quaternion.identity);
                    instance.GetComponent<SpriteRenderer>().flipX = true;
                    Destroy(instance, 1);

                    float random = Random.value;
                    if (random <= 0.5f)
                    {
                        FindObjectOfType<AudioManager>().Play("iceSpike");
                    }
                    else if (random > 0.5f)
                    {
                        FindObjectOfType<AudioManager>().Play("iceSpike2");
                    }

                    break;
                case false:
                    curSpikeSpeed = 0;
                    instance2 = Instantiate(spike, new Vector2(transform.position.x + 2, transform.position.y - 0f), Quaternion.identity);
                    instance2.GetComponent<SpriteRenderer>().flipX = false;
                    Destroy(instance2, 1);

                    float random2 = Random.value;
                    if (random2 <= 0.5f)
                    {
                        FindObjectOfType<AudioManager>().Play("iceSpike");
                    }
                    else if (random2 > 0.5f)
                    {
                        FindObjectOfType<AudioManager>().Play("iceSpike2");
                    }

                    break;
            }

            //StartCoroutine(StopWallAnim());
            spikeUI.padding = new Vector4(0, 0, 0, 106);
            canSpawnSpikefullBarVFX = true;
            timerSpike = spikeCooldown;
        }

        curSpikeSpeed += spikeSpeedCurve.Evaluate(Time.deltaTime * spikeSpeed);
        //Debug.Log(curSpikeSpeed);

        if (isSpiking)
        {
            Invoke("StopSpiking", 0.25f);
        }

        if (controls.currentActionMap.FindAction("Spike").triggered && timerSpike > 0)
        {
            StartCoroutine(CannotPlaceWallFeedback(spikeUI));
        }

        #endregion

        #region attack for damages or effects : with damageCooldown
        Collider2D[] col = Physics2D.OverlapCircleAll(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);

        //Debug.Log(col.Length);

        for (int i = 0; i < col.Length; i++)
        {
            // Debug.Log(col[i].gameObject.name);
            //Debug.Log(timerDamage);

            if (col[i].gameObject.CompareTag("Enemy"))
            {

                if (controls.currentActionMap.FindAction("Attack").triggered)
                {
                    AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

                    for (int a = 0; a < audioS.Length; a++)
                    {
                        if (audioS[a].clip.name == "ice-sword")
                        {
                            audioS[a].Stop();
                            FindObjectOfType<AudioManager>().Play("iceSwordHit");
                            StartCoroutine(ShakeCamera(1f, 0.25f, 0.2f));
                        }

                        if (audioS[a].clip.name == "ice-sword2")
                        {
                            audioS[a].Stop();
                            FindObjectOfType<AudioManager>().Play("iceSwordHit2");
                            StartCoroutine(ShakeCamera(1f, 0.5f, 0.3f));
                        }
                    }

                    if (col[i].gameObject.GetComponent<EnemyHealth2>() != null)
                    {
                        col[i].gameObject.GetComponent<EnemyHealth2>().TakeDamage(damage, "sword");
                    }
                    else
                    {
                        col[i].gameObject.GetComponent<EnemyHealth>().TakeDamage(damage, "sword");
                    }
                }
            }

            if (col[i].gameObject.CompareTag("Boss"))
            {
                int j = Random.Range(0, 300);

                switch (playerSprite.flipX)
                {
                    case false:
                        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 2.5f);

                        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.right * 2.5f, Color.red);

                        if (controls.currentActionMap.FindAction("Attack").triggered && hitRight)
                        {
                            if (hitRight.transform.gameObject.GetComponent<BossHealth>())
                            {
                                col[i].gameObject.GetComponent<BossHealth>().TakeDamage(damage);
                            }
                        }

                        break;
                    case true:
                        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.right, 2.5f);

                        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), -Vector2.right * 2.5f, Color.red);

                        if (controls.currentActionMap.FindAction("Attack").triggered && hitLeft)
                        {
                            if (hitLeft.transform.gameObject.GetComponent<BossHealth>())
                            {
                                col[i].gameObject.GetComponent<BossHealth>().TakeDamage(damage);
                            }
                        }
                        break;
                }
            }

            if (col[i].gameObject.CompareTag("Door") && controls.currentActionMap.FindAction("Attack").triggered)
            {
                col[i].gameObject.GetComponent<Door>().DestroyDoor();
                StartCoroutine(ShakeCamera(1f, 0.25f, 0.15f));
            }

            if (col[i].gameObject.CompareTag("FuzeBox") && controls.currentActionMap.FindAction("Attack").triggered)
            {
                col[i].gameObject.GetComponent<FuzeBox>().DestoyFuze();
            }

            if (col[i].gameObject.GetComponent<IceWall>() && controls.currentActionMap.FindAction("Attack").triggered)
            {
                col[i].gameObject.GetComponent<IceWall>().Fall();
            }

            if (col[i].gameObject.CompareTag("Shield") && controls.currentActionMap.FindAction("Attack").triggered)
            {
                //Debug.Log("play block sound");
                AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

                for (int a = 0; a < audioS.Length; a++)
                {
                    if (audioS[a].clip.name == "ice-sword")
                    {
                        audioS[a].Stop();
                    }

                    if (audioS[a].clip.name == "ice-sword2")
                    {
                        audioS[a].Stop();
                    }

                    if (audioS[a].clip.name == "ice-sword-damage")
                    {
                        audioS[a].Stop();
                    }

                    if (audioS[a].clip.name == "ice-sword-damage2")
                    {
                        audioS[a].Stop();
                    }

                    if (audioS[a].clip.name == "ice-sword-block")
                    {
                        audioS[a].Stop();
                    }

                    if (audioS[a].clip.name == "ice-sword-block2")
                    {
                        audioS[a].Stop();
                    }
                }

                float random = Random.value;
                if (random <= 0.5f)
                {
                    FindObjectOfType<AudioManager>().Play("iceSwordBlock");
                }
                else if (random > 0.5f)
                {
                    FindObjectOfType<AudioManager>().Play("iceSwordBlock2");
                }
            }
        }
        #endregion

        #region modifications en fonction de la direction du joueur
        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if (xAxis > 0)
        {
            attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);

            wallEffect2.GetComponent<Animator>().SetBool("Build", false);

            wallEffect2.SetActive(false);
        }
        if (xAxis < 0)
        {
            attackPos.localPosition = new Vector3(-attackPosDistance.x, attackPosDistance.y, attackPos.position.z);

            wallEffect.SetActive(false);
            wallEffect.GetComponent<Animator>().SetBool("Build", false);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        #region pics de glace

        // lerp
        float curseur = 1;
        if (instance != null)
        {
            //Debug.Log(gameObject.GetComponent<PlayerControllerV2>().curVelocitySpeed);
            spikeLerp = Vector2.Lerp(spikeLerp, new Vector2(curSpikeSpeed - (controls.currentActionMap.FindAction("Move").ReadValue<float>() * 20 - gameObject.GetComponent<PlayerControllerV2>().dashTime * 150), spikeLerp.y) * -instance.transform.right, curseur);
            Vector2 nextPos = new Vector2(instance.transform.position.x + spikeLerp.x * Time.deltaTime, instance.transform.position.y);
            instance.transform.position = nextPos;
        }

        if (instance2 != null)
        {
            spikeLerp = Vector2.Lerp(spikeLerp, new Vector2(curSpikeSpeed + (controls.currentActionMap.FindAction("Move").ReadValue<float>() * 20 + gameObject.GetComponent<PlayerControllerV2>().dashTime * 150), spikeLerp.y) * instance2.transform.right, curseur);
            Vector2 nextPos = new Vector2(instance2.transform.position.x + spikeLerp.x * Time.deltaTime, instance2.transform.position.y);
            instance2.transform.position = nextPos;
        }

        // UI sliding
        spikeUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(spikeUI.padding.w - spikeCooldown * 2.5f, 4, 106));
        if (spikeUI.padding.w <= 15 && spikeUI.padding.w >= 13)
        {
            spikeUI.GetComponentInChildren<Animator>().SetTrigger("glitch");
        }

        if (spikeUI.padding.w <= 4 && canSpawnSpikefullBarVFX)
        {
            spikeFullBarVFX_instance = Instantiate(fullBarVFX, new Vector3(spikeUI.transform.position.x, spikeUI.transform.position.y + 0.5f, attackUI.transform.position.z), Quaternion.Euler(-90, 0, 0));
            spikeFullBarVFX_instance.transform.SetParent(spikeUI.transform, false);
            Destroy(spikeFullBarVFX_instance, 0.5f);
            canSpawnSpikefullBarVFX = false;
        }

        if (spikeFullBarVFX_instance != null)
        {
            spikeFullBarVFX_instance.transform.position = new Vector3(spikeUI.transform.position.x, spikeUI.transform.position.y + 0.5f, attackUI.transform.position.z);
        }
        #endregion

        #region mur de glace
        // UI sliding
        float w = wallUI.padding.w;
        w -= wallCooldown / 1.85f;
        wallUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(w, 0, 99));
        if (w <= 10 && w >= 9)
        {
            wallUI.GetComponentInChildren<Animator>().SetTrigger("glitch");
        }

        if (wallUI.padding.w <= 0 && canSpawnWallfullBarVFX)
        {
            wallFullBarVFX_instance = Instantiate(fullBarVFX, new Vector3(wallUI.transform.position.x, wallUI.transform.position.y + 0.5f, attackUI.transform.position.z), Quaternion.Euler(-90, 0, 0));
            wallFullBarVFX_instance.transform.SetParent(wallUI.transform, false);
            Destroy(wallFullBarVFX_instance, 0.5f);
            canSpawnWallfullBarVFX = false;
        }


        if (wallFullBarVFX_instance != null)
        {
            wallFullBarVFX_instance.transform.position = new Vector3(wallUI.transform.position.x, wallUI.transform.position.y + 0.5f, attackUI.transform.position.z);
        }
        #endregion

        #region attack
        // UI sliding
        float a_w = attackUI.padding.w;
        a_w -= 4f;
        attackUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(a_w, 3, 113));
        if (a_w <= 10 && a_w >= 6)
        {
            attackUI.GetComponentInChildren<Animator>().SetTrigger("glitch");
        }

        if (attackUI.padding.w <= 3 && canSpawnAttackfullBarVFX)
        {
            jumpFullBarVFX_instance = Instantiate(fullBarVFX, new Vector3(attackUI.transform.position.x, attackUI.transform.position.y + 0.5f, attackUI.transform.position.z), Quaternion.Euler(-90, 0, 0));
            jumpFullBarVFX_instance.transform.SetParent(attackUI.transform, false);
            Destroy(jumpFullBarVFX_instance, 0.5f);
            canSpawnAttackfullBarVFX = false;
        }

        if (jumpFullBarVFX_instance != null)
        {
            jumpFullBarVFX_instance.transform.position = new Vector3(attackUI.transform.position.x, attackUI.transform.position.y + 0.5f, attackUI.transform.position.z);
        }
        #endregion
    }

    /// <summary>
    /// feedback visuel de debug pour le rayon d'attaque
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);
    }

    /// <summary>
    /// permet d'arr�ter l'animation de ice wall
    /// </summary>
    /// <returns></returns>
    IEnumerator StopWallAnim()
    {
        yield return new WaitForSeconds(0.2857143f);

        switch (playerSprite.flipX)
        {
            case true:
                wallEffect2.SetActive(false);
                wallEffect2.GetComponent<Animator>().SetBool("Build", false);
                break;
            case false:
                wallEffect.SetActive(false);
                wallEffect.GetComponent<Animator>().SetBool("Build", false);
                break;
        }

        yield break;
    }

    /// <summary>
    /// Return true if player is walling and false if not
    /// </summary>
    /// <returns></returns>
    public bool IsWalling()
    {
        if (isWalling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Return true if player is spiking and false if not
    /// </summary>
    /// <returns></returns>
    public bool IsSpiking()
    {
        if (isSpiking)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// stop la boolean walling
    /// </summary>
    void StopWalling()
    {
        isWalling = false;
    }

    /// <summary>
    /// stop la boolean Spiking
    /// </summary>
    void StopSpiking()
    {
        isSpiking = false;
    }

    void SheatheSword()
    {
        attackAnim.SetTrigger("stopAttack");
        hasAttacked = false;
        timerSheathe = 0.2f;
    }

    void CreateBullet()
    {
        GameObject bullet;
        bullet = Instantiate(bulletIce, transform.position, transform.rotation);
        bullet.GetComponent<IceParticle>().iceToAdd = iceToAdd;
        bullet.GetComponent<IceParticle>().player = gameObject;
    }

    /// <summary>
    /// red color feedback on input UI
    /// </summary>
    /// <param name="UI"></param>
    /// <returns></returns>
    IEnumerator CannotPlaceWallFeedback(RectMask2D UI)
    {
        UI.transform.GetChild(0).GetComponent<Image>().color = Color.red;

        yield return new WaitForSeconds(0.25f);

        UI.transform.GetChild(0).GetComponent<Image>().color = Color.white;

        yield break;
    }

    public IEnumerator ShakeCamera(float intensity, float frequency, float time)
    {
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        bcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
        bcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;

        yield return new WaitForSeconds(time);

        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        bcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        bcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;

        yield break;
    }

    void StopAttack()
    {
        isAttacking = false;
    }
}
