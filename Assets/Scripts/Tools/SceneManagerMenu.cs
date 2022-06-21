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

        if (sceneIndex == 0)
        {
            if (gameObject.GetComponent<PlayerInput>() != null)
            {
                if (gameObject.GetComponent<PlayerInput>().currentActionMap.FindAction("Return").triggered && canReturn)
                {
                    //Debug.Log("return");
                    SwitchMenu(1);
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

    public void SwitchMenu(int index)
    {
        switch (index)
        {
            case 0:
                Invoke("ShowOption", 0.26f);
                childGlitch[1].GetComponent<Animator>().SetTrigger("isCut");
                break;
            case 1:
                if (curButton != null)
                {
                    EventSystem eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(curButton.gameObject, new BaseEventData(eventSystem));
                }
                Invoke("HideOptions", 0.3f);
                break;
            case 2:
                Invoke("ShowCredits", 0.26f);
                childGlitch[2].GetComponent<Animator>().SetTrigger("isCut");
                break;
            case 3:
                if (curButton != null)
                {
                    EventSystem eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(curButton.gameObject, new BaseEventData(eventSystem));
                }
                Invoke("HideCredits", 0.3f);
                break;
        }
        if (!isFading)
        {
            StartCoroutine(Fade(index));
        }
    }

    IEnumerator Fade(int i)
    {
        yield return new WaitForSeconds(0.1f);
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
                yield return new WaitForSeconds(0.1f);
                Debug.Log(alphaMod.a);
                switch (i)
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
                isFading = false;
            }
        }

        yield return new WaitForSeconds(0.2f);

        while (isFading == false)
        {
            alphaMod.a -= Time.deltaTime * speed;
            background.color = alphaMod;
            yield return new WaitForSeconds(0.05f);

            if (alphaMod.a <= 0)
            {
                StopAllCoroutines();
                Debug.Log(alphaMod.a);
            }
        }
    }

    public void Anim(int i)
    {
        glitchObjects[i].GetComponent<Animator>().SetTrigger("Glitch");
    }

    public void Click(int i)
    {
        childGlitch[i].GetComponent<Animator>().SetTrigger("isCut");
        Invoke("StartGame", 0.5f);
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
