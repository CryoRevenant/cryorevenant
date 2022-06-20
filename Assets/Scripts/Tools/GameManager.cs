using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float score;
    float savedScore;

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeWaitTime;
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] GameObject respawnMenu;
    [SerializeField] TextMeshProUGUI hintText;

    bool isFading;
    bool canRespawn;

    public Transform respawnPoint;
    GameObject player;

    [SerializeField] List<ListOfLists> listEnemies = new List<ListOfLists>();
    [SerializeField] List<Brasero> resetPoints = new List<Brasero>();

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        scoreText.text = score.ToString();
        player = GameObject.Find("Player");
    }

    public void AddToList(int listIndex, GameObject newGameObject)
    {
        if (listEnemies.Count < listIndex)
        {
            ListOfLists newList = new ListOfLists();
            listEnemies.Add(newList);
            listEnemies[listIndex].Add(newGameObject);
        }
        else
        {
            listEnemies[listIndex].Add(newGameObject);
        }
    }

    public void RemoveFromList(int index)
    {
        if (listEnemies[index].CheckActive())
        {
            resetPoints[index].Activate();
        }
    }

    public void AddScore(float scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = score.ToString();
    }

    public void SaveScore(Transform checkpoint)
    {
        savedScore = score;
        respawnPoint = checkpoint;
        scoreText.text = score.ToString();
    }

    public IEnumerator Fade()
    {
        HintPhrase();
        isFading = true;
        Color alphaMod = new Color();
        while (isFading == true)
        {
            alphaMod.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = alphaMod;

            if (alphaMod.a >= 0.98f)
            {
                isFading = false;
            }
            yield return new WaitForSeconds(0.05f);
        }
        respawnMenu.SetActive(true);
        yield return new WaitUntil(CanRespawn);

        player.transform.position = respawnPoint.position;
        player.GetComponent<IceBar>().StartCoroutine("ResetBar");
        player.gameObject.tag = "Player";

        RespawnEnemy();

        score = savedScore;

        while (isFading == false)
        {
            alphaMod.a -= Time.deltaTime * fadeSpeed;
            fadeImage.color = alphaMod;
            yield return new WaitForSeconds(0.05f);

            if (alphaMod.a <= 0)
            {
                player.layer = 0;
                player.GetComponent<PlayerAttack>().enabled = true;
                player.GetComponent<PlayerControllerV2>().enabled = true;
                canRespawn = false;
                StopCoroutine("Fade");
            }
        }
    }

    bool CanRespawn()
    {
        if (canRespawn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RespawnEnemy()
    {
        for (int i = 0; i < listEnemies.Count; i++)
        {
            foreach (ListOfLists list in listEnemies)
            {
                if (listEnemies[i].CheckActive() == false)
                {
                    listEnemies[i].Respawn();
                }
            }
        }
    }

    void HintPhrase()
    {
        int i = Random.Range(0, 6);

        switch (i)
        {
            case 0:
                hintText.text = "Dash to avoid hits or flee to a safe place !";
                break;
            case 1:
                hintText.text = "Make every hit count !";
                break;
            case 2:
                hintText.text = "Try every spell on every enemy, you may find something useful.";
                break;
            case 3:
                hintText.text = "Look closely at the enemies, hit and run isn't always the solution.";
                break;
            case 4:
                hintText.text = "Take your time ! Or not, it's up to you.";
                break;
            case 5:
                hintText.text = "You can break doors with a normal attack or a dash.";
                break;
        }
    }

    public void BoolRespawn()
    {
        canRespawn = true;
        respawnMenu.SetActive(false);
    }
}
