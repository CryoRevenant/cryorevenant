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
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < doorDebris.transform.childCount; i++)
        {
            childsDebris.Add(doorDebris.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroyDoor()
    {
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


    void DestroyGM()
    {
        Destroy(gameObject);
    }
}
