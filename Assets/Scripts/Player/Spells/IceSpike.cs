using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpike : MonoBehaviour
{
    [SerializeField] private GameObject spikeImpactVFX;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            if (collision.gameObject.layer == 3)
            {
                if (collision.gameObject.GetComponent<SoldatHealth>() != null)
                {
                    collision.gameObject.GetComponent<SoldatHealth>().Slowed();
                    collision.gameObject.GetComponent<EnemyHealth2>().TakeDamage(0.5f,"bullet");
                }

                if (collision.gameObject.GetComponent<BossHealth>())
                {
                    collision.gameObject.GetComponent<BossHealth>().TakeDamage(0.5f);
                }
            }

            CheckFlip();

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("HitTrigger"))
        {
            CheckFlip();
            Destroy(gameObject);
        }
    }

    public void CheckFlip()
    {
        switch (gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            case true:
                GameObject instanceR = Instantiate(spikeImpactVFX, transform.position, Quaternion.identity);
                instanceR.GetComponent<SpriteRenderer>().flipX = true;
                Destroy(instanceR, 0.5f);
                break;
            case false:
                GameObject instanceL = Instantiate(spikeImpactVFX, transform.position, Quaternion.identity);
                instanceL.GetComponent<SpriteRenderer>().flipX = false;
                Destroy(instanceL, 0.5f);
                break;
        }
    }
}
