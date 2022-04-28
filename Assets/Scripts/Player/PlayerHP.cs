using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{

    public bool isDead;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            GameManager.instance.SaveScore(other.transform);
        }
    }

    public void Death()
    {
        Debug.Log("hzlo");
        // gameObject.layer = 6;
        // GameManager.instance.StopCoroutine("Fade");
        // GameManager.instance.StartCoroutine("Fade");
    }
}
