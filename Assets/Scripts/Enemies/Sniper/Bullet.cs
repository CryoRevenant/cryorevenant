using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool hitPlayer = true;
    Rigidbody2D rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitPlayer)
        {
            if (other.gameObject.layer == 0)
            {
                other.gameObject.GetComponent<PlayerHP>().Death();
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.gameObject.layer == 3)
            {
                other.GetComponent<EnemyHealth>().TakeDamage(1);
                if (other.GetComponent<AttackSniper>() != null)
                {
                    other.GetComponent<AttackSniper>().CancelInvoke("Attack");
                }
                Destroy(gameObject);
            }
            if (other.gameObject.layer == 0)
            {
                other.gameObject.GetComponent<PlayerHP>().Death();
                Destroy(gameObject);
            }
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            Reflect();
        }

        if (other.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }
    }

    void Reflect()
    {
        hitPlayer = false;

        rigid.velocity = rigid.velocity * -1;
    }
}

