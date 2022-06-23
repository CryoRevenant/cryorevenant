using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

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
    [SerializeField] private Image freezeUI;
    [SerializeField] RectMask2D rectMask;
    private Vector2Int softnessVect;

    [SerializeField] private CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Start()
    {
        iceAmount = 30;
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

        //Debug.Log(iceAmount);

        if (iceAmount == 30)
        {
            Vignette vignette;
            if (lowHP.profile.TryGet(out Vignette v))
            {
                vignette = v;
                vignette.intensity.value -= 0.02f;
                vignette.intensity.value = Mathf.Clamp(vignette.intensity.value, 0, 0.35f);
                //Debug.Log("add intensity");
            }

            softnessVect.x += 35;
            softnessVect.x = Mathf.Clamp(softnessVect.x, 0, 1000);
            softnessVect.y += 35;
            softnessVect.y = Mathf.Clamp(softnessVect.y, 0, 1000);
            rectMask.softness = softnessVect;

            Color tranparency = freezeUI.color;
            tranparency.a = 0;
            freezeUI.color = tranparency;
        }
    }

    public void AddBar(int amount)
    {
        Color tranparency = freezeUI.color;
        tranparency.a = 1;
        freezeUI.color = tranparency;

        Vignette vignette;
        if (lowHP.profile.TryGet(out Vignette v))
        {
            vignette = v;
            vignette.intensity.value += 0.02f;
            vignette.intensity.value = Mathf.Clamp(vignette.intensity.value,0, 0.35f);
            //Debug.Log("add intensity");
        }

        softnessVect.x -= 35;
        softnessVect.x = Mathf.Clamp(softnessVect.x, 0, 1000);
        softnessVect.y -= 35;
        softnessVect.y = Mathf.Clamp(softnessVect.y, 0, 1000);
        rectMask.softness = softnessVect;

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
        StartCoroutine(ShakeCamera(1f, 0.5f, 0.3f));

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

    public IEnumerator ShakeCamera(float intensity, float frequency, float time)
    {
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;

        yield return new WaitForSeconds(time);

        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;

        yield break;
    }

}
