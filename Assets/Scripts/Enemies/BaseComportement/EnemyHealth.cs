using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] string elevatorToUnlock;
    [SerializeField] bool needUnlock;
    GameObject elevator;

    private void Awake()
    {
        if (needUnlock)
        {
            elevator = GameObject.Find(elevatorToUnlock);
            elevator.GetComponent<Ascenceur>().AddEnemy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            if (needUnlock)
                elevator.GetComponent<Ascenceur>().RemoveEnemy(gameObject);

            Destroy(gameObject);
        }
    }
}
