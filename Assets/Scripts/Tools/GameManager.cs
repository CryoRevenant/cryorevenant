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
    bool isFading;

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

        yield return new WaitForSeconds(fadeWaitTime);
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
                StopCoroutine("Fade");
            }
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
}
