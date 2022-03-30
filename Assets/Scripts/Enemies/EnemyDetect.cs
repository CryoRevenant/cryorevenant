using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    [SerializeField] float radiusPlayer;
    [SerializeField] float radiusEnemy;

    bool detect;
    public bool otherDetect;

    EnemyMove move;

    // Start is called before the first frame update
    void Start()
    {
        move = GetComponent<EnemyMove>();
        StartCoroutine("DetectAround");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PreventOther()
    {
        Collider2D[] detectEnemy = Physics2D.OverlapCircleAll(transform.position, radiusEnemy);
        if (detectEnemy != null)
        {
            foreach (Collider2D other in detectEnemy)
            {
                if (other.gameObject.GetComponent<EnemyDetect>() != null && other.gameObject.GetComponent<EnemyDetect>().otherDetect == false)
                {
                    RaycastHit2D hit2D;
                    if (hit2D = Physics2D.Linecast(transform.position, other.transform.position))
                    {
                        if (hit2D.transform.gameObject.layer == 3)
                        {
                            GameObject otherGO = hit2D.transform.gameObject;

                            Debug.DrawLine(transform.position, hit2D.transform.position, Color.cyan, 2);

                            otherGO.GetComponent<EnemyDetect>().otherDetect = true;
                            otherGO.GetComponent<EnemyDetect>().StartCoroutine("PreventOther");

                            otherGO.GetComponent<EnemyMove>().mustGo = true;
                            otherGO.GetComponent<EnemyMove>().StopCoroutine("MoveOver");
                            otherGO.GetComponent<EnemyMove>().StartCoroutine("MoveOver", gameObject);

                        }
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }
        yield return null;
    }

    IEnumerator DetectAround()
    {
        Collider2D detectCircle = Physics2D.OverlapCircle(transform.position, radiusPlayer, 1 << 0);
        if (detectCircle != null)
        {
            RaycastHit2D hit;
            if (hit = Physics2D.Linecast(transform.position, detectCircle.gameObject.transform.position))
            {
                if (hit.transform.gameObject.layer == 0)
                {
                    Debug.DrawLine(gameObject.transform.position, detectCircle.transform.position, Color.magenta, 0.5f);

                    otherDetect = true;
                    move.mustGo = true;

                    move.StopCoroutine("MoveOver");
                    move.StartCoroutine("MoveOver", detectCircle.gameObject);

                    StartCoroutine("PreventOther");
                }
            }
        }
        else
        {
            otherDetect = false;
        }
        yield return new WaitForSeconds(1);
        StartCoroutine("DetectAround");
    }
}
