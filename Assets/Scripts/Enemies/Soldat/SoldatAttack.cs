using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatAttack : EnemyAttack
{
    [Header("IndexAnim")]
    public int indexParent;

    float timer;
    bool willAttack = true;
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
        if (willAttack && !mustBlock && indexParent == 0)
        {
            parentAnim.SetBool("isBlocking", true);
        }
    }

    public override void CheckAttack()
    {
        RaycastHit2D hit;

        if (hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, radius, LayerMask.GetMask("Default")))
        {
            PreAttack();
        }
        else
        {
            GetComponent<SoldatAttack>().Reset();
        }
    }

    public override void Attack()
    {
        parentAnim.SetBool("isBlocking", false);
        int i = Random.Range(0, dashChance);

        if (i == 1)
        {
            GetComponent<EnemyHealth2>().StopCoroutine("RecoilHit");
            GoDash();
        }
        else
        {
            willAttack = true;

            parentAnim.SetBool("isDashing", false);

            parentAnim.SetBool("isPreAttacking", false);
            parentAnim.SetBool("isAttacking", true);

            parentAnim.SetInteger("indexAttack", indexParent);
        }
    }

    public void PreAttack()
    {
        if (willAttack)
        {
            willAttack = false;

            parentAnim.SetBool("isBlocking", false);
            parentAnim.SetBool("isPreAttacking", true);

            timer = Random.Range(minMaxTimer.x, minMaxTimer.y);

            Invoke("Attack", timer);
        }
    }

    public void SneakAttack()
    {
        parentAnim.SetTrigger("Sneak");
        Reset();
    }

    void GoDash()
    {
        //faire un dash lol
    }

    public void Reset()
    {
        indexParent = 0;

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
