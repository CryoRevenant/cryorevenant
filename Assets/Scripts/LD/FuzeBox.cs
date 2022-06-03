using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class FuzeBox : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] Sprite changeSprite;
    [SerializeField] Light2D light2D;
    [SerializeField] float speed;

    bool isDestroyed;
    bool canDestroy;
    bool intDown;

    float intensity = 0;

    GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        if (!intDown)
        {
            intensity += Time.deltaTime * speed;

            if (intensity >= 1)
            {
                intDown = true;
            }
        }
        if (intDown)
        {
            intensity -= Time.deltaTime * speed;

            if (intensity <= 0)
            {
                intDown = false;
            }
        }

        light2D.intensity = intensity;
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

            Invoke("Anim", 0.30f);
            Invoke("ChangeCam", 2f);

            player.GetComponent<PlayerAttack>().enabled = false;
            player.GetComponent<PlayerControllerV2>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = changeSprite;

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
