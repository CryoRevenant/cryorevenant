using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public bool isGrounded;

    [Header("Déplacement")]
    public float speed;
    public float speedFall;
    public bool canMove = true;
    public bool lookLeft;
    public float maxStoppingDist;
    public Animator anim;
    bool stopMove;

    [Header("Dash")]
    public float speedDash;
    public float distDash;
    public bool isDashing;
    [SerializeField] Sprite frontDash;
    [SerializeField] Sprite backDash;
    public bool travel;

    [Header("Raycasts")]
    public float rayLengthDown;
    public float rayLengthSide;
    public float rayCheckWall;
    public float rayLengthSideEnemy;
    public float bounds;

    [Header("Debug")]
    public bool showRays;

    [Header("Audio")]
    [SerializeField] AudioSource walkSFX;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Bool pour debug et ajuster les rays
        #region ShowRays
        if (showRays)
        {
            Debug.DrawRay(new Vector2(transform.position.x + bounds, transform.position.y), Vector2.down * rayLengthDown, Color.cyan, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x - bounds, transform.position.y), Vector2.down * rayLengthDown, Color.cyan, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left * rayLengthSide, Color.blue, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right * rayLengthSide, Color.blue, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left * rayLengthSideEnemy, Color.green, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right * rayLengthSideEnemy, Color.green, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left * rayCheckWall, Color.yellow, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right * rayCheckWall, Color.yellow, 0.2f);
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

        //Rays sur les côtés pour éviter que l'ennemi passe au travers des murs
        #region RaysSide

        TravelRays();
        BlockRays();

        int layerMask = LayerMask.GetMask("Box") + LayerMask.GetMask("Enemy");
        RaycastHit2D hit2Dt = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSide, ~layerMask);
        RaycastHit2D hit2Du = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSide, ~layerMask);
        if (hit2Dt || hit2Du)
        {
            if (!travel)
            {
                StopMove();
                Offset();
            }
        }
        else
        {
            canMove = true;
        }

        #endregion

        if (isGrounded == false)
        {
            Vector2 pos = transform.position;
            pos.y -= Time.deltaTime * speedFall;
            transform.position = pos;
        }

        #region DashRays
        int layerMask2 = ~LayerMask.GetMask("Default") + LayerMask.GetMask("Box");

        if (isDashing && !travel)
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSide, layerMask2) || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSide, layerMask2))
            {
                GetComponent<CapsuleCollider2D>().enabled = true;
                StopCoroutine("Dash");
            }
        }
        #endregion
    }

    void BlockRays()
    {
        RaycastHit2D hit2DL = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayCheckWall, 1 << 6);
        RaycastHit2D hit2DR = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayCheckWall, 1 << 6);

        if (hit2DL || hit2DR)
        {
            GetComponent<EnemyHealth2>().canRecoil = false;
            Offset();
            GetComponent<EnemyHealth2>().StopCoroutine("RecoilHit");
        }
    }

    void TravelRays()
    {
        RaycastHit2D hit2DL = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSide);
        RaycastHit2D hit2DR = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSide);

        if (hit2DL.collider != null)
        {
            if (hit2DL.collider.transform.gameObject.layer == 3 && GetComponent<EnemyAttack>().isPlayerNear == false)
            {
                travel = true;
                StartCoroutine("Dash", 0);
            }

            if (hit2DL.collider.transform.gameObject.layer == 0)
            {
                if (GetComponent<SoldatAttack>() != null)
                {
                    GetComponent<SoldatAttack>().CheckPlayer();
                }

                LookDirection(hit2DL.transform.position);

                if (GetComponent<SoldatAttack>() != null)
                    GetComponent<EnemyHealth2>().canRecoil = true;
            }
        }
        if (hit2DR.collider != null)
        {
            if (hit2DR.collider.transform.gameObject.layer == 3 && GetComponent<EnemyAttack>().isPlayerNear == false)
            {
                travel = true;
                StartCoroutine("Dash", 1);
            }
            if (hit2DR.collider.transform.gameObject.layer == 0)
            {
                if (GetComponent<SoldatAttack>() != null)
                {
                    GetComponent<SoldatAttack>().CheckPlayer();
                }

                LookDirection(hit2DR.transform.position);

                if (GetComponent<SoldatAttack>() != null)
                    GetComponent<EnemyHealth2>().canRecoil = true;
            }
        }
    }

    //Coroutine qui permet à l'ennemi de se déplacer dans la direction du joueur
    public virtual IEnumerator MoveOver(GameObject lastPos)
    {
        if (canMove && !stopMove)
        {
            //Debug.Log("walk");
            walkSFX.Play();
            Vector3 posToGo = lastPos.transform.position;

            //Tant que l'ennemi doit bouger
            while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToGo.x, transform.position.y, 0), Time.deltaTime * speed);
                anim.SetBool("isRunning", true);

                LookDirection(posToGo);

                //Si l'ennemi est assez proche, il s'arrête
                if (Vector3.Distance(transform.position, posToGo) < maxStoppingDist)
                {
                    Reset(lastPos);
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            //Debug.Log("don't walk");
            walkSFX.Stop();
        }
    }

    public void LockMove(bool state)
    {
        stopMove = state;

        if (state)
        {
            anim.SetBool("isRunning", false);
            StopCoroutine("MoveOver");
        }
    }

    //Reset de la position donnée dans la coroutine pour que l'ennemi s'arrête
    public void Reset(GameObject player)
    {
        player = null;
    }

    //Décalage de l'ennemi quand il touche un mur pour ne pas qu'il se bloque dedans
    public void Offset()
    {
        int layerMask = ~LayerMask.GetMask("Default");

        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSide, layerMask))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 0.1f, transform.position.y, 0), Time.deltaTime * speed);
        }

        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSide, layerMask))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 0.1f, transform.position.y, 0), Time.deltaTime * speed);
        }
    }

    //Flip de l'ennemi pour qu'il regarde dans la direction du joueur
    public void LookDirection(Vector3 newPos)
    {
        if (newPos.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            lookLeft = true;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            lookLeft = false;
        }
    }

    public void StopMove()
    {
        stopMove = true;
        StopCoroutine("MoveOver");
    }

    void DashDir(bool isLeft, int dir)
    {
        if ((isLeft && dir == 1) || (!isLeft && dir == 0))
        {
            anim.SetBool("backDash", true);
            GetComponent<SpriteRenderer>().sprite = backDash;
        }
        else
        {
            anim.SetBool("backDash", false);
            GetComponent<SpriteRenderer>().sprite = frontDash;
        }
        anim.SetBool("isDashing", true);
    }

    public IEnumerator Dash(int direction)
    {
        if (isDashing == false)
        {
            float random = Random.value;

            if (random > 0.5f)
            {
                FindObjectOfType<AudioManager>().Play("soldatDash");
            }
            else if (random <= 0.5f)
            {
                FindObjectOfType<AudioManager>().Play("soldatDash2");
            }

            GetComponent<CapsuleCollider2D>().enabled = false;
            isDashing = true;
            StopMove();

            int dir = direction;
            if (dir == 3)
            {
                dir = Random.Range(0, 2);
            }

            Vector3 newPos;

            if (dir == 0)
            {
                newPos = new Vector3(transform.position.x - distDash, transform.position.y, 0);
            }
            else
            {
                newPos = new Vector3(transform.position.x + distDash, transform.position.y, 0);
            }


            if (newPos.x < transform.position.x)
            {
                DashDir(true, dir);
            }
            else
            {
                DashDir(false, dir);
            }
            float i = 0;

            while (isDashing == true)
            {
                i += 0.01f;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(newPos.x, transform.position.y, 0), Time.deltaTime * speedDash);

                if (Vector3.Distance(transform.position, newPos) < maxStoppingDist || i >= 3f)
                {
                    GetComponent<CapsuleCollider2D>().enabled = true;
                    isDashing = false;
                    anim.SetBool("isDashing", false);
                }
                yield return new WaitForSeconds(0.01f);
            }

            float j = transform.rotation.y;
            transform.rotation = Quaternion.Euler(0, j + 180, 0);

            canMove = true;
            travel = false;
        }
    }
}

