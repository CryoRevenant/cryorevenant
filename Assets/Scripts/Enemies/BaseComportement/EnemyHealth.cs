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

    bool forceStop;

    EnemyMove move;

    [Header("Animation")]
    public Animator anim;
    public Animator parentAnim;
    public bool isBlocking;
    public bool isAttacking;
    [SerializeField] SpriteRenderer child;
    [SerializeField] Sprite resetSprite;
    [SerializeField] float speedRecoil;

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
            CheckEnnemi();
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
            {
                elevator.GetComponent<Ascenceur>().CheckOpen();
            }

            if (GetComponent<EnemyDetect>() != null)
            {
                child.GetComponent<BoxCollider2D>().enabled = false;
                child.sprite = resetSprite;
            }

            gameObject.SetActive(false);
            GameManager.instance.RemoveFromList(indexIceBar, gameObject);
            GameManager.instance.AddScore(scoreToAdd);

            AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

            for (int a = 0; a < audioS.Length; a++)
            {
                if (audioS[a].clip.name == "charge-laser")
                {
                    audioS[a].enabled = false;
                }
            }
        }
    }

    private void CheckEnnemi()
    {
        if (GetComponent<SacAttak>() != null)
        {
            anim.SetTrigger("isHit");
        }

        if (GetComponent<AttackSniper>() != null)
        {
            GetComponent<AttackSniper>().CancelInvoke("Attack");
        }
    }


    public virtual void Block()
    {
        if (move.lookLeft)
        {
            StopCoroutine(RecoilHit(0, 0));
            StartCoroutine(RecoilHit(1, 2));
        }
        else
        {
            StopCoroutine(RecoilHit(0, 0));
            StartCoroutine(RecoilHit(0, 2));
        }
    }

    void Recoil()
    {
        if (move.lookLeft)
        {
            isAttacking = false;
            anim.SetTrigger("forceReco");

            StopCoroutine(RecoilHit(0, 0));
            StartCoroutine(RecoilHit(1, 3));
        }
        else
        {
            isAttacking = false;
            anim.SetTrigger("forceReco");

            StopCoroutine(RecoilHit(0, 0));
            StartCoroutine(RecoilHit(0, 3));
        }
    }

    public void ResetPos()
    {
        transform.position = originPos;
        currHP = hp;
        if (GetComponent<EnemyDetect>() != null)
        {
            GetComponent<EnemyDetect>().otherDetect = false;
            GetComponent<EnemyDetect>().StopCoroutine("DetectAround");
            GetComponent<EnemyDetect>().StartCoroutine("DetectAround");
        }
    }

    IEnumerator RecoilHit(int direction, float distDash)
    {
        forceStop = false;
        Vector3 newPos;

        if (direction == 0)
        {
            newPos = new Vector3(transform.position.x - distDash, transform.position.y, 0);
        }
        else
        {
            newPos = new Vector3(transform.position.x + distDash, transform.position.y, 0);
        }

        parentAnim.SetBool("isRecoil", true);

        float i = 0;
        while (transform.position.x != newPos.x || forceStop)
        {
            i += Time.deltaTime;
            if (i >= 1.5f)
            {
                forceStop = true;
                transform.position = new Vector2(newPos.x, transform.position.y);
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(newPos.x, transform.position.y, 0), Time.deltaTime * speedRecoil);
            yield return new WaitForSeconds(0.01f);
        }
        parentAnim.SetBool("isRecoil", false);

    }
}
