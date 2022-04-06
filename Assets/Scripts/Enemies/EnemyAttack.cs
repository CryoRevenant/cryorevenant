using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float radius;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckAttack(GameObject player)
    {
        Collider2D detectCircle = Physics2D.OverlapCircle(transform.position, radius, 1 << 0);

        if (detectCircle != null)
        {
            Debug.DrawLine(transform.position, detectCircle.gameObject.transform.position, Color.green, 0.5f);
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("helo");
    }
}
