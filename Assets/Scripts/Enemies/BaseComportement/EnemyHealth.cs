using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] string elevatorToUnlock;
    [SerializeField] bool needUnlock;

    public bool isBlocking;
    public bool isAttacking;
    GameObject elevator;

    EnemyMove move;
    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<EnemyMove>();
        if (needUnlock)
        {
            elevator = GameObject.Find(elevatorToUnlock);
            elevator.GetComponent<Ascenceur>().AddEnemy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isBlocking)
        {
            hp -= damage;
        }
        else if (isAttacking)
        {
            Recoil();
        }
        else
        {
            Block();
        }

        if (hp <= 0)
        {
            if (needUnlock)
                elevator.GetComponent<Ascenceur>().RemoveEnemy(gameObject);

            Destroy(gameObject);
        }
    }

    void Block()
    {
        if (move.lookLeft)
        {
            move.distDash = 1;
            move.StartCoroutine("Dash", 1);
            move.distDash = 5;
        }
        else
        {
            move.distDash = 2;
            move.StartCoroutine("Dash", 0);
            move.distDash = 5;
        }
    }

    void Recoil()
    {
        if (move.lookLeft)
        {
            move.distDash = 3;
            move.StartCoroutine("Dash", 1);
            anim.SetTrigger("forceReco");
            move.distDash = 5;
        }
        else
        {
            move.distDash = 3;
            anim.SetTrigger("forceReco");
            move.StartCoroutine("Dash", 0);
            move.distDash = 5;
        }
    }
}
