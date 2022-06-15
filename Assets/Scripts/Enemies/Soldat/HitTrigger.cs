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

    private void Start()
    {
        move = GetComponent<EnemyMove>();
        hp = GetComponent<EnemyHealth2>();
        attack = GetComponent<SoldatAttack>();
    }

    #region trash;

    // void Check()
    // {
    //     attack.CheckAttack();
    // }

    // void FakeAttack()
    // {
    //     int i = Random.Range(0, fakeChance);

    //     if (i == 1)
    //     {
    //         GetComponentInParent<SoldatAttack>().SneakAttack();
    //     }
    // }

    // void Increase()
    // {
    //     attack.indexParent++;
    // }

    // void ResetAnim()
    // {
    //     attack.Reset();
    // }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         other.gameObject.GetComponent<PlayerHP>().Death();
    //     }
    // }

    // void isVulnerable()
    // {
    //     colorBox.color = Color.green;
    //     move.canMove = false;

    //     move.LockMove(true);

    //     hp.isBlocking = false;
    //     trigger.enabled = false;
    // }

    // void isBlocking()
    // {
    //     colorBox.color = Color.white;
    //     move.LockMove(false);
    //     hp.isBlocking = true;
    // }

    // void isAttacking()
    // {
    //     float random = Random.value;

    //     if (random > 0.5f)
    //     {
    //         FindObjectOfType<AudioManager>().Play("soldatAttack");
    //     }
    //     else if (random <= 0.5f)
    //     {
    //         FindObjectOfType<AudioManager>().Play("soldatAttack2");
    //     }

    //     colorBox.color = Color.red;

    //     hp.isBlocking = false;
    //     move.LockMove(true);

    //     hp.isAttacking = true;
    //     attack.parentAnim.SetBool("isAttacking", true);
    //     attack.parentAnim.SetBool("isRunning", true);
    //     attack.parentAnim.SetBool("isPreAttacking", false);
    //     trigger.enabled = true;
    // }

    // void StopAttack()
    // {
    //     move.LockMove(false);
    //     trigger.enabled = false;
    //     attack.parentAnim.SetBool("isAttacking", false);
    //     hp.isAttacking = false;
    // }


    // void StopDash()
    // {
    //     attack.anim.SetBool("dash", false);
    // }

    // void dash()
    // {
    //     GetComponent<EnemyMove>().StartCoroutine("Dash", 3);
    // }

    // void StopBlock()
    // {
    //     attack.anim.SetBool("forceBlock", false);
    // }

    // void Nothing()
    // {
    //     // Debug.Log("ü");
    // }

    // void HideSprite()
    // {
    //     GetComponent<SpriteRenderer>().enabled = false;
    // }
    #endregion

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
        colorBox.color = Color.green;
        move.canMove = false;

        move.LockMove(true);

        trigger.enabled = false;

        attack.parentAnim.SetBool("isAttacking", false);
        hp.isBlocking = false;
        hp.isAttacking = false;

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

        colorBox.color = Color.red;

        move.LockMove(true);

        hp.isAttacking = true;
        attack.parentAnim.SetBool("isAttacking", true);
        trigger.enabled = true;
    }

    void isBlocking()
    {
        colorBox.color = Color.white;
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
}
