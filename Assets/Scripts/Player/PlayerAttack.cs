using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Transform attackPos;
    [SerializeField] private Vector2 attackPosDistance;
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;

    [Header("IceBar")]
    [SerializeField] private int iceToAdd;
    [Header("Slash")]
    [SerializeField] private RectMask2D attackUI;
    [SerializeField] private GameObject slashEffect;
    [SerializeField] private GameObject slashEffect2;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float attackCooldown;
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
    private float timerDamage;
    private float timerAttack;
    private float timerSpike;
    private float timerWall;
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

    private void Awake()
    {
        #region cooldowns
        timerDamage = damageCooldown;
        timerAttack = attackCooldown;
        timerWall = wallCooldown;
        timerSpike = spikeCooldown;
        wallUI.padding = new Vector4(0, 0, 0, 99);
        canSpawnWallfullBarVFX = true;
        spikeUI.padding = new Vector4(0, 0, 0, 106);
        canSpawnSpikefullBarVFX = true;
        attackUI.padding = new Vector4(0, 0, 0, 113);
        canSpawnAttackfullBarVFX = true;
        #endregion

        #region get
        controls = gameObject.GetComponent<PlayerInput>();
        #endregion

        attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);
        slashOrder = 2;

        #region slash set
        slashEffect2.SetActive(false);
        slashEffect2.GetComponent<Animator>().SetBool("Recover", false);
        slashEffect.SetActive(false);
        slashEffect.GetComponent<Animator>().SetBool("Recover", false);
        #endregion

        #region wall set
        wallEffect2.SetActive(false);
        wallEffect2.GetComponent<Animator>().SetBool("Recover", false);
        wallEffect.SetActive(false);
        wallEffect.GetComponent<Animator>().SetBool("Recover", false);
        #endregion
    }

    private void Update()
    {
        #region attack for sprites and ice bar : with attackCooldown

        if (playerSprite.flipX != lastFlip)
        {
            //Debug.Log("reset slash anim");
            slashOrder = 2;
        }

        timerAttack -= Time.deltaTime;
        if (controls.currentActionMap.FindAction("Attack").triggered && timerAttack <= 0)
        {
            switch (playerSprite.flipX)
            {
                case true:
                    lastFlip = playerSprite.flipX;
                    slashEffect2.SetActive(true);
                    switch (slashOrder)
                    {
                        case 1:
                            slashOrder = 2;
                            slashEffect2.GetComponent<Animator>().Play("SlashAttack_02");
                            break;
                        case 2:
                            slashOrder = 1;
                            slashEffect2.GetComponent<Animator>().Play("SlashAttack_01");
                            break;
                    }
                    slashEffect2.GetComponent<Animator>().SetBool("Recover", true);
                    break;
                case false:
                    lastFlip = playerSprite.flipX;
                    slashEffect.SetActive(true);
                    switch (slashOrder)
                    {
                        case 1:
                            slashOrder = 2;
                            slashEffect.GetComponent<Animator>().Play("SlashAttack_02");
                            break;
                        case 2:
                            slashOrder = 1;
                            slashEffect.GetComponent<Animator>().Play("SlashAttack_01");
                            break;
                    }
                    slashEffect.GetComponent<Animator>().SetBool("Recover", true);
                    break;
            }
            StartCoroutine(StopSlashAnim());

            GetComponent<IceBar>().AddBar(iceToAdd);
            //Debug.Log("stop dashing");
            attackUI.padding = new Vector4(0, 0, 0, 113);
            canSpawnAttackfullBarVFX = true;

            timerAttack = attackCooldown;
        }
        #endregion

        #region attack for sprites and ice bar and instance : with wallCooldown
        timerWall -= Time.deltaTime;
        //Debug.Log(timerWall);
        //Debug.Log(!gameObject.GetComponent<PlayerControllerV2>().isDashUIStarted);
        //Debug.Log("isSpiking" + IsSpiking());
        //Debug.Log("isWalling" + IsWalling());

        if (controls.currentActionMap.FindAction("Wall").triggered && timerWall <= 0 && (gameObject.GetComponent<PlayerControllerV2>().isGroundedL || gameObject.GetComponent<PlayerControllerV2>().isGroundedR) && gameObject.GetComponent<Rigidbody2D>().velocity.y==0 && !gameObject.GetComponent<PlayerControllerV2>().isDashUIStarted && !IsSpiking())
        {
            isWalling = true;
            //Debug.Log("ice wall");

            switch (playerSprite.flipX)
            {
                case true:
                    wallEffect2.SetActive(true);
                    wallEffect2.GetComponent<Animator>().SetBool("Build",true);
                    GameObject instance = Instantiate(wall, new Vector3(transform.position.x - 3, transform.position.y - 0.3f, transform.position.z), Quaternion.identity);
                    instance.GetComponent<SpriteRenderer>().flipX = true;
                    break;
                case false:
                    wallEffect.SetActive(true);
                    wallEffect.GetComponent<Animator>().SetBool("Build",true);
                    GameObject instance2 = Instantiate(wall, new Vector3(transform.position.x + 3, transform.position.y - 0.3f, transform.position.z), Quaternion.identity);
                    instance2.GetComponent<SpriteRenderer>().flipX = false;
                    break;
            }

            StartCoroutine(StopWallAnim());
            GetComponent<IceBar>().AddBar(iceToAdd);
            wallUI.padding = new Vector4(0, 0, 0, 99);
            canSpawnWallfullBarVFX = true;

            timerWall = wallCooldown;
        }

        if (isWalling)
        {
            Invoke("StopWalling",0.25f);
        }
        #endregion

        #region attack for sprites and ice bar and instance : with spikeCooldown
        timerSpike -= Time.deltaTime;
        //Debug.Log(spikeUI.padding.w);

        if (controls.currentActionMap.FindAction("Spike").triggered && timerSpike <= 0 && !gameObject.GetComponent<PlayerControllerV2>().isDashUIStarted && !IsWalling())
        {
            isSpiking = true;
            //Debug.Log("ice spike");

            switch (playerSprite.flipX)
            {
                case true:
                    curSpikeSpeed = 0;
                    instance = Instantiate(spike, new Vector2(transform.position.x - 2, transform.position.y - 0f), Quaternion.identity);
                    instance.GetComponent<SpriteRenderer>().flipX = true;
                    Destroy(instance, 1);
                    break;
                case false:
                    curSpikeSpeed = 0;
                    instance2 = Instantiate(spike, new Vector2(transform.position.x + 2, transform.position.y - 0f), Quaternion.identity);
                    instance2.GetComponent<SpriteRenderer>().flipX = false;
                    Destroy(instance2, 1);
                    break;
            }

            //StartCoroutine(StopWallAnim());
            GetComponent<IceBar>().AddBar(iceToAdd);
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

        #endregion

        #region attack for damages or effects : with damageCooldown
        Collider2D[] col = Physics2D.OverlapCircleAll(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);

        //Debug.Log(col.Length);

        for (int i = 0; i < col.Length; i++)
        {
            // Debug.Log(col[i].gameObject.name);
            timerDamage -= Time.deltaTime;
            if (col[i].gameObject.CompareTag("Enemy"))
            {
                int j = Random.Range(0, 300);

                // if (j == 1)
                // {
                //     Debug.Log("dash");
                //     col[i].gameObject.GetComponent<EnemyMove>().StartCoroutine("Dash", 3);
                // }
                if (controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
                {
                    col[i].gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
                    timerDamage = damageCooldown;
                }
            }

            if (col[i].gameObject.CompareTag("Boss"))
            {
                int j = Random.Range(0, 300);

                // if (j == 1)
                // {
                //     Debug.Log("dash");
                //     col[i].gameObject.GetComponent<EnemyMove>().StartCoroutine("Dash", 3);
                // }
                if (controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
                {
                    col[i].gameObject.GetComponent<BossHealth>().TakeDamage(damage);
                    timerDamage = damageCooldown;
                }
            }

            if (col[i].gameObject.CompareTag("Door") && controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
            {
                col[i].gameObject.GetComponent<Door>().DestroyDoor();
                timerDamage = damageCooldown;
            }
            if (col[i].gameObject.CompareTag("FuzeBox") && controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
            {
                col[i].gameObject.GetComponent<FuzeBox>().DestoyFuze();
                timerDamage = damageCooldown;
            }
            if (col[i].gameObject.GetComponent<IceWall>() && controls.currentActionMap.FindAction("Attack").triggered && timerDamage <= 0)
            {
                Destroy(col[i].gameObject);
                timerDamage = damageCooldown;
            }
        }
        #endregion

        #region modifications en fonction de la direction du joueur
        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if (xAxis > 0)
        {
            attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);

            slashEffect2.SetActive(false);
            slashEffect2.GetComponent<Animator>().SetBool("Recover", false);
            wallEffect2.GetComponent<Animator>().SetBool("Build", false);

            wallEffect2.SetActive(false);
        }
        if (xAxis < 0)
        {
            attackPos.localPosition = new Vector3(-attackPosDistance.x, attackPosDistance.y, attackPos.position.z);

            slashEffect.SetActive(false);
            slashEffect.GetComponent<Animator>().SetBool("Recover", false);
            
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
            spikeLerp = Vector2.Lerp(spikeLerp, new Vector2(curSpikeSpeed - (controls.currentActionMap.FindAction("Move").ReadValue<float>()*20 - gameObject.GetComponent<PlayerControllerV2>().dashTime * 150), spikeLerp.y) * -instance.transform.right, curseur);
            Vector2 nextPos = new Vector2(instance.transform.position.x + spikeLerp.x * Time.deltaTime, instance.transform.position.y);
            instance.transform.position = nextPos;
        }

        if (instance2 != null)
        {
            spikeLerp = Vector2.Lerp(spikeLerp, new Vector2(curSpikeSpeed + (controls.currentActionMap.FindAction("Move").ReadValue<float>()* 20 + gameObject.GetComponent<PlayerControllerV2>().dashTime * 150), spikeLerp.y) * instance2.transform.right, curseur);
            Vector2 nextPos = new Vector2(instance2.transform.position.x + spikeLerp.x * Time.deltaTime, instance2.transform.position.y);
            instance2.transform.position = nextPos;
        }

        // UI sliding
        spikeUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(spikeUI.padding.w - spikeCooldown*2, 4, 106));

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
        w -= wallCooldown/2.05f;
        wallUI.padding = new Vector4(0, 0, 0, Mathf.Clamp(w, 0, 99));

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
        
        if(attackUI.padding.w <= 3 && canSpawnAttackfullBarVFX)
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
    /// permet d'arr�ter l'animation de slash
    /// </summary>
    /// <returns></returns>
    IEnumerator StopSlashAnim()
    {
        yield return new WaitForSeconds(0.4f);

        switch (playerSprite.flipX)
        {
            case true:
                slashEffect2.SetActive(false);
                slashEffect2.GetComponent<Animator>().SetBool("Recover", false);
                break;
            case false:
                slashEffect.SetActive(false);
                slashEffect.GetComponent<Animator>().SetBool("Recover", false);
                break;
        }

        yield break;
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
}
