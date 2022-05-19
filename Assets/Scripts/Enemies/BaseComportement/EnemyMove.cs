using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    bool isGrounded;

    [Header("Déplacement")]
    [SerializeField] float speed;
    [SerializeField] float speedFall;
    public bool canMove = true;
    public bool lookLeft;
    public float maxStoppingDist;

    [Header("Dash")]
    [SerializeField] float speedDash;
    public float distDash;
    public bool isDashing;

    [Header("Raycasts")]
    [SerializeField] float rayLengthDown;
    [SerializeField] float rayLengthSide;
    [SerializeField] float bounds;

    [Header("Debug")]
    [SerializeField] bool showRays;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Bool pour debug et ajuster les rays
        #region ShowRays
        if (showRays)
        {
            Debug.DrawRay(new Vector2(transform.position.x + bounds, transform.position.y), Vector2.down * rayLengthDown, Color.cyan, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x - bounds, transform.position.y), Vector2.down * rayLengthDown, Color.cyan, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left * rayLengthSide, Color.blue, 0.2f);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right * rayLengthSide, Color.blue, 0.2f);
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

        int layerMask = ~LayerMask.GetMask("Box");

        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSide, layerMask) || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSide, layerMask))
        {
            StopMove();
            Offset();
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

        if (isDashing)
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSide, layerMask2) || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSide, layerMask2))
            {
                Debug.Log("StopDash");
                GetComponent<CapsuleCollider2D>().isTrigger = false;
                StopCoroutine("Dash");
            }
        }
        #endregion
    }

    //Coroutine qui permet à l'ennemi de se déplacer dans la direction du joueur
    public virtual IEnumerator MoveOver(GameObject lastPos)
    {
        if (canMove == true)
        {
            Vector3 posToGo = lastPos.transform.position;

            //Tant que l'ennemi doit bouger
            while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
            {
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

    //Décalage de l'ennemi quand il touche un mur pour ne pas qu'il se bloque dedans
    void Offset()
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

    public IEnumerator Dash(int direction)
    {
        if (isDashing == false)
        {
            GetComponent<CapsuleCollider2D>().isTrigger = true;
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

            while (isDashing == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(newPos.x, transform.position.y, 0), Time.deltaTime * speedDash);

                if (Vector3.Distance(transform.position, newPos) < maxStoppingDist)
                {
                    isDashing = false;
                }
                yield return new WaitForSeconds(0.01f);
            }

            //Le joueur est à gauche ?
            if (newPos.x < transform.position.x)
            {
                lookLeft = true;
                LookDirection();
            }
            else
            {
                lookLeft = false;
                LookDirection();
            }
            GetComponent<CapsuleCollider2D>().isTrigger = false;
            canMove = true;
        }
    }
}

