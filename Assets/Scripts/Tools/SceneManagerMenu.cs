using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
    [SerializeField] int sceneIndex;

    [Header("Pause")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseButtons;
    [SerializeField] GameObject optionMenu;
    GameObject player;
    PlayerInput controls;

    private void Start()
    {
        if (sceneIndex == 1)
        {
            player = GameObject.Find("Player");
            controls = player.GetComponent<PlayerInput>();
        }
    }
    private void Update()
    {
        if (sceneIndex == 1)
        {
            if (controls.currentActionMap.FindAction("Pause").triggered)
            {
                PauseMenu();
            }
            else
            {
            }
        }
    }

    #region MenuPrincipal
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        sceneIndex++;
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
    #endregion

    #region PauseMenu

    public void PauseMenu()
    {
        player.GetComponent<PlayerAttack>().enabled = false;
        player.GetComponent<PlayerControllerV2>().enabled = false;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void HidePauseMenu()
    {
        player.GetComponent<PlayerAttack>().enabled = true;
        player.GetComponent<PlayerControllerV2>().enabled = true;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowPauseOption()
    {
        pauseButtons.SetActive(false);
        optionMenu.SetActive(true);
    }

    public void HidePauseOption()
    {
        pauseButtons.SetActive(true);
        optionMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        sceneIndex--;
        SceneManager.LoadScene(0);
    }
    #endregion
}
