using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatAttack : EnemyAttack
{
    [Header("IndexAnim")]
    public int index;
    public int indexParent;

    float timer;
    bool willAttack;
    public bool mustBlock;
    [SerializeField] Vector2 minMaxTimer;
    [SerializeField] int dashChance;

    public override void Start()
    {
        base.Start();
        timer = Random.Range(minMaxTimer.x, minMaxTimer.y);
    }

    private void Update()
    {
        if (willAttack && !mustBlock)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0 && willAttack)
        {
            PreAttack();
        }
    }

    public override void Attack()
    {
        int i = Random.Range(0, dashChance);

        if (i == 1)
        {
            GoDash();
        }
        else
        {
            willAttack = true;

            parentAnim.SetBool("isDashing", false);

            anim.SetInteger("attackIndex", index);

            anim.SetBool("isPlayerNear", isPlayerNear);
            parentAnim.SetInteger("indexAttack", indexParent);
        }
    }

    public void PreAttack()
    {
        willAttack = false;

        anim.SetBool("isPreAttack", true);

        parentAnim.SetBool("isAttacking", true);

        timer = Random.Range(minMaxTimer.x, minMaxTimer.y);
    }

    void GoDash()
    {
        anim.SetBool("dash", true);
    }

    public void Reset()
    {
        index = 0;
        indexParent = 0;

        anim.SetBool("isPreAttack", false);
        anim.SetInteger("attackIndex", 0);
        anim.SetBool("isPlayerNear", false);

        parentAnim.SetInteger("indexAttack", 0);
        parentAnim.SetBool("isAttacking", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<EnemyAttack>().anim.SetBool("forceBlock", true);
        }
    }
}
