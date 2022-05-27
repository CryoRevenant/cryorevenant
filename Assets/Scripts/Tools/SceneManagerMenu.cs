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
    GameObject[] buttons;

    [Header("Fade")]
    [SerializeField] SpriteRenderer background;
    [SerializeField] float speed;
    [SerializeField] float fadeWaitTime;
    bool isFading;
    int test;

    void Start()
    {
        buttons = GameObject.FindGameObjectsWithTag("Buttons");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowOption()
    {
        menuCanvas.SetActive(false);
        optionCanvas.SetActive(true);
    }

    public void HideOptions()
    {
        menuCanvas.SetActive(true);
        optionCanvas.SetActive(false);
    }

    public void ShowCredits()
    {
        creditCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }

    public void HideCredits()
    {
        creditCanvas.SetActive(false);
        menuCanvas.SetActive(true);
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
                foreach (GameObject item in buttons)
                {
                    item.GetComponent<Button>().interactable = true;
                }
                StopCoroutine("Fade");
            }
        }
    }

    public void StartFade(int i)
    {
        //Debug.Log("helo");
        foreach (GameObject item in buttons)
        {
            item.GetComponent<Button>().interactable = false;
        }
        StartCoroutine(Fade(i));
    }
}
