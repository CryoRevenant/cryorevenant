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
            parentAnim.SetBool("isBlocking", true);
        }

        if (timer <= 0 && willAttack)
        {
            parentAnim.SetBool("isBlocking", false);
            PreAttack();
        }
    }

    public override void Attack()
    {
        int i = Random.Range(0, dashChance);

        if (i == 1)
        {
            GetComponent<EnemyHealth>().StopCoroutine("RecoilHit");
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

    public void SneakAttack()
    {
        anim.SetBool("forceBlock", true);
        parentAnim.SetTrigger("Sneak");
        Reset();
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
        anim.SetBool("forceBlock", false);

        parentAnim.SetInteger("indexAttack", 0);
        parentAnim.SetBool("isAttacking", false);
        parentAnim.SetBool("isBlocking", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<EnemyAttack>().anim.SetBool("forceBlock", true);
        }
    }
}
