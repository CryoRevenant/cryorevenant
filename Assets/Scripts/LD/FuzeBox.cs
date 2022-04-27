using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FuzeBox : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera vcam;

    bool isDestroyed;

    GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    public void DestoyFuze()
    {
        if (isDestroyed == false)
        {
            vcam.m_Priority = 40;

            Invoke("Anim", 0.15f);
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
