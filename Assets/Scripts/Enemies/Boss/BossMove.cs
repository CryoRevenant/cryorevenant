using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    public bool isGrounded;

    [Header("Déplacement")]
    public float speed;
    public float speedFall;
    public bool canMove = true;
    public bool lookLeft;
    public float maxStoppingDist;

    [Header("Dash")]
    public float speedDash;
    public float distDash;
    public bool isDashing;

    [Header("Raycasts")]
    public float rayLengthDown;
    public float rayLengthSides;
    public float bounds;

    [Header("Debug")]
    public bool showRays;

    public int bossPhase;
    [HideInInspector] public int slowness=0;
    [HideInInspector] public float attackTimer=0;
    private bool isPlayerClose;
    [HideInInspector] public bool isStopped;
    [HideInInspector]public bool isSlowAttacking;

    // Start is called before the first frame update
    void Start()
    {
        //bossPhase = 1;
        isDashing = false;
        isPlayerClose = false;
        isStopped = false;
        isSlowAttacking = false;
    }

    // Update is called once per frame
    public void Update()
    {
        //Bool pour debug et ajuster les rays
        #region ShowRays
        if (showRays)
        {
            Debug.DrawRay(new Vector2(transform.position.x + bounds, transform.position.y), Vector2.down * rayLengthDown, Color.cyan, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x - bounds, transform.position.y), Vector2.down * rayLengthDown, Color.cyan, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left * rayLengthSides, Color.blue, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right * rayLengthSides, Color.blue, 0.2f);
        }
        #endregion

        //Rays dessous pour la gravité 
        #region RaysDown
        if (Physics2D.Raycast(new Vector2(transform.position.x + bounds, transform.position.y), Vector2.down, rayLengthDown))
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x - bounds, transform.position.y), Vector2.down, rayLengthDown))
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
        #endregion

        //Ray au dessus de sa tête pour voir si le joueur saute par dessus lui
        #region RayUp

        RaycastHit2D hitUp = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+1), Vector2.up, rayLengthSides);

        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y+1), Vector2.up*rayLengthSides,Color.red);

        if (hitUp && !isSlowAttacking)
        {
            if (hitUp.transform.CompareTag("Player"))
            {
                //Debug.Log("player jumping over");
                StartCoroutine(Dash(hitUp.transform.GetChild(0).gameObject));
                gameObject.GetComponent<BossAttack>().Attack();
            }
        }

        #endregion

        // gravity
        if (isGrounded == false)
        {
            Vector2 pos = transform.position;
            pos.y -= Time.deltaTime * speedFall;
            transform.position = pos;
        }

        #region ice wall boss react

        if(bossPhase == 1)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSides, 1 << 8);
            RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSides, 1 << 8);
            //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left * rayLengthSides, Color.red);
            //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right * rayLengthSides, Color.red);

            if (hitRight)
            {
                //Debug.Log(hitRight.transform.name);
                if (hitRight.transform.GetComponent<IceWall>())
                {
                    //Debug.Log("wall Detected");
                    gameObject.GetComponent<BossAttack>().canCheckAttack = false;
                    gameObject.GetComponent<BossAttack>().Reset();
                    StopMove();
                    Destroy(hitRight.transform.gameObject);
                }
            }
            else if (hitLeft)
            {
                //Debug.Log(hitRight.transform.name);
                if (hitLeft.transform.GetComponent<IceWall>())
                {
                    //Debug.Log("wall Detected");
                    gameObject.GetComponent<BossAttack>().canCheckAttack = false;
                    gameObject.GetComponent<BossAttack>().Reset();
                    StopMove();
                    Destroy(hitLeft.transform.gameObject);
                }
            }
        }

        if(bossPhase == 2)
        {
            //Debug.Log("Break Wall");
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSides, 1 << 8);
            RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSides, 1 << 8);
            //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left * rayLengthSides, Color.red);
            //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right * rayLengthSides, Color.red);

            if (hitRight)
            {
                //Debug.Log(hitRight.transform.name);
                if (hitRight.transform.GetComponent<IceWall>())
                {
                    //Debug.Log("wall Detected");
                    Destroy(hitRight.transform.gameObject);
                }
            }
            else if (hitLeft)
            {
                //Debug.Log(hitRight.transform.name);
                if (hitLeft.transform.GetComponent<IceWall>())
                {
                    //Debug.Log("wall Detected");
                    Destroy(hitLeft.transform.gameObject);
                }
            }
        }

        #endregion

        #region phase 2
        if (bossPhase == 2)
        {
            speed = 10;
            //Debug.Log("running");
            //Debug.Log("timer " + timer);
            if(slowness >= 3)
            {
                StopMove();
                //gameObject.GetComponent<BossAttack>().canCheckAttack = false;
                gameObject.GetComponent<BossAttack>().Reset();

                RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x-0.5f, transform.position.y - bounds+1), Vector2.left, 2.5f);
                RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x+0.5f, transform.position.y - bounds+1), Vector2.right, 2.5f);
                //Debug.DrawRay(new Vector2(transform.position.x - 0.5f, transform.position.y - bounds+1), Vector2.left * 2.5f, Color.red);
                //Debug.DrawRay(new Vector2(transform.position.x + 0.5f, transform.position.y - bounds+1), Vector2.right * 2.5f, Color.red);

                if (hitRight)
                {
                    //Debug.Log(hitRight.transform.name);
                    if (hitRight.transform.CompareTag("Player"))
                    {
                        //Debug.Log("player Detected");

                        //transform.rotation = Quaternion.Euler(0, 0, 0);

                        isPlayerClose = true;
                    }
                }
                else if (hitLeft)
                {
                    //Debug.Log(hitRight.transform.name);
                    if (hitLeft.transform.CompareTag("Player"))
                    {
                        //Debug.Log("player Detected");

                        //transform.rotation = Quaternion.Euler(0, 180, 0);

                        isPlayerClose = true;
                    }
                }

                if(hitRight || hitLeft)
                {
                    if (hitRight && !hitLeft)
                    {
                        if (!hitRight.transform.CompareTag("Player"))
                        {
                            //Debug.Log("no player close");
                            isPlayerClose = false;
                        }
                    }
                    else if (hitLeft && !hitRight)
                    {
                        if (!hitLeft.transform.CompareTag("Player"))
                        {
                            //Debug.Log("no player close");
                            isPlayerClose = false;
                        }
                    }
                }

                if(!hitRight && !hitLeft)
                {
                    //Debug.Log("no player close");
                    isPlayerClose = false;
                }

                //Debug.Log("isPlayerClose = " + isPlayerClose);

                if (!isSlowAttacking)
                {
                    //Debug.Log("breathing");
                    if (!isPlayerClose)
                    {
                        //Debug.Log("move");
                        isStopped = false;
                        StartCoroutine(StartMove(3));
                    }
                    else if (isPlayerClose)
                    {
                        attackTimer += Time.deltaTime;

                        if(attackTimer >= 1f)
                        {
                            if (transform.rotation == Quaternion.Euler(0, 0, 0) && hitRight)
                            {
                                if (hitRight.transform.CompareTag("Player"))
                                {
                                    //Debug.Log("not move right");
                                    isStopped = true;
                                    StopCoroutine(StartMove(0));
                                    StartCoroutine(gameObject.GetComponent<BossAttack>().StartSlowAttack());
                                }
                            }
                            else if (transform.rotation == Quaternion.Euler(0, 180, 0) && hitLeft)
                            {
                                if (hitLeft.transform.CompareTag("Player"))
                                {
                                    //Debug.Log("not move left");
                                    isStopped = true;
                                    StopCoroutine(StartMove(0));
                                    StartCoroutine(gameObject.GetComponent<BossAttack>().StartSlowAttack());
                                }
                            }
                            attackTimer = 0;
                        }
                    }
                }

                //Debug.Log("isSlowAttacking " + isSlowAttacking);
                //Debug.Log("canMove " + canMove);
                //Debug.Log("isSecondAttack " + transform.GetChild(0).GetComponent<Animator>().GetBool("isSecondAttack"));
            }
        }

        //Debug.Log(bossPhase);
        #endregion

        #region Collision Management

        RaycastHit2D damageLeft = Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y - bounds + 1), Vector2.left, 2.5f);
        RaycastHit2D damageRight = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y - bounds + 1), Vector2.right, 2.5f);
        Debug.DrawRay(new Vector2(transform.position.x - 0.5f, transform.position.y - bounds+1), Vector2.left * 2.5f, Color.yellow);
        Debug.DrawRay(new Vector2(transform.position.x + 0.5f, transform.position.y - bounds+1), Vector2.right * 2.5f, Color.yellow);

        if (damageRight || damageLeft)
        {
            if (transform.rotation == Quaternion.Euler(0, 180, 0) && damageRight)
            {
                if (damageRight.transform.CompareTag("Player"))
                {
                    //Debug.Log("can damage right");

                    gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
                }
            }
            else if (transform.rotation == Quaternion.Euler(0, 0, 0) && damageLeft)
            {
                if (damageLeft.transform.CompareTag("Player"))
                {
                    //Debug.Log("can damage left");

                    gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
                }
            }
        }

        if (!damageRight && !damageLeft)
        {
            //Debug.Log("no player close");

            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }

        #endregion
    }

    //Coroutine qui permet à l'ennemi de se déplacer dans la direction du joueur
    public IEnumerator MoveOver(GameObject lastPos)
    {
        if (canMove == true)
        {
            Vector3 posToGo = lastPos.transform.position;

            //Tant que l'ennemi doit bouger
            while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
            {
                //Debug.Log("playing?");
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToGo.x, transform.position.y, 0), Time.deltaTime * speed);

                //Le joueur est à gauche ?
                if (posToGo.x < transform.position.x)
                {
                    lookLeft = true;
                    LookDirection();
                }
                else
                {
                    lookLeft = false;
                    LookDirection();
                }

                //Si l'ennemi est assez proche, il s'arrête
                if (Vector3.Distance(transform.position, posToGo) < maxStoppingDist)
                {
                    Reset(lastPos);
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    //Reset de la position donnée dans la coroutine pour que l'ennemi s'arrête
    public void Reset(GameObject player)
    {
        player = null;
    }

    //Flip de l'ennemi pour qu'il regarde dans la direction du joueur
    public void LookDirection()
    {
        if (lookLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void StopMove()
    {
        canMove = false;
        StopCoroutine("MoveOver");
    }

    public IEnumerator StartMove(float waitTime)
    {
        //Debug.Log("Start move waitTime =" + waitTime);
        yield return new WaitForSeconds(waitTime);
        //Debug.Log("isStopped " + isStopped);
        //Debug.Log("isSlowAttacking " + isSlowAttacking);
        if (!isStopped && !isSlowAttacking)
        {
            //Debug.Log("Start move");
            canMove = true;
            transform.GetChild(0).GetComponent<BossHitTrigger>().StopSlowAttacking();
            slowness = 0;
        }
        yield break;
    }

    public IEnumerator Dash(GameObject player)
    {
        if (isDashing == false)
        {
            isDashing = true;
            StopMove();

            Vector3 newPos;

            if (player.GetComponent<SpriteRenderer>().flipX)
            {
                newPos = new Vector3(transform.position.x - distDash, transform.position.y, 0);
            }
            else
            {
                newPos = new Vector3(transform.position.x + distDash, transform.position.y, 0);
            }

            while (isDashing == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(newPos.x, transform.position.y, 0), Time.deltaTime * speedDash);

                if (Vector3.Distance(transform.position, newPos) < maxStoppingDist)
                {
                    isDashing = false;
                }
                yield return new WaitForSeconds(0.01f);
            }

            canMove = true;
        }
    }
}