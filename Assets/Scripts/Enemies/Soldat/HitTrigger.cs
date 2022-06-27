using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger : MonoBehaviour
{
    SoldatAttack attack;
    EnemyHealth2 hp;
    EnemyMove move;
    public BoxCollider2D trigger;
    [SerializeField] SpriteRenderer colorBox;
    [SerializeField] int fakeChance;
    [SerializeField] int invAttackChance;
    public GameObject shield;
    public bool isInvincible;

    private void Start()
    {
        move = GetComponent<EnemyMove>();
        hp = GetComponent<EnemyHealth2>();
        attack = GetComponent<SoldatAttack>();
    }

    void Increase()
    {
        attack.indexParent++;
        attack.parentAnim.SetInteger("indexAttack", attack.indexParent);
    }

    void Check()
    {
        attack.CheckAttack();
    }

    void StopAttack()
    {
        hp.isAttacking = false;
        move.LockMove(false);
        trigger.enabled = false;
        attack.parentAnim.SetBool("isAttacking", false);
    }

    void ResetAnim()
    {
        attack.Reset();
    }

    void WillAttack()
    {
        attack.willAttack = false;
    }

    void isVulnerable()
    {
        move.canMove = false;

        move.LockMove(true);

        trigger.enabled = false;

        attack.parentAnim.SetBool("isAttacking", false);
        hp.isBlocking = false;
        hp.isAttacking = false;

        Color newColor = new Vector4(250,220,114,255)/255;
        shield.GetComponent<SpriteRenderer>().color = newColor;
        shield.SetActive(false);

    }

    void isAttacking()
    {

        float random = Random.value;

        if (random > 0.5f)
        {
            FindObjectOfType<AudioManager>().Play("soldatAttack");
        }
        else if (random <= 0.5f)
        {
            FindObjectOfType<AudioManager>().Play("soldatAttack2");
        }


        move.LockMove(true);

        hp.isAttacking = true;
        attack.parentAnim.SetBool("isAttacking", true);
    }

    public void TriggerOn()
    {
        trigger.enabled = true;
    }

    void isBlocking()
    {
        move.LockMove(false);
        hp.isBlocking = true;
    }

    void FakeAttack()
    {
        int i = Random.Range(0, fakeChance);

        if (i == 1)
        {
            GetComponentInParent<SoldatAttack>().SneakAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponentInParent<EnemyAttack>().anim.SetBool("forceBlock", true);
        }
    }

    void ChanceInvAttack()
    {
        int i = Random.Range(0, invAttackChance);

        if (i == 1)
        {
            hp.anim.SetBool("invAttack", true);
        }
    }

    void InvincibleAttack()
    {
        isInvincible = true;
        shield.SetActive(true);
    }

    void UnInvincible()
    {
        if (isInvincible == true)
        {
            isInvincible = false;
        }
    }
}
