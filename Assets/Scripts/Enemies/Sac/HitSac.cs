using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSac : MonoBehaviour
{
    Animator animator;
    public int attackIndex;
    [SerializeField] float timeMoveAgain;
    [SerializeField] SpriteRenderer parentColor;

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
        GetComponentInParent<SacMove>().StopCoroutine("AttackMove");
    }

    void CanMove()
    {
        GetComponentInParent<SacMove>().EndAttack();
    }

    void TriggerOn()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    void TriggerOff()
    {
        GetComponent<BoxCollider2D>().enabled = false;
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
    }
}
