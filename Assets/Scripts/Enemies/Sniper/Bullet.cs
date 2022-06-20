using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool hitPlayer = true;
    Rigidbody2D rigid;

    float timer = 1f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
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
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (other.gameObject.GetComponent<EnemyHealth>() != null)
                    other.GetComponent<EnemyHealth>().TakeDamage(1);
                else
                    other.GetComponent<EnemyHealth2>().TakeDamage(1);

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
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z +180);

        timer = 1f;

        rigid.velocity = rigid.velocity * -1;
    }
}

