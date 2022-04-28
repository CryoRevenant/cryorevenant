using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float attackDuration;
    public float cooldown;

    bool attack = true;

    GameObject triggerHit;

    // Start is called before the first frame update
    public virtual void Start()
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

        if (hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, radius, 1 << 0))
        {
            int i = Random.Range(0, 6);

            switch (i)
            {
                case 1:
                    int j = Random.Range(0, 2);
                    Debug.Log("Dash");
                    gameObject.GetComponent<EnemyMove>().StartCoroutine("Dash");
                    break;
                default:
                    Invoke("Attack", cooldown);
                    Debug.Log("Attack");
                    break;
            }
            Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized, Color.green, 0.5f);
        }
    }

    public virtual void Attack()
    {
        if (attack == true)
        {
            attack = false;
            triggerHit.SetActive(true);
            Invoke("StopAttack", attackDuration);
        }
    }

    void StopAttack()
    {
        attack = true;
        triggerHit.SetActive(false);
    }
}
