using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public bool isDead;
    public bool canDie;
    [SerializeField] Animator animator;
    [SerializeField] GameObject iceParticle;

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

    public IEnumerator IceDeath()
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

            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.cyan;

            yield return new WaitForSeconds(1.5f);

            gameObject.SetActive(false);
            GameObject instance = Instantiate(iceParticle, transform.position, Quaternion.identity);
            Destroy(instance,1);

            yield return new WaitForSeconds(3);

            GameManager.instance.StopCoroutine("Fade");
            GameManager.instance.StartCoroutine("Fade");

            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.SetActive(true);

            yield break;
        }

        yield break;
    }
}
