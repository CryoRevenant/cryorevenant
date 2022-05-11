using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacMove : EnemyMove
{
    [SerializeField] float maxAttackDist;

    [SerializeField] float overrideSpeed;
    [SerializeField] float baseSpeed;
    float speedMove;

    bool canCall = true;

    SacAttak attak;

    private void Start()
    {
        attak = GetComponent<SacAttak>();
        speedMove = baseSpeed;
    }

    public override IEnumerator MoveOver(GameObject lastPos)
    {
        if (canMove == true && canCall)
        {
            canCall = false;
            Vector3 posToGo = lastPos.transform.position;

            //Tant que l'ennemi doit bouger
            while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToGo.x, transform.position.y, 0), Time.deltaTime * speedMove);

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

                if (Vector3.Distance(transform.position, posToGo) < maxAttackDist)
                {
                    StartCoroutine("AttackMove", lastPos);
                    StopCoroutine("MoveOver");
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator AttackMove(GameObject lastPos)
    {
        Vector3 posToGo = lastPos.transform.position;

        speedMove = overrideSpeed;

        attak.Attack();

        //Tant que l'ennemi doit bouger
        while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToGo.x, transform.position.y, 0), Time.deltaTime * speedMove);
            yield return new WaitForSeconds(0.01f);
        }
        canCall = true;
        speedMove = baseSpeed;
        attak.StopAttack();
    }
}
