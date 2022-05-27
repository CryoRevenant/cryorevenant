using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManagerMenu : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject optionCanvas;
    [SerializeField] GameObject creditCanvas;

    [Header("Fade")]
    [SerializeField] SpriteRenderer background;
    [SerializeField] float speed;
    [SerializeField] float fadeWaitTime;
    bool isFading;
    int test;

    void Start()
    {
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowOption()
    {
        menuCanvas.SetActive(false);
        optionCanvas.SetActive(true);
        creditCanvas.SetActive(false);
    }

    public void HideOptions()
    {
        menuCanvas.SetActive(true);
        optionCanvas.SetActive(false);
        creditCanvas.SetActive(false);
    }

    public void ShowCredits()
    {
        menuCanvas.SetActive(false);
        optionCanvas.SetActive(false);
        creditCanvas.SetActive(true);
    }

    public void HideCredits()
    {
        menuCanvas.SetActive(true);
        optionCanvas.SetActive(false);
        creditCanvas.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator Fade(int index)
    {
        isFading = true;
        Color alphaMod = new Color();
        while (isFading == true)
        {
            alphaMod.a += Time.deltaTime * speed;
            background.color = alphaMod;

            if (alphaMod.a >= 0.98f)
            {
                isFading = false;
            }
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(fadeWaitTime);
        switch (index)
        {
            case 0:
                ShowOption();
                break;
            case 1:
                HideOptions();
                break;
            case 2:
                ShowCredits();
                break;
            case 3:
                HideCredits();
                break;
        }

        while (isFading == false)
        {
            alphaMod.a -= Time.deltaTime * speed;
            background.color = alphaMod;
            yield return new WaitForSeconds(0.05f);

            if (alphaMod.a <= 0)
            {
                StopCoroutine("Fade");
            }
        }
    }

    public void StartFade(int i)
    {
        if (!isFading)
        {
            StartCoroutine(Fade(i));
        }
    }
}
