using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FuzeBox : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera vcam;

    bool isDestroyed;
    bool canDestroy;

    GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canDestroy = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canDestroy = true;
        }
    }

    public void DestoyFuze()
    {
        if (isDestroyed == false && canDestroy == true)
        {
            FindObjectOfType<AudioManager>().Play("buttonPress");
            vcam.m_Priority = 40;

            Invoke("Anim", 0.25f);
            Invoke("ChangeCam", 2f);

            player.GetComponent<PlayerAttack>().enabled = false;
            player.GetComponent<PlayerControllerV2>().enabled = false;

            isDestroyed = true;
        }
    }

    void Anim()
    {
        animator.SetTrigger("Fall");
    }

    void ChangeCam()
    {
        vcam.m_Priority = -40;

        player.GetComponent<PlayerAttack>().enabled = true;
        player.GetComponent<PlayerControllerV2>().enabled = true;
    }
}
