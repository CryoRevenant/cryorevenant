using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSac : MonoBehaviour
{
    Animator animator;
    int attackIndex;
    [SerializeField] float timeMoveAgain;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Incr()
    {
        attackIndex++;
        animator.SetInteger("index", attackIndex);
    }

    void Reset()
    {
        attackIndex = 0;
        animator.SetBool("isAttacking", false);
        GetComponentInParent<SacMove>().StopCoroutine("AttackMove");
        Invoke("CanMove", timeMoveAgain);
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
}
