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

    private void Update()
    {
    }

    public override void Attack()
    {
        if (canAttack == true)
        {
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
