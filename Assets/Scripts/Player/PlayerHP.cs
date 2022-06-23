using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public bool isDead;
    public bool canDie;
    [SerializeField] Animator animator;

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
            //Debug.Log("die");

            animator.SetTrigger("Death");
            animator.SetTrigger("isFalling");

            gameObject.layer = 6;
            gameObject.tag = "Untagged";

            GetComponent<PlayerControllerV2>().enabled = false;
            GetComponent<PlayerAttack>().enabled = false;

            GameManager.instance.StopCoroutine("Fade");
            GameManager.instance.StartCoroutine("Fade");
        }
    }
}
