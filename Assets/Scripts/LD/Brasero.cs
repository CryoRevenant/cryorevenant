using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brasero : MonoBehaviour
{
    bool state;
    bool passed;

    [SerializeField] int index;
    [SerializeField] ParticleSystem fire;

    IceBar playerBar;

    private void Start()
    {
        playerBar = GameObject.Find("Player").GetComponent<IceBar>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (state && other.gameObject.GetComponent<IceBar>().iceAmount != 100 && !passed)
            {
                playerBar.StartCoroutine("ResetBar");
                GameManager.instance.AddScore(playerBar.iceAmount * 2);
                passed = true;
            }

            if (passed)
            {
                Desactivate();
            }
        }
    }

    public void Activate()
    {
        state = true;
        fire.gameObject.SetActive(true);
    }

    public void Desactivate()
    {
        state = false;
        GetComponentInChildren<AudioSource>().Stop();
        fire.Stop(true);
    }
}
