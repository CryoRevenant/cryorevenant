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
        anim.SetBool("isPreAttack", true);
        anim.SetInteger("attackIndex", index);
        anim.SetBool("isPlayerNear", isPlayerNear);
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

}
