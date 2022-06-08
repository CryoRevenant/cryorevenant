using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceBar : MonoBehaviour
{
    public float iceAmount;
    float amountToLose;

    float timer;
    [SerializeField] float hurtTimer;

    bool hurt;

    [SerializeField] Slider backSlide;
    [SerializeField] Slider frontSlide;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        timer = hurtTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (hurt)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            LoseBar();
        }
    }

    public void AddBar(int amount)
    {
        FindObjectOfType<AudioManager>().Play("Cold");

        amountToLose += amount;

        frontSlide.value = 100 - amountToLose;

        hurt = true;
        timer = hurtTimer;

        iceAmount = 100 - amountToLose;

        if (iceAmount <= 0)
        {
            GetComponent<PlayerHP>().Death();
        }
    }

    void LoseBar()
    {
        //backSlide.value = Mathf.Lerp(backSlide.value, 100 - amountToLose, 0.02f);
        backSlide.value = 100 - amountToLose;
        hurt = false;
    }

    public IEnumerator ResetBar()
    {
        FindObjectOfType<AudioManager>().Play("Hot");

        while (iceAmount <= 100)
        {
            iceAmount += 1 * speed;
            amountToLose -= 1 * speed;
            frontSlide.value = iceAmount;

            yield return new WaitForSeconds(0.01f);
        }

        backSlide.value = 100;
        iceAmount = 100;
        amountToLose = 0;
        StopCoroutine("ResetBar");
    }
}
