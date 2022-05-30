using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("IndexAnim")]
    public int index;

    [SerializeField] float radius;
    [SerializeField] float attackDuration;
    public float cooldown;

    public bool attack = true;
    public bool isPlayerNear;

    public GameObject player;
    GameObject triggerHit;

    public Animator anim;
    public bool canCheckAttack;

    void Awake()
    {
        canCheckAttack = true;
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
        triggerHit = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (player.GetComponent<PlayerControllerV2>().IsDashing() && gameObject.GetComponent<BossMove>().canMove && !gameObject.GetComponent<BossMove>().isSlowAttacking)
        {
            //Debug.Log(gameObject.GetComponent<BossMove>().isSlowAttacking);
            Attack();
        }
    }

    public void Attack()
    {
        //Debug.Log("attack");
        StartCoroutine(PreAttack());
        anim.SetInteger("attackIndex", index);
        anim.SetBool("isPlayerNear", isPlayerNear);
    }

    IEnumerator PreAttack()
    {
        //Debug.Log("preAttack");
        anim.SetBool("isPreAttack", true);
        yield break;
    }

    public void Reset()
    {
        //Debug.Log("Reset");
        index = 0;
        anim.SetBool("isPreAttack", false);
        anim.SetInteger("attackIndex", 0);
        anim.SetBool("isPlayerNear", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("ForceBlock");
            GetComponent<BossAttack>().anim.SetBool("forceBlock", true);
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

    public void CheckAttack()
    {
        if (canCheckAttack)
        {
            RaycastHit2D hit;

            if (hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, radius, 1 << 0))
            {
                if (attack)
                {
                    if (GetComponentInChildren<AimRay>() != null)
                    {
                        GetComponentInChildren<AimRay>().Aim();
                    }
                    isPlayerNear = true;
                    Attack();
                }
            }
            else
            {
                Reset();
            }
        }
    }

    void StopAttack()
    {
        //Debug.Log("StopAttack");
        attack = true;
    }

    public IEnumerator StartSlowAttack()
    {
        yield return new WaitForSeconds(1);
        //Debug.Log("StartSlowAttack");
        gameObject.GetComponent<BossMove>().isSlowAttacking = true;
        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("isSecondAttack", true);
        //gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("forceBlock", true);
        //gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("isPreAttack", false);
        yield break;
    }
}
