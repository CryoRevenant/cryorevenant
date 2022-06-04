using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSniper : EnemyAttack
{
    [SerializeField] GameObject bullet;

    public bool canAttack = true;
    bool touchSmth;

    [SerializeField] float force;
    // Start is called before the first frame update


    public override void Start()
    {
        base.Start();
    }

    public override void CheckAttack()
    {
        RaycastHit2D hit;
        int layerMask = LayerMask.GetMask("Box") + LayerMask.GetMask("Enemy");
        if (hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, radius, ~layerMask))
        {
            if (attack && hit.collider.transform.gameObject.layer == 0)
            {
                GetComponentInChildren<AimRay>().Aim();

                isPlayerNear = true;

                Invoke("Attack", cooldown);
            }
        }
        else
        {
            CancelInvoke("Attack");
            GetComponentInChildren<AimRay>().StopAim();
            isPlayerNear = false;
        }
    }

    public override void Attack()
    {
        if (canAttack == true)
        {
            FindObjectOfType<AudioManager>().Play("shotLaser");
            canAttack = false;
            GameObject shoot = Instantiate(bullet, transform.position, transform.rotation);
            shoot.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * force, ForceMode2D.Impulse);
            GetComponentInChildren<AimRay>().StopAim();
            Invoke("ReAttack", cooldown);
        }
    }

    void ReAttack()
    {
        canAttack = true;
    }
}
