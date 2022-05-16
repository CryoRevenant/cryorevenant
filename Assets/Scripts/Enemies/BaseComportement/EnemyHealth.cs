using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float hp;
    float currHP;
    [SerializeField] string elevatorToUnlock;
    [SerializeField] bool needUnlock;
    [SerializeField] int indexIceBar;
    [SerializeField] float scoreToAdd;

    public bool isBlocking;
    public bool isAttacking;
    GameObject elevator;

    Vector3 originPos;

    EnemyMove move;

    public Animator anim;

    private void Awake()
    {
        currHP = hp;
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<EnemyMove>();
        originPos = transform.position;

        if (needUnlock)
        {
            elevator = GameObject.Find(elevatorToUnlock);
            elevator.GetComponent<Ascenceur>().AddEnemy(gameObject);
        }
    }

    private void Start()
    {
        GameManager.instance.AddToList(indexIceBar, gameObject);
    }

    public void TakeDamage(float damage)
    {
        if (!isBlocking)
        {
            currHP -= damage;
        }
        else if (isAttacking)
        {
            Recoil();
        }
        else
        {
            Block();
        }

        if (currHP <= 0)
        {
            if (needUnlock)
                elevator.GetComponent<Ascenceur>().RemoveEnemy(gameObject);

            gameObject.SetActive(false);
            GameManager.instance.RemoveFromList(indexIceBar, gameObject);
            GameManager.instance.AddScore(scoreToAdd);
        }
    }

    void Block()
    {
        if (move.lookLeft)
        {
            move.distDash = 1;
            move.StartCoroutine("Dash", 1);
            move.distDash = 4;
        }
        else
        {
            move.distDash = 2;
            move.StartCoroutine("Dash", 0);
            move.distDash = 4;
        }
    }

    void Recoil()
    {
        if (move.lookLeft)
        {
            move.distDash = 3;
            move.StartCoroutine("Dash", 1);
            anim.SetTrigger("forceReco");
            move.distDash = 4;
        }
        else
        {
            move.distDash = 3;
            anim.SetTrigger("forceReco");
            move.StartCoroutine("Dash", 0);
            move.distDash = 4;
        }
    }

    public void ResetPos()
    {
        transform.position = originPos;
        currHP = hp;
        GetComponent<EnemyDetect>().otherDetect = false;
        GetComponent<EnemyDetect>().StopCoroutine("DetectAround");
        GetComponent<EnemyDetect>().StartCoroutine("DetectAround");
    }
}
