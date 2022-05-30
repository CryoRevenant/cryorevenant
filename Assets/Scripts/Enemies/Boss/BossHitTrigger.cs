using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHitTrigger : MonoBehaviour
{
    BossAttack parent;
    BossHealth hp;
    BossMove move;
    [SerializeField] SpriteRenderer colorBox;
    public bool isAttackBeginning;

    private void Start()
    {
        isAttackBeginning = false;
        move = GetComponentInParent<BossMove>();
        hp = GetComponentInParent<BossHealth>();
        parent = GetComponentInParent<BossAttack>();
    }

    private void Update()
    {
        //Debug.Log("isAttackBeginning " + isAttackBeginning);
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
        Invoke("StopAttack",0.25f);
    }

    void StopAttack()
    {
        move.canMove = true;
        hp.isAttacking = false;
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

    void isSlowAttacking()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        colorBox.color = Color.red;
    }

    void AttackBegin()
    {
        isAttackBeginning = true;
    }

    void AttackFinish()
    {
        isAttackBeginning = false;
    }

    public void StopSlowAttacking()
    {
        //Debug.Log("StopSlowAttack");
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.transform.parent.GetComponent<BossMove>().isSlowAttacking = false;
        gameObject.transform.parent.GetComponent<BossMove>().isStopped = false;
        gameObject.transform.parent.GetComponent<BossMove>().timer = 0;
        gameObject.transform.parent.GetComponent<BossMove>().canMove = true;
        gameObject.GetComponent<Animator>().SetBool("isSecondAttack", false);
    }
}
