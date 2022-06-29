using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacDeflect : MonoBehaviour
{
    EnemyMove move;
    [SerializeField] GameObject circle;

    private void Start()
    {
        move = GetComponentInParent<EnemyMove>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IceSpike>() != null)
        {
            if (other.transform.position.x < transform.position.x)
            {
                move.LookDirection(other.transform.position);
            }
            else
            {
                move.LookDirection(other.transform.position);
            }
            other.GetComponent<IceSpike>().CheckFlip();
            Destroy(other.gameObject);
        }

        if (other.GetComponent<Bullet>() != null && other.GetComponent<Bullet>().hitPlayer == false)
        {
            if (other.transform.position.x < transform.position.x)
            {
                move.LookDirection(other.transform.position);
            }
            else
            {
                move.LookDirection(other.transform.position);
            }

            circle.GetComponent<Animator>().SetTrigger("Deflect");
            other.GetComponent<Bullet>().Reflect();
        }
    }
}
