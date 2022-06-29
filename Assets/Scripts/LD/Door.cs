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

    GameObject player;
    private GameObject instance;
    [HideInInspector] public bool gbeDestroyed;
    [SerializeField] private Transform[] points;

    void Start()
    {
        instance = Instantiate(doorDebris,transform.position,Quaternion.identity);
        instance.transform.SetParent(this.transform);
        
        for (int i = 0; i < instance.transform.childCount; i++)
        {
            childsDebris.Add(instance.transform.GetChild(i).gameObject);
        }

        gbeDestroyed = false;
        gameObject.GetComponent<LineRenderer>().positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            gameObject.GetComponent<LineRenderer>().SetPosition(i, points[i].position);
        }
    }

    public void DestroyDoor()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        FindObjectOfType<AudioManager>().Play("cutDoor");

        doorIdle.SetActive(false);
        instance.SetActive(true);
        //Debug.Log(childsDebris.Count);
        instance.transform.SetParent(null);

        foreach (GameObject go in childsDebris)
        {
            dir.x = Random.Range(5, maxPush.x);
            dir.y = Random.Range(0, maxPush.y);
            go.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
            //Debug.Log("addForce " + go.name);
        }
        Invoke("DestroyGM", 1f);
        gbeDestroyed = true;
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
                    StartCoroutine(player.GetComponent<PlayerAttack>().ShakeGamepad(1.5f, 1.5f, 0.15f));
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

    public void DestroyGM()
    {
        if (gbeDestroyed)
        {
            Destroy(gameObject);
        }

        doorIdle.SetActive(true);
        doorDebris.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = true;

        for (int i = 0; i < instance.transform.childCount; i++)
        {
            childsDebris.Remove(instance.transform.GetChild(i).gameObject);
        }
        Destroy(instance);

        if(instance != null)
        {
            instance = Instantiate(doorDebris, transform.position, Quaternion.identity);
            instance.transform.SetParent(this.transform);
            for (int i = 0; i < instance.transform.childCount; i++)
            {
                childsDebris.Add(instance.transform.GetChild(i).gameObject);
            }
        }
    }
}
