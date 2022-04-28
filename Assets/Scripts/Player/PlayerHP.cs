using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{

    public bool isDead;
    [SerializeField] bool canDie;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            GameManager.instance.SaveScore(other.transform);
        }
    }

    public void Death()
    {
        if (canDie)
        {
            Debug.Log("die");

            gameObject.layer = 6;

            GetComponent<PlayerControllerV2>().enabled = false;
            GetComponent<PlayerAttack>().enabled = false;

            GameManager.instance.StopCoroutine("Fade");
            GameManager.instance.StartCoroutine("Fade");
        }
    }
}
