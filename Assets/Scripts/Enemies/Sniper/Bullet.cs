using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool hitPlayer = true;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(3, 7, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitPlayer)
        {
            if (other.gameObject.layer == 0)
            {
                other.gameObject.GetComponent<PlayerHP>().Death();
                Destroy(transform.parent.gameObject);
            }
        }
        else
        {
            Physics2D.IgnoreLayerCollision(3, 7, false);
            if (other.gameObject.layer == 3)
            {
                other.GetComponent<EnemyHealth>().TakeDamage(1);
                other.GetComponent<AttackSniper>().CancelInvoke("Attack");
                Destroy(transform.parent.gameObject);
            }
            if (other.gameObject.layer == 0)
            {
                other.gameObject.GetComponent<PlayerHP>().Death();
                Destroy(transform.parent.gameObject);
            }
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            hitPlayer = false;
        }

        if (other.gameObject.layer == 6)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}

