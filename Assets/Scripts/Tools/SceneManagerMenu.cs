using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    bool isPaused;
    bool canReturn;
    float timer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        isPaused = false;
        canReturn = false;

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
            if(controls != null)
            {
                if (controls.currentActionMap.FindAction("Pause").triggered)
                {
                    switch (isPaused)
                    {
                        case true:
                            //Debug.Log("UnPause");
                            HidePauseMenu();
                            isPaused = false;
                            break;
                        case false:
                            //Debug.Log("Pause");
                            PauseMenu();
                            break;
                    }
                }
            }

            if (gameObject.GetComponent<PlayerInput>() != null)
            {
                //Debug.Log("canReturn = " + canReturn);
                
                if (gameObject.GetComponent<PlayerInput>().currentActionMap.FindAction("Return").triggered && canReturn)
                {
                    //Debug.Log("return");
                    HidePauseOption();
                }
            }
        }

        if(sceneIndex == 0)
        {
            if (gameObject.GetComponent<PlayerInput>() != null)
            {
                if (gameObject.GetComponent<PlayerInput>().currentActionMap.FindAction("Return").triggered && canReturn)
                {
                    //Debug.Log("return");
                    StartFade(1);
                }
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
        //Debug.Log("option");
        canReturn = true;
        menuCanvas.SetActive(false);
        optionCanvas.SetActive(true);
        creditCanvas.SetActive(false);
    }

    public void HideOptions()
    {
        //Debug.Log("main menu");
        canReturn = false;
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
        //Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<PlayerAttack>().enabled = false;
        player.GetComponent<PlayerControllerV2>().enabled = false;
        pauseMenu.SetActive(true);
        optionMenu.SetActive(false);
        pauseButtons.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void HidePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<PlayerAttack>().enabled = true;
        player.GetComponent<PlayerControllerV2>().enabled = true;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowPauseOption()
    {
        Debug.Log("pause option");
        pauseButtons.SetActive(false);
        optionMenu.SetActive(true);
        canReturn = true;
    }

    public void HidePauseOption()
    {
        Debug.Log("pause menu");
        pauseButtons.SetActive(true);
        optionMenu.SetActive(false);
        canReturn = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        sceneIndex--;
        SceneManager.LoadScene(0);
    }
    #endregion
}
