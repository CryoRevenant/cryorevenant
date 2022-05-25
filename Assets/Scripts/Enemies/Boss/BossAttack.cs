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

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
        triggerHit = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (player.GetComponent<PlayerControllerV2>().IsDashing() && gameObject.GetComponent<BossMove>().canMove)
        {
            Attack();
        }
    }

    public void Attack()
    {
        StartCoroutine(PreAttack());
        anim.SetInteger("attackIndex", index);
        anim.SetBool("isPlayerNear", isPlayerNear);
    }

    IEnumerator PreAttack()
    {
        anim.SetBool("isPreAttack", true);
        yield break;
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

    void StopAttack()
    {
        attack = true;
    }
}
