using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSniper : EnemyAttack
{
    GameObject player;
    [SerializeField] GameObject bullet;

    [SerializeField] float force;
    // Start is called before the first frame update
    public override void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attack()
    {
        GameObject shoot = Instantiate(bullet, transform.position, transform.rotation);
        shoot.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * force, ForceMode2D.Impulse);
    }
}
