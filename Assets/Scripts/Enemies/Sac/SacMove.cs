using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacMove : EnemyMove
{
    [SerializeField] float maxAttackDist;

    [SerializeField] float overrideSpeed;
    [SerializeField] float baseSpeed;
    [SerializeField] float timerBeforeAttack;

    public float speedMove;

    public bool canCall = true;

    SacAttak attak;

    [Header("Audio")]
    [SerializeField] private AudioSource sacWalkSFX;

    private void Start()
    {
        attak = GetComponent<SacAttak>();
        speedMove = baseSpeed;
    }

    public override IEnumerator MoveOver(GameObject lastPos)
    {
        if (canMove && canCall)
        {
            sacWalkSFX.Play();
            Vector3 posToGo = lastPos.transform.position;

            //Tant que l'ennemi doit bouger
            while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToGo.x, transform.position.y, 0), Time.deltaTime * speedMove);

                LookDirection(posToGo);

                if (Vector3.Distance(transform.position, posToGo) < maxAttackDist)
                {
                    StartCoroutine("AttackMove", lastPos);
                    StopCoroutine("MoveOver");
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            sacWalkSFX.Stop();
        }
    }

    IEnumerator AttackMove(GameObject lastPos)
    {
        //GetComponentInChildren<HitSac>().CubeOn();
        float timer = timerBeforeAttack;
        Vector3 posToGo = lastPos.transform.position;

        speedMove = overrideSpeed;
        canCall = false;

        //Tant que l'ennemi doit bouger
        while (Vector3.Distance(transform.position, posToGo) > maxStoppingDist)
        {
            timer -= 0.1f;

            if (timer <= 0)
            {
                attak.Attack();
            }

            Debug.DrawRay(transform.position, new Vector2(posToGo.x - transform.position.x, posToGo.y - transform.position.y), Color.yellow, 0.5f);

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(posToGo.x, transform.position.y, 0), Time.deltaTime * speedMove);

            yield return new WaitForSeconds(0.01f);
        }
        EndAttack();
    }

    public void EndAttack()
    {
        StopCoroutine("AttackMove");
        //GetComponentInChildren<HitSac>().CubeOff();
        canCall = true;
        speedMove = baseSpeed;
        attak.StopAttack();
    }
}
