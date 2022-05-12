using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            if (collision.gameObject.layer == 3)
            {
                if (collision.gameObject.GetComponent<SoldatHealth>() != null)
                {
                    collision.gameObject.GetComponent<SoldatHealth>().Slowed();
                }
                collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(0.5f);
            }
            Destroy(gameObject);
        }
    }
}
