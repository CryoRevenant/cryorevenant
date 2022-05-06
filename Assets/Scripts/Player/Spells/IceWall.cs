using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    private bool canCoroutine;
    void Awake()
    {
        Destroy(gameObject, 3);
        canCoroutine = true;
    }

    private void Update()
    {
        Debug.Log(canCoroutine);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.GetComponent<PlayerControllerV2>());
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerControllerV2>().dashTime>0 && canCoroutine)
        {
            Physics2D.IgnoreLayerCollision(this.gameObject.layer, collision.gameObject.layer);
            StopCoroutine(StopIgnoreCol(collision));
            StartCoroutine(StopIgnoreCol(collision));
            canCoroutine = false;
        }
    }

    IEnumerator StopIgnoreCol(Collision2D col)
    {
        yield return new WaitForSeconds(0.4f);

        Physics2D.IgnoreLayerCollision(this.gameObject.layer, col.gameObject.layer, false);
        canCoroutine = true;
    }
}
