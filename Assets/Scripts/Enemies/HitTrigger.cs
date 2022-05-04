using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger : MonoBehaviour
{
    SoldatAttack parent;
    EnemyHealth hp;
    EnemyMove move;
    [SerializeField] SpriteRenderer colorBox;

    private void Start()
    {
        move = GetComponentInParent<EnemyMove>();
        hp = GetComponentInParent<EnemyHealth>();
        parent = GetComponentInParent<SoldatAttack>();
    }

    void Check()
    {
        parent.CheckAttack();
        parent.index++;
    }

    void ResetAnim()
    {
        parent.Reset();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHP>().Death();
        }
    }

    void isVulnerable()
    {
        colorBox.color = Color.green;
        hp.isBlocking = false;
    }

    void isBlocking()
    {
        colorBox.color = Color.red;
        hp.isBlocking = true;
    }

    void isAttacking()
    {
        move.canMove = false;
        hp.isAttacking = true;
    }

    void StopAttack()
    {
        move.canMove = true;
        hp.isAttacking = false;
    }
}
