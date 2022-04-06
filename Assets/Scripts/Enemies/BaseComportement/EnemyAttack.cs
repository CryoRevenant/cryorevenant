using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float timer;

    GameObject triggerHit;

    // Start is called before the first frame update
    void Start()
    {
        triggerHit = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckAttack(GameObject player)
    {
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(transform.position,(player.transform.position - transform.position).normalized, radius, 1<<0))
        {
            Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized, Color.green, 0.5f);
            Attack();
        }
    }

    void Attack()
    {
        triggerHit.SetActive(true);
        Invoke("StopAttack", timer);
    }

    void StopAttack()
    {
        triggerHit.SetActive(false);
    }
}
