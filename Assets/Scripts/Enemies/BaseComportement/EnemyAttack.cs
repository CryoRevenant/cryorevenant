using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float radius;
    [SerializeField] float attackDuration;
    public float cooldown;

    public bool attack = true;
    public bool isPlayerNear;

    public GameObject player;
    GameObject triggerHit;

    public Animator anim;
    public Animator parentAnim;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        if (GetComponent<SacAttak>() != null)
        {
            triggerHit = transform.GetChild(0).gameObject;
            anim = GetComponentInChildren<Animator>();
        }
        else
        {
            anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public virtual void CheckAttack()
    {
        RaycastHit2D hit;

        if (hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, radius, LayerMask.GetMask("Default")))
        {
            if (attack)
            {
                isPlayerNear = true;
                Attack();
            }
        }
        else
        {
            if (GetComponent<SoldatAttack>() != null)
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
