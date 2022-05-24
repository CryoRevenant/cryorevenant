using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float hp;
    float currHP;
    [Header("Index")]
    [SerializeField] float scoreToAdd;

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
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<EnemyMove>();
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

            child.GetComponent<BoxCollider2D>().enabled = false;
            child.sprite = resetSprite;
            gameObject.SetActive(false);
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
}
