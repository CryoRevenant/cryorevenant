using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            GameManager.instance.SaveScore(other.transform);
        }
    }

    public void Death()
    {
        gameObject.layer = 6;
        GameManager.instance.StopCoroutine("Fade");
        GameManager.instance.StartCoroutine("Fade");
    }
}
