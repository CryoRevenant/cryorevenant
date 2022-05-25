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

    BossMove move;
    GameObject player;

    [Header("Animation")]
    public Animator anim;
    public bool isBlocking;
    public bool isAttacking;

    private void Awake()
    {
        currHP = hp;
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<BossMove>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void TakeDamage(float damage)
    {
        currHP -= damage;
        //Debug.Log(currHP);

        if (currHP > 0)
        {
            StartCoroutine(move.Dash(player.transform.GetChild(0).gameObject));
        }

        if (currHP <= 0)
        {
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            gameObject.SetActive(false);
            GameManager.instance.AddScore(scoreToAdd);
        }
    }
}
