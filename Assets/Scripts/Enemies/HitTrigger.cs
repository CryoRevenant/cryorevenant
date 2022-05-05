using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger : MonoBehaviour
{
    SoldatAttack parent;
    EnemyHealth hp;
    EnemyMove move;
    CircleCollider2D trigger;
    [SerializeField] SpriteRenderer colorBox;

    private void Start()
    {
        move = GetComponentInParent<EnemyMove>();
        hp = GetComponentInParent<EnemyHealth>();
        parent = GetComponentInParent<SoldatAttack>();
        trigger = GetComponent<CircleCollider2D>();
    }

    void Check()
    {
        parent.CheckAttack();
        parent.index++;
    }

    void ResetAnim()
    {
        parent.Reset();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHP>().Death();
        }
    }

    void isVulnerable()
    {
        colorBox.color = Color.green;
        move.canMove = false;
        hp.isBlocking = false;
        trigger.enabled = false;
    }

    void isBlocking()
    {
        colorBox.color = Color.gray;
        move.canMove = true;
        hp.isBlocking = true;
    }

    void isAttacking()
    {
        colorBox.color = Color.red;
        move.canMove = false;
        hp.isAttacking = true;
        trigger.enabled = true;
    }

    void StopAttack()
    {
        move.canMove = true;
        hp.isAttacking = false;
        trigger.enabled = false;
    }

    void StopDash()
    {
        parent.anim.SetBool("dash", false);
    }

    void dash()
    {
        GetComponentInParent<EnemyMove>().StartCoroutine("Dash", 3);
    }
}
