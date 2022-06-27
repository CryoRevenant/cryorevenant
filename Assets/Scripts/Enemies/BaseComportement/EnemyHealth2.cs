using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth2 : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float hp;
    float currHP;
    [Header("Index")]
    [SerializeField] string elevatorToUnlock;
    [SerializeField] bool needUnlock;
    [SerializeField] int indexIceBar;
    [SerializeField] float scoreToAdd;
    public bool canRecoil;

    GameObject elevator;

    Vector3 originPos;

    bool forceStop;

    EnemyMove move;

    [Header("Animation")]
    public Animator anim;
    public bool isBlocking;
    public bool isAttacking;
    [SerializeField] BoxCollider2D hitTrigger;

    [SerializeField] float speedRecoil;
    [SerializeField] Slider healthBar;

    [Header("FX")]
    [SerializeField] GameObject vfxBlock;
    [SerializeField] GameObject vfxShield;

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

    private void Update()
    {
        if (healthBar != null)
        {
            healthBar.value = currHP;
        }
    }

    public void TakeDamage(float damage, string hitObject)
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
        else if (GetComponent<HitTrigger>().isInvincible == true)
        {
            CounterAttack();
        }
        else if (isBlocking)
        {
            if (hitObject == "sword")
            {
                //Debug.Log("sword");

                GameObject vfxBlockInstanceR = Instantiate(vfxBlock, transform.position, Quaternion.identity);
                Destroy(vfxBlockInstanceR, 0.25f);

                //Debug.Log(transform.rotation.y);
                switch (transform.rotation.y)
                {
                    case 0:
                        //Debug.Log("Spawn Right");

                        GameObject instanceR = Instantiate(vfxShield, transform.position, Quaternion.identity);
                        instanceR.GetComponent<SpriteRenderer>().flipX = true;
                        instanceR.transform.SetParent(transform);
                        Destroy(instanceR, 0.25f);
                        break;
                    case 1:
                        //Debug.Log("Spawn Left");

                        GameObject instanceL = Instantiate(vfxShield, transform.position, Quaternion.identity);
                        instanceL.GetComponent<SpriteRenderer>().flipX = false;
                        instanceL.transform.SetParent(transform);
                        Destroy(instanceL, 0.25f);
                        break;
                }

            }
            Block();
        }

        if (currHP <= 0)
        {
            if (needUnlock)
            {
                elevator.GetComponent<Ascenceur>().CheckOpen();
            }

            if (GetComponent<AttackSniper>() != null)
            {
                GetComponentInChildren<AimRay>().StopAim();
            }

            if (GetComponent<EnemyDetect>() != null)
            {
                hitTrigger.enabled = false;
            }

            gameObject.SetActive(false);
            GameManager.instance.RemoveFromList(indexIceBar);
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
        if (canRecoil && GetComponent<HitTrigger>().isInvincible == false)
        {
            Debug.Log("block");
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
    }

    void Recoil()
    {
        if (canRecoil)
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
    }

    void CounterAttack()
    {
        Color newColor = new Vector4(165,83,255,255)/255;
        GetComponent<HitTrigger>().shield.GetComponent<SpriteRenderer>().color = newColor;
        GetComponent<SoldatAttack>().anim.SetBool("isAttacking", true);
        GetComponent<HitTrigger>().TriggerOn();
    }

    public void ResetPos()
    {
        transform.position = originPos;
        currHP = hp;
        if (needUnlock)
        {
            elevator.GetComponent<Ascenceur>().Lock();
        }
        if (GetComponent<EnemyDetect>() != null)
        {
            GetComponent<EnemyDetect>().otherDetect = false;
            GetComponent<EnemyDetect>().StopCoroutine("DetectAround");
            GetComponent<EnemyDetect>().StartCoroutine("DetectAround");
        }
        else
        {
            GetComponent<DetectSniper>().StopCoroutine("DetectAround");
            GetComponent<DetectSniper>().StartCoroutine("DetectAround");
        }
    }

    IEnumerator RecoilHit(int direction, float distDash)
    {
        Vector3 newPos;

        if (direction == 0)
        {
            newPos = new Vector3(transform.position.x - distDash, transform.position.y, 0);
        }
        else
        {
            newPos = new Vector3(transform.position.x + distDash, transform.position.y, 0);
        }

        anim.SetBool("isRecoil", true);

        float i = 0;
        while (transform.position.x != newPos.x)
        {
            i += 0.1f;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(newPos.x, transform.position.y, 0), Time.deltaTime * speedRecoil);
            yield return new WaitForSeconds(0.01f);
            if (i >= 0.8f)
            {
                transform.position = newPos;
            }
        }
        anim.SetBool("isRecoil", false);
    }
}
