using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float attackDuration;
    public float cooldown;

    public bool attack = true;
    public bool isPlayerNear;

    public GameObject player;
    GameObject triggerHit;

    public Animator anim;
    public EnemyHealth hp;

    // Start is called before the first frame update
    public virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        hp = GetComponent<EnemyHealth>();
        player = GameObject.Find("Player");
        triggerHit = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    }

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
            GetComponent<SoldatAttack>().Reset();
        }
    }

    public virtual void Attack()
    {
        attack = false;
    }

    void StopAttack()
    {
        attack = true;
    }
}
