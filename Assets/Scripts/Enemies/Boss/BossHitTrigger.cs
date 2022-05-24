using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitTrigger : MonoBehaviour
{
    BossAttack parent;
    BossHealth hp;
    BossMove move;
    CircleCollider2D trigger;
    [SerializeField] SpriteRenderer colorBox;

    private void Start()
    {
        move = GetComponentInParent<BossMove>();
        hp = GetComponentInParent<BossHealth>();
        parent = GetComponentInParent<BossAttack>();
        trigger = GetComponent<CircleCollider2D>();
    }

    void Check()
    {
        parent.CheckAttack();
    }

    void Increase()
    {
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
        move.StopCoroutine("MoveOver");
        hp.isBlocking = false;
        trigger.enabled = false;
    }

    void isBlocking()
    {
        colorBox.color = Color.white;
        hp.isBlocking = true;
    }

    void isAttacking()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        hp.isBlocking = false;
        colorBox.color = Color.red;
        move.canMove = false;
        move.StopCoroutine("MoveOver");
        hp.isAttacking = true;
        trigger.enabled = true;
        Invoke("StopAttack",0.25f);
    }

    void StopAttack()
    {
        move.canMove = true;
        hp.isAttacking = false;
        trigger.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void StopDash()
    {
        parent.anim.SetBool("dash", false);
    }

    void dash()
    {
        GetComponentInParent<EnemyMove>().StartCoroutine("Dash", 3);
    }

    void StopBlock()
    {
        parent.anim.SetBool("forceBlock", false);
    }

    void Nothing()
    {
        // Debug.Log("fuck ü");
    }

    void HideSprite()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
