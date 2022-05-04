using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatAttack : EnemyAttack
{
    public int index;

    public override void Attack()
    {
        anim.SetBool("isPreAttack", true);
        anim.SetInteger("attackIndex", index);
        anim.SetBool("isPlayerNear", isPlayerNear);
    }

    public void Reset()
    {
        index = 0;
        anim.SetBool("isPreAttack", false);
        anim.SetInteger("attackIndex", 0);
        anim.SetBool("isPlayerNear", false);
    }
}
