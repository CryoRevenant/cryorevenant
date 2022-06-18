using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSac : MonoBehaviour
{
    Animator animator;

    public int attackIndex;

    [SerializeField] float timeMoveAgain;
    [SerializeField] SpriteRenderer parentColor;
    [SerializeField] GameObject hitTrigger;
    [SerializeField] BoxCollider2D blockSpike;

    GameObject player;

    EnemyMove move;

    private void Start()
    {
        player = GameObject.Find("Player");
        move = GetComponentInParent<EnemyMove>();
        animator = GetComponent<Animator>();
    }
    void Incr()
    {
        attackIndex++;
        animator.SetInteger("index", attackIndex);
    }

    public void Reset()
    {
        attackIndex = 0;
        animator.SetInteger("index", attackIndex);
        animator.SetBool("isAttacking", false);
        GetComponentInParent<SacMove>().EndAttack();
    }

    void TriggerOn()
    {
        float random = Random.value;
        if (random > 0.5f)
        {
            FindObjectOfType<AudioManager>().Play("sacAttack");
        }
        else if (random <= 0.5f)
        {
            FindObjectOfType<AudioManager>().Play("sacAttack2");
        }
        hitTrigger.GetComponent<BoxCollider2D>().enabled = true;
    }

    void TriggerOff()
    {
        hitTrigger.GetComponent<BoxCollider2D>().enabled = false;
    }

    // public void CubeOn()
    // {
    //     hitTrigger.SetActive(true);
    // }

    // public void CubeOff()
    // {
    //     hitTrigger.SetActive(false);
    // }

    public void BlockSpike()
    {
        //Debug.Log("block spike");
        FindObjectOfType<AudioManager>().Play("sacCutSpike");
        blockSpike.enabled = true;
    }

    void StopBlockSpike()
    {
        blockSpike.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            RaycastHit2D hit;
            if (hit = Physics2D.Raycast(transform.position, other.transform.position - transform.position, 10f))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    other.GetComponent<PlayerHP>().Death();
                    FindObjectOfType<AudioManager>().Play("sacKill");
                }
                if (other.transform.CompareTag("Wall"))
                {
                    other.GetComponent<IceWall>().Fall();
                }
            }
        }
    }

    void Block()
    {
        Reset();
        parentColor.color = Color.red;
        GetComponentInParent<EnemyHealth>().isBlocking = true;
    }

    void StopBlock()
    {
        parentColor.color = Color.white;
        GetComponentInParent<EnemyHealth>().isBlocking = false;
        GetComponentInParent<SacMove>().StopAllCoroutines();
    }

    void Nothing()
    {

    }

    void LookDirection()
    {
        move.LookDirection(player.transform.position);
    }
}
