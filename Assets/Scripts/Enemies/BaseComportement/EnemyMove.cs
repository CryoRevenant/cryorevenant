using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    bool canMove = true;
    bool isGrounded;
    public bool lookLeft;

    [Header("Déplacement")]
    [SerializeField] float speed;
    [SerializeField] float speedFall;
    public float maxStoppingDist;
    public bool mustGo;

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
            canMove = false;
            StopCoroutine("MoveOver");
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
    }

    //Coroutine qui permet à l'ennemi de se déplacer dans la direction du joueur
    IEnumerator MoveOver(GameObject lastPos)
    {
        if (canMove == true)
        {
            Vector3 posToGo = lastPos.transform.position;

            //Tant que l'ennemi doit bouger
            while (mustGo == true)
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
    void Reset(GameObject player)
    {
        player = null;
        mustGo = false;
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
    void LookDirection()
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
}