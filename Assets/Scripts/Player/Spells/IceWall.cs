using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    private bool canCoroutine;
    private GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Invoke("Fall", 1f);
        canCoroutine = false;
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, player.layer, false);
    }

    private void Update()
    {
        //Debug.Log(canCoroutine);
        //Debug.Log(player);
        //Debug.Log(player.GetComponent<PlayerControllerV2>().IsDashing());

        if (canCoroutine && player.GetComponent<PlayerControllerV2>().IsDashing())
        {
            //Debug.Log("dash and ignore collision");
            Physics2D.IgnoreLayerCollision(this.gameObject.layer, player.layer);
            StartCoroutine(StopIgnoreCol());
            canCoroutine = false;
        }
    }

    public void Fall()
    {
        if (this.IsInvoking("Fall"))
        {
            CancelInvoke("Fall");
        }
        GetComponent<Animator>().SetTrigger("fall");
        Destroy(gameObject, 1);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.GetComponent<PlayerControllerV2>());
        if (collision.gameObject.CompareTag("Player"))
        {
            canCoroutine = true;
        }
    }

    IEnumerator StopIgnoreCol()
    {
        yield return new WaitForSeconds(0.3f);

        //Debug.Log("reactivate collision");
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, player.layer, false);
        yield break;
    }

    public void DisableCollider()
    {
        GetComponent<PolygonCollider2D>().enabled = false;
    }
}
