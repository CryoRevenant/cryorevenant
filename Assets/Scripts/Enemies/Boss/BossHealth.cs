using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Health")]
    [SerializeField] Slider healthBar;

    private void Awake()
    {
        currHP = hp;
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<BossMove>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (healthBar != null)
        {
            healthBar.value = currHP;
        }
    }

    public void TakeDamage(float damage)
    {
        currHP -= damage;
        //Debug.Log(currHP);

        if (currHP > 0)
        {
            AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

            for (int a = 0; a < audioS.Length; a++)
            {
                if (audioS[a].clip.name == "ice-sword-block")
                {
                    audioS[a].Stop();
                    FindObjectOfType<AudioManager>().Play("iceSwordHit");

                }

                if (audioS[a].clip.name == "ice-sword-block2")
                {
                    audioS[a].Stop();
                    FindObjectOfType<AudioManager>().Play("iceSwordHit2");

                }
            }

            Invoke("BossDamageFinish",0.5f);
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }

        if (currHP == 1)
        {
            //Debug.Log("change phase");
            gameObject.GetComponent<BossMove>().bossPhase = 2;
        }

        if (transform.GetChild(0).GetComponent<BossHitTrigger>())
        {
            if (transform.GetChild(0).GetComponent<BossHitTrigger>().isAttackBeginning)
            {
                Debug.Log("DamageTaken");

                AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

                for (int a = 0; a < audioS.Length; a++)
                {
                    if (audioS[a].clip.name == "ice-sword-block")
                    {
                        audioS[a].Stop();
                        FindObjectOfType<AudioManager>().Play("iceSwordHit");

                    }

                    if (audioS[a].clip.name == "ice-sword-block2")
                    {
                        audioS[a].Stop();
                        FindObjectOfType<AudioManager>().Play("iceSwordHit2");

                    }
                }

                gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
                //gameObject.SetActive(false);
                //GameManager.instance.AddScore(scoreToAdd);
            }
            else if (currHP < 1)
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
            }
        }
    }

    void BossDamageFinish()
    {
        StartCoroutine(move.Dash(player.transform.GetChild(0).gameObject));
    }
}
