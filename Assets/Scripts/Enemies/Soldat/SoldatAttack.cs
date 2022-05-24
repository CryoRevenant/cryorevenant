using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatAttack : EnemyAttack
{
    [Header("IndexAnim")]
    public int index;

    float timer;
    bool willAttack;
    [SerializeField] Vector2 minMaxTimer;

    public override void Start()
    {
        base.Start();
        timer = Random.Range(minMaxTimer.x, minMaxTimer.y);
    }

    private void Update()
    {
        if (willAttack)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            PreAttack();
        }
    }

    public override void Attack()
    {
        int i = Random.Range(0, 10);

        if (i == 1)
        {
            GoDash();
        }
        else
        {
            willAttack = true;
            anim.SetInteger("attackIndex", index);
            anim.SetBool("isPlayerNear", isPlayerNear);
        }
    }

    public void PreAttack()
    {
        anim.SetBool("isPreAttack", true);
        timer = Random.Range(minMaxTimer.x, minMaxTimer.y);
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
