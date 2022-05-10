using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatHealth : EnemyHealth
{
    [SerializeField] RuntimeAnimatorController normalSpeed;
    [SerializeField] RuntimeAnimatorController slowSpeed;

    public void Slowed()
    {
        anim.runtimeAnimatorController = slowSpeed;
    }

    public void NormalSpeed()
    {
        anim.runtimeAnimatorController = normalSpeed;
    }
}
