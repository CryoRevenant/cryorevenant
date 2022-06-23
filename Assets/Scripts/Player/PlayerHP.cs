using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public bool isDead;
    public bool canDie;
    [SerializeField] Animator animator;
    [SerializeField] List<GameObject> doors = new List<GameObject>();

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
            foreach (GameObject door in doors)
            {
                if (!door.activeSelf || door.GetComponent<Door>().gbeDestroyed)
                {
                    door.GetComponent<Door>().gbeDestroyed = false;
                    door.SetActive(true);
                    //Debug.Log("door name : " + door.gameObject.name);
                }
            }

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
