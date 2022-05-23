using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacDeflect : MonoBehaviour
{
    EnemyMove move;
    [SerializeField] HitSac hit;

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
                move.lookLeft = true;
                move.LookDirection();
            }
            else
            {
                move.lookLeft = false;
                move.LookDirection();
            }
            hit.GetComponent<Animator>().SetTrigger("Deflect");
            other.GetComponent<IceSpike>().CheckFlip();
            Destroy(other.gameObject);
        }
    }
}
