using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatAttack : EnemyAttack
{
    [Header("IndexAnim")]
    public int index;

    public override void Attack()
    {
        CheckDash();
        StartCoroutine(PreAttack());
        anim.SetInteger("attackIndex", index);
        anim.SetBool("isPlayerNear", isPlayerNear);
    }

    IEnumerator PreAttack()
    {
        float i = Random.Range(0, 2);
        Debug.Log(i);
        yield return new WaitForSeconds(i);
        anim.SetBool("isPreAttack", true);
    }

    private void CheckDash()
    {
        int i = Random.Range(0, 10);

        if (i == 1)
        {
            GoDash();
        }
    }

    void GoDash()
    {
        anim.SetBool("dash", true);
    }

    public void Reset()
    {
        index = 0;
        anim.SetBool("isPreAttack", false);
        anim.SetInteger("attackIndex", 0);
        anim.SetBool("isPlayerNear", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<EnemyAttack>().anim.SetBool("forceBlock", true);
        }
    }

    // public void TriggerAttack1()
    // {
    //     animAttack.SetTrigger("Attack1");
    // }
    // public void TriggerAttack2()
    // {
    //     animAttack.SetTrigger("Attack2");
    // }
}
