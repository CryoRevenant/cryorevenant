using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Brasero : MonoBehaviour
{
    bool state;
    public bool passed;

    [SerializeField] int index;
    [SerializeField] ParticleSystem fire;
    [SerializeField] ParticleSystem smoke;

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
                StartCoroutine(ShakeGamepad(2f, 2f, 0.3f));

                Desactivate();

                passed = false;
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
        smoke.gameObject.SetActive(true);
    }

    public IEnumerator ShakeGamepad(float lowFreq, float highFreq, float duration)
    {
        Gamepad.current.SetMotorSpeeds(lowFreq, highFreq);

        yield return new WaitForSeconds(duration);

        Gamepad.current.SetMotorSpeeds(0, 0);

        yield break;
    }
}
