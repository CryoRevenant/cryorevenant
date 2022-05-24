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

    // Start is called before the first frame update
    void Start()
    {
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

        if (hitUp)
        {
            if (hitUp.transform.CompareTag("Player"))
            {
                //Debug.Log("player jumping over");
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

        #region ice wall block boss

        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.left, rayLengthSides, 1<<8);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - bounds), Vector2.right, rayLengthSides, 1<<8);

        if (hitRight)
        {
            if (hitRight.transform.GetComponent<IceWall>())
            {
                //Debug.Log("wall Detected");
                StopMove();
            }
        }
        else if (hitLeft)
        {
            if (hitLeft.transform.GetComponent<IceWall>())
            {
                //Debug.Log("wall Detected");
                StopMove();
            }
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
}