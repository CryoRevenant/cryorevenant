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

    private void Start()
    {
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
        animator.SetBool("isAttacking", false);
        GetComponentInParent<SacMove>().EndAttack();
    }

    void CanMove()
    {
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
            other.GetComponent<PlayerHP>().Death();
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
}
