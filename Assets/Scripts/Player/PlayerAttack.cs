using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private int damage;
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private float attackCooldown;
    private float timer;

    private void Awake()
    {
        timer = attackCooldown;
    }

    private void Update()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);

        //Debug.Log(col.Length);

        for (int i = 0; i < col.Length; i++)
        {
            //Debug.Log(col[i].gameObject.name);
            timer -= Time.deltaTime;
            if (col[i].gameObject.CompareTag("Enemy") && attackInput.action.triggered && timer <= 0)
            {
                //Debug.Log("hit Enemy");
                col[i].gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
                timer = attackCooldown;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);
    }
}
