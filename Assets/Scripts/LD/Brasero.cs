using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brasero : MonoBehaviour
{
    bool state;
    bool passed;
    public int index;
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
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void Desactivate()
    {
        state = false;
        GetComponent<SpriteRenderer>().color = Color.black;
    }
}
