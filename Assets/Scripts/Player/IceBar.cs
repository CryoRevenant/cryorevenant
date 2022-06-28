using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using UnityEngine.InputSystem;

public class IceBar : MonoBehaviour
{
    public float iceAmount;
    float amountToLose;

    float timer;
    [SerializeField] float hurtTimer;
    bool hurt;

    [SerializeField] Animator animator;

    [SerializeField] GameObject backSlide;
    [SerializeField] Slider frontSlide;
    [SerializeField] float speed;
    [SerializeField] Image redFill;
    [SerializeField] float xVal;
    [SerializeField] GameObject handle;
    [SerializeField] AnimationCurve curve;
    float curveVal;
    Color newColor = new Vector4(1, 1, 1, 1);

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
        if (hurt)
        {
            timer -= Time.deltaTime;
            curveVal += Time.deltaTime;
            newColor.a = curve.Evaluate(curveVal);
            Debug.Log(newColor.a);
            handle.GetComponent<Image>().color = newColor;
        }

        if (timer <= 0)
        {
            LoseBar();
        }

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
            vignette.intensity.value = Mathf.Clamp(vignette.intensity.value, 0, 0.35f);
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
        handle.SetActive(true);

        frontSlide.value = 30 - amountToLose;

        redFill.GetComponent<RectTransform>().position -= new Vector3(xVal, 0, 0);

        hurt = true;

        timer = hurtTimer;

        iceAmount = 30 - amountToLose;

        if (iceAmount <= 0)
        {
            StartCoroutine(GetComponent<PlayerHP>().IceDeath());
        }
    }

    void LoseBar()
    {

        if (backSlide.transform.position.x <= redFill.GetComponent<RectTransform>().position.x)
        {
            hurt = false;
            handle.SetActive(false);
            curveVal = 0;
        }
        else
        {
            backSlide.transform.position -= new Vector3(0.02f, 0, 0);
        }
    }

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
