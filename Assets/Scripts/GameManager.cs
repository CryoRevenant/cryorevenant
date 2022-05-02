using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int score;
    int savedScore;

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeWaitTime;
    bool isFading;

    public Transform respawnPoint;
    GameObject player;

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

        player = GameObject.Find("Player");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }

    public void SaveScore(Transform checkpoint)
    {
        savedScore = score;
        respawnPoint = checkpoint;
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
}
