using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneManagerMenu : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject optionCanvas;
    [SerializeField] GameObject creditCanvas;
    [SerializeField] List<GameObject> glitchObjects = new List<GameObject>();
    [SerializeField] List<GameObject> childGlitch = new List<GameObject>();

    [Header("Fade")]
    [SerializeField] SpriteRenderer background;
    [SerializeField] float speed;
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

    private Button curButton;

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

        foreach (GameObject item in glitchObjects)
        {
            item.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        }

    }
    private void Update()
    {
        if (sceneIndex == 1)
        {
            if (controls != null)
            {
                if (controls.currentActionMap.FindAction("Pause").triggered)
                {
                    switch (isPaused)
                    {
                        case true:
                            HidePauseMenu();
                            isPaused = false;
                            break;
                        case false:
                            PauseMenu();
                            break;
                    }
                }
            }

            if (gameObject.GetComponent<PlayerInput>() != null)
            {

                if (gameObject.GetComponent<PlayerInput>().currentActionMap.FindAction("Return").triggered && canReturn)
                {
                    HidePauseOption();
                }
            }
        }

        if (sceneIndex == 0)
        {
            if (gameObject.GetComponent<PlayerInput>() != null)
            {
                if (gameObject.GetComponent<PlayerInput>().currentActionMap.FindAction("Return").triggered && canReturn)
                {
                    if (optionCanvas.activeSelf)
                    {
                        Click(3);
                    }

                    if (creditCanvas.activeSelf)
                    {
                        Click(4);
                    }
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
        canReturn = true;
        menuCanvas.SetActive(false);
        optionCanvas.SetActive(true);
        creditCanvas.SetActive(false);
    }

    public void HideOptions()
    {
        canReturn = false;
        menuCanvas.SetActive(true);
        optionCanvas.SetActive(false);
        creditCanvas.SetActive(false);
    }

    public void ShowCredits()
    {
        canReturn = true;
        menuCanvas.SetActive(false);
        optionCanvas.SetActive(false);
        creditCanvas.SetActive(true);
    }

    public void HideCredits()
    {
        canReturn = false;
        menuCanvas.SetActive(true);
        optionCanvas.SetActive(false);
        creditCanvas.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetCurrBtn(Button btn)
    {
        curButton = btn;
    }

    IEnumerator Fade(int i)
    {
        Color alphaMod = new Color();
        alphaMod.a = 0;
        isFading = true;

        while (isFading == true)
        {
            alphaMod.a += Time.deltaTime * speed;
            background.color = alphaMod;
            yield return new WaitForSeconds(0.05f);

            if (alphaMod.a >= 1.5f)
            {
                isFading = false;
                switch (i)
                {
                    case 1:
                        ShowOption();
                        break;
                    case 2:
                        ShowCredits();
                        break;
                    case 3:
                        HideOptions();
                        break;
                    case 4:
                        HideCredits();
                        break;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        while (isFading == false)
        {
            alphaMod.a -= Time.deltaTime * speed;
            background.color = alphaMod;
            yield return new WaitForSeconds(0.05f);

            if (alphaMod.a <= 0)
            {
                StopAllCoroutines();
            }
        }
    }

    public void Anim(int i)
    {
        glitchObjects[i].GetComponent<Animator>().SetTrigger("Glitch");
    }

    public void Click(int index)
    {
        childGlitch[index].GetComponent<Animator>().SetTrigger("isCut");

        switch (index)
        {
            case 0:
                Invoke("StartGame", 0.5f);
                break;

            case 1:
                if (!isFading)
                {
                    StartCoroutine(Fade(index));
                }
                if (curButton != null)
                {
                    //Debug.Log("select Option");
                    EventSystem eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(curButton.gameObject, new BaseEventData(eventSystem));
                }
                break;

            case 2:
                if (!isFading)
                {
                    StartCoroutine(Fade(index));
                }
                if (curButton != null)
                {
                    EventSystem eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(curButton.gameObject, new BaseEventData(eventSystem));
                }
                break;

            case 3:
                if (!isFading)
                {
                    StartCoroutine(Fade(index));
                }
                break;

            case 4:
                if (!isFading)
                {
                    StartCoroutine(Fade(index));
                }
                break;
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
        //Debug.Log("pause option");
        pauseButtons.SetActive(false);
        optionMenu.SetActive(true);
        canReturn = true;
    }

    public void HidePauseOption()
    {
        //Debug.Log("pause menu");
        if (curButton != null)
        {
            EventSystem eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(curButton.gameObject, new BaseEventData(eventSystem));
        }
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
