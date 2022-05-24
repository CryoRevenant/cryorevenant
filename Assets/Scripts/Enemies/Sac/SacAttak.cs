using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacAttak : EnemyAttack
{
    public override void Attack()
    {
        anim.SetBool("isAttacking", true);
    }

    public void StopAttack()
    {
        anim.SetBool("isAttacking", false);
    }
}
