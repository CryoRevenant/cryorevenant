using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPos;
    [SerializeField] private Vector2 attackPosDistance;
    [SerializeField] private float attackRange;
    [SerializeField] private int damage;
    [SerializeField] private float attackCooldown;
    private PlayerInput controls;
    private float timer;

    private void Awake()
    {
        timer = attackCooldown;
        controls = gameObject.GetComponent<PlayerInput>();
        attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);
    }

    private void Update()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);

        //Debug.Log(col.Length);

        for (int i = 0; i < col.Length; i++)
        {
            //Debug.Log(col[i].gameObject.name);
            timer -= Time.deltaTime;
            if (col[i].gameObject.CompareTag("Enemy") && controls.currentActionMap.FindAction("Attack").triggered && timer <= 0)
            {
                //Debug.Log("hit Enemy");
                col[i].gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
                timer = attackCooldown;
            }

            if (col[i].gameObject.CompareTag("Door") && controls.currentActionMap.FindAction("Attack").triggered && timer <= 0)
            {
                //Debug.Log("hit Enemy");
                col[i].gameObject.GetComponent<Door>().DestroyDoor();
                timer = attackCooldown;
            }
        }

        float xAxis = controls.currentActionMap.FindAction("Move").ReadValue<float>();

        if (xAxis > 0)
        {
            attackPos.localPosition = new Vector3(attackPosDistance.x, attackPosDistance.y, attackPos.position.z);
        }
        if (xAxis < 0)
        {
            attackPos.localPosition = new Vector3(-attackPosDistance.x, attackPosDistance.y, attackPos.position.z);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(new Vector3(attackPos.position.x, attackPos.position.y, attackPos.position.z), attackRange);
    }
}
