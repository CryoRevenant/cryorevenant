using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacMove : EnemyMove
{
    [SerializeField] float maxAttackDist;

    [SerializeField] float overrideSpeed;
    [SerializeField] float baseSpeed;
    public float speedMove;

    public bool canCall = true;

    SacAttak attak;

    private void Start()
    {
        attak = GetComponent<SacAttak>();
        speedMove = baseSpeed;
    }

    public override IEnumerator MoveOver(GameObject lastPos)
    {
        if (canMove && canCall)
        {
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
        canCall = false;

        attak.Attack();

        //Tant que l'ennemi doit bouger
        while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
        {
            Debug.DrawRay(transform.position, new Vector2(posToGo.x-transform.position.x, posToGo.y-transform.position.y), Color.yellow, 0.5f);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToGo.x, transform.position.y, 0), Time.deltaTime * speedMove);
            yield return new WaitForSeconds(0.01f);
        }
        EndAttack();
    }

    public void EndAttack()
    {
        StopCoroutine("AttackMove");
        canCall = true;
        speedMove = baseSpeed;
        GetComponentInChildren<HitSac>().attackIndex = 0;
        attak.StopAttack();
    }
}
