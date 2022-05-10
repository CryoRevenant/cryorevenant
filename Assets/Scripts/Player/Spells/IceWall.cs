using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    private bool canCoroutine;
    private Collision2D col;
    void Awake()
    {
        Destroy(gameObject, 3);
        canCoroutine = true;
    }

    private void Update()
    {
        Debug.Log(canCoroutine);

        if (canCoroutine && col.gameObject.GetComponent<PlayerControllerV2>().dashTime > 0)
        {
            Physics2D.IgnoreLayerCollision(this.gameObject.layer, col.gameObject.layer);
            StopCoroutine(StopIgnoreCol());
            StartCoroutine(StopIgnoreCol());
            canCoroutine = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.GetComponent<PlayerControllerV2>());
        if (collision.gameObject.CompareTag("Player"))
        {
            col = collision;
            canCoroutine = true;
        }
    }

    IEnumerator StopIgnoreCol()
    {
        yield return new WaitForSeconds(0.3f);

        Physics2D.IgnoreLayerCollision(this.gameObject.layer, col.gameObject.layer, false);
        canCoroutine = true;
    }
}
