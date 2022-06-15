using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatAttack : EnemyAttack
{
    [Header("IndexAnim")]
    public int indexParent;

    float timer;
    public bool willAttack;
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
        if (willAttack)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            PreAttack();
        }

    }

    public override void CheckAttack()
    {
        RaycastHit2D hit;
        if (GetComponent<SoldatHealth>().isAttacking == false)
        {
            if (hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, radius, LayerMask.GetMask("Default")))
            {
                willAttack = true;
            }
            else
            {
                parentAnim.SetBool("isNear", false);
                Reset();
            }
        }
    }

    public override void Attack()
    {
        int i = Random.Range(0, dashChance);

        if (i == 1)
        {
            GetComponent<EnemyHealth2>().StopCoroutine("RecoilHit");
            GoDash();
        }
        else
        {
            parentAnim.SetBool("isBlocking", false);

            parentAnim.SetBool("isDashing", false);

            parentAnim.SetBool("isPreAttacking", false);
            parentAnim.SetBool("isAttacking", true);
        }
    }

    public void PreAttack()
    {
        if (willAttack && parentAnim.GetBool("isBlocking") == true)
        {
            willAttack = false;

            GetComponent<SoldatHealth>().isAttacking = true;

            parentAnim.SetBool("isBlocking", false);
            parentAnim.SetBool("isPreAttacking", true);

            timer = Random.Range(minMaxTimer.x, minMaxTimer.y);

            Invoke("Attack", timer);
        }
    }

    public void SneakAttack()
    {
        parentAnim.SetTrigger("forceBlock");
        Reset();
    }

    void GoDash()
    {
        GetComponent<EnemyMove>().StartCoroutine("Dash", 3);
        parentAnim.SetBool("isPreAttacking", false);
        //faire un dash lol
    }

    public void Reset()
    {
        indexParent = 0;

        parentAnim.SetInteger("indexAttack", 0);
        parentAnim.SetBool("isAttacking", false);
        parentAnim.SetBool("isBlocking", false);
    }

    public void CheckPlayer()
    {
        Debug.Log(GetComponent<SoldatHealth>().isAttacking + " " + gameObject.name);
        if (GetComponent<SoldatHealth>().isAttacking == false)
        {
            parentAnim.SetBool("isRunning", false);
            parentAnim.SetBool("isBlocking", true);
        }
        parentAnim.SetBool("isNear", true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<EnemyAttack>().anim.SetBool("forceBlock", true);
        }
    }
}
