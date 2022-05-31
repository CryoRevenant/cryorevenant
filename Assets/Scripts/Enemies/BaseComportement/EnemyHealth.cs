using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float hp;
    float currHP;
    [Header("Index")]
    [SerializeField] string elevatorToUnlock;
    [SerializeField] bool needUnlock;
    [SerializeField] int indexIceBar;
    [SerializeField] float scoreToAdd;

    GameObject elevator;

    Vector3 originPos;

    EnemyMove move;

    [Header("Animation")]
    public Animator anim;
    public bool isBlocking;
    public bool isAttacking;
    [SerializeField] SpriteRenderer child;
    [SerializeField] Sprite resetSprite;

    private void Awake()
    {
        currHP = hp;
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
        if (!isBlocking && !isAttacking)
        {
            currHP -= damage;
            CheckSac();
        }
        else if (isAttacking)
        {
            Recoil();
        }
        else if (isBlocking)
        {
            Block();
        }

        if (currHP <= 0)
        {
            if (needUnlock)
                elevator.GetComponent<Ascenceur>().CheckOpen();

            child.GetComponent<BoxCollider2D>().enabled = false;
            child.sprite = resetSprite;
            gameObject.SetActive(false);
            GameManager.instance.RemoveFromList(indexIceBar, gameObject);
            GameManager.instance.AddScore(scoreToAdd);
        }
    }

    private void CheckSac()
    {
        if (GetComponent<SacAttak>() != null)
        {
            anim.SetTrigger("isHit");
        }
    }

    public virtual void Block()
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
            isAttacking = false;
            move.StartCoroutine("Dash", 1);
            Debug.Log("lookLeft");
            anim.SetTrigger("forceReco");
            move.distDash = 4;
        }
        else
        {
            move.distDash = 3;
            isAttacking = false;
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
