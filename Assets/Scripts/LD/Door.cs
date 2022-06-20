using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject doorIdle;
    [SerializeField] GameObject doorDebris;

    List<GameObject> childsDebris = new List<GameObject>();

    [SerializeField] Transform expPos;

    [SerializeField] Vector2 maxPush;
    Vector2 dir;

    bool canDestroy;
    GameObject player;


    void Start()
    {
        for (int i = 0; i < doorDebris.transform.childCount; i++)
        {
            childsDebris.Add(doorDebris.transform.GetChild(i).gameObject);
        }

        canDestroy = false;
    }

    public void DestroyDoor()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        FindObjectOfType<AudioManager>().Play("cutDoor");

        doorIdle.SetActive(false);
        doorDebris.SetActive(true);
        foreach (GameObject go in childsDebris)
        {
            dir.x = Random.Range(5, maxPush.x);
            dir.y = Random.Range(0, maxPush.y);
            go.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
        }
        Invoke("DestroyGM", 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collision name = " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("collision name = " + collision.gameObject.name);
            player = collision.gameObject;
            if (player.GetComponent<PlayerControllerV2>().IsDashing())
            {
                DestroyDoor();
                if (player != null)
                {
                    StartCoroutine(player.GetComponent<PlayerAttack>().ShakeCamera(1f, 0.25f, 0.35f));
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Debug.Log("collision name = " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("collision name = " + collision.gameObject.name);
            player = collision.gameObject;
            if (player.GetComponent<PlayerControllerV2>().IsDashing())
            {
                DestroyDoor();
                if (player != null)
                {
                    StartCoroutine(player.GetComponent<PlayerAttack>().ShakeCamera(1f, 0.25f, 0.35f));
                }
            }
        }
    }

    void DestroyGM()
    {
        Destroy(gameObject);
    }
}
