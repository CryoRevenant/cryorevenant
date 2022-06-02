using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSac : MonoBehaviour
{
    Animator animator;

    public int attackIndex;

    [SerializeField] float timeMoveAgain;
    [SerializeField] SpriteRenderer parentColor;
    [SerializeField] BoxCollider2D hitTrigger;
    [SerializeField] BoxCollider2D blockSpike;

    GameObject player;

    EnemyMove move;

    private void Start()
    {
        player = GameObject.Find("Player");
        move = GetComponentInParent<EnemyMove>();
        animator = GetComponent<Animator>();
    }
    void Incr()
    {
        attackIndex++;
        animator.SetInteger("index", attackIndex);
    }

    public void Reset()
    {
        attackIndex = 0;
        animator.SetInteger("index", attackIndex);
        animator.SetBool("isAttacking", false);
        GetComponentInParent<SacMove>().EndAttack();
    }

    void TriggerOn()
    {
        hitTrigger.enabled = true;
    }

    void TriggerOff()
    {
        hitTrigger.enabled = false;
    }

    public void BlockSpike()
    {
        blockSpike.enabled = true;
    }

    void StopBlockSpike()
    {
        blockSpike.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            RaycastHit2D hit;
            if (hit = Physics2D.Raycast(transform.position, other.transform.position - transform.position, 10f))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    other.GetComponent<PlayerHP>().Death();
                }
                if (other.transform.CompareTag("Wall"))
                {
                    other.GetComponent<IceWall>().Fall();
                }
            }
        }
    }

    void Block()
    {
        Reset();
        parentColor.color = Color.magenta;
        GetComponentInParent<EnemyHealth>().isBlocking = true;
    }

    void StopBlock()
    {
        parentColor.color = Color.cyan;
        GetComponentInParent<EnemyHealth>().isBlocking = false;
        GetComponentInParent<SacMove>().StopAllCoroutines();
    }

    void Nothing()
    {

    }

    void LookDirection()
    {
        //Le joueur est à gauche ?
        if (player.transform.position.x < transform.position.x)
        {
            move.lookLeft = true;
        }
        else
        {
            move.lookLeft = false;
        }
        move.LookDirection();
    }
}
