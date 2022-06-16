using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class IceBar : MonoBehaviour
{
    public float iceAmount;
    float amountToLose;

    float timer;
    [SerializeField] float hurtTimer;

    [SerializeField] Animator animator;

    [SerializeField] Slider backSlide;
    [SerializeField] Slider frontSlide;
    [SerializeField] float speed;

    [SerializeField] Volume lowHP;

    // Start is called before the first frame update
    void Start()
    {
        timer = hurtTimer;
    }

    void Update()
    {
        //if (hurt)
        //{
        //    timer -= Time.deltaTime;
        //}

        //if (timer <= 0)
        //{
        //    LoseBar();
        //}

        if (iceAmount <= 8)
        {
            lowHP.enabled = true;
        }
        else
        {
            lowHP.enabled = false;
        }
    }

    public void AddBar(int amount)
    {
        FindObjectOfType<AudioManager>().Play("Cold");
        animator.SetTrigger("glitch");

        amountToLose += amount;

        frontSlide.value = 30 - amountToLose;

        timer = hurtTimer;

        iceAmount = 30 - amountToLose;

        if (iceAmount <= 0)
        {
            GetComponent<PlayerHP>().Death();
        }
    }

    // void LoseBar()
    // {
    //backSlide.value = Mathf.Lerp(backSlide.value, 100 - amountToLose, 0.02f);
    //     if (hurt)
    //     {
    //         hurt = false;
    //     }
    // backSlide.value = 100 - amountToLose;
    // }

    public IEnumerator ResetBar()
    {
        FindObjectOfType<AudioManager>().Play("Hot");

        while (iceAmount <= 30)
        {
            iceAmount += 1 * speed;
            amountToLose -= 1 * speed;
            frontSlide.value = iceAmount;

            yield return new WaitForSeconds(0.01f);
        }

        // backSlide.value = 100;
        iceAmount = 30;
        amountToLose = 0;
        StopCoroutine("ResetBar");
    }
}
