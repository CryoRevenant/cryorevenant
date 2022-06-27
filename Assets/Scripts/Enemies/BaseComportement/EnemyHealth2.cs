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

    [Header("Dead Body")]
    [SerializeField] GameObject body1;
    [SerializeField] GameObject body2;

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

            //Debug.Log(gameObject.transform.localRotation.y);

            switch (gameObject.transform.localRotation.y)
            {
                case 0:
                    GameObject instanceR = Instantiate(body1, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
                    instanceR.GetComponent<SpriteRenderer>().flipX = true;
                    instanceR.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1), ForceMode2D.Impulse);
                    Destroy(instanceR.GetComponent<Collider2D>(), 0.05f);
                    Destroy(instanceR, 0.5f);

                    GameObject instanceR2 = Instantiate(body2, new Vector2(transform.position.x, transform.position.y - 0.5f), Quaternion.identity);
                    instanceR2.GetComponent<SpriteRenderer>().flipX = true;
                    instanceR2.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, -1), ForceMode2D.Impulse);
                    Destroy(instanceR2.GetComponent<Collider2D>(), 0.05f);
                    Destroy(instanceR2, 0.5f);
                    //Debug.Log("right");
                    break;
                case 1:
                    GameObject instanceL = Instantiate(body1, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
                    instanceL.GetComponent<SpriteRenderer>().flipX = false;
                    instanceL.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1), ForceMode2D.Impulse);
                    Destroy(instanceL.GetComponent<Collider2D>(), 0.05f);
                    Destroy(instanceL, 0.5f);

                    GameObject instanceL2 = Instantiate(body2, new Vector2(transform.position.x, transform.position.y - 0.5f), Quaternion.identity);
                    instanceL2.GetComponent<SpriteRenderer>().flipX = false;
                    instanceL2.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, -1), ForceMode2D.Impulse);
                    Destroy(instanceL2.GetComponent<Collider2D>(), 0.05f);
                    Destroy(instanceL2, 0.5f);
                    //Debug.Log("left");
                    break;
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
