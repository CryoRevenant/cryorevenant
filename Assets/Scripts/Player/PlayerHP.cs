using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public bool isDead;
    public bool canDie;
    [SerializeField] Animator animator;
    [SerializeField] GameObject iceParticle;
    [SerializeField] GameObject freezeParticle;
    [SerializeField] GameObject iceCloudParticle;
    private GameObject instanceIce;
    private GameObject instanceFreeze;
    private GameObject instanceIceCloud;

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

            if (instanceIce == null)
            {
                instanceIce = Instantiate(iceParticle, transform.position, Quaternion.identity);
                Destroy(instanceIce, 2f);
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.cyan;

            yield return new WaitForSeconds(1.5f);

            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

            if (instanceFreeze == null)
            {
                instanceFreeze = Instantiate(freezeParticle, transform.position, Quaternion.identity);
                Destroy(instanceFreeze, 2f);
            }

            if (instanceIceCloud == null)
            {
                instanceIceCloud = Instantiate(iceCloudParticle, transform.position, Quaternion.identity);
                Destroy(instanceIceCloud, 2f);
            }

            yield return new WaitForSeconds(0.75f);

            GameManager.instance.StopCoroutine("Fade");
            GameManager.instance.StartCoroutine("Fade");

            yield return new WaitForSeconds(1);

            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

            yield break;
        }

        yield break;
    }
}
