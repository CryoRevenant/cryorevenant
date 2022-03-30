using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    [SerializeField] float radius;
    bool detect;
    public bool otherDetect;
    public bool mustGo;
    ContactFilter2D contact;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider2D detectCircle = Physics2D.OverlapCircle(transform.position, radius, 1 << 0);
        if (detectCircle != null)
        {
            Debug.DrawLine(gameObject.transform.position, detectCircle.transform.position, Color.magenta, 0.5f);
            Debug.Log(detectCircle.transform.name);
            otherDetect = true;
            mustGo = true;
            CallCouroutines(detectCircle.gameObject);
        }
    }

    IEnumerator PreventOther()
    {
        Collider2D[] detectEnemy = Physics2D.OverlapCircleAll(transform.position, radius);
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
                            Debug.DrawLine(transform.position, hit2D.transform.position, Color.cyan, 50);
                            hit2D.transform.gameObject.GetComponent<EnemyDetect>().otherDetect = true;
                            hit2D.transform.gameObject.GetComponent<EnemyDetect>().mustGo = true;
                            hit2D.transform.gameObject.GetComponent<EnemyDetect>().CallCouroutines(gameObject);
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

    IEnumerator MoveOver(GameObject lastPos)
    {
        Vector3 posToGo = lastPos.transform.position;
        while (mustGo == true)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, posToGo.x, Time.deltaTime * speed), Mathf.Lerp(transform.position.y, posToGo.y, Time.deltaTime *speed), 0);
            if (Vector3.Distance(transform.position, posToGo) < 1f)
            {
                StartCoroutine("Reset", lastPos);
                mustGo = false;
            }
            //la vitesse décroît par ennemi et ils finissent pas ne plus se déplacer tellement ils sont lents
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Reset(GameObject player)
    {
        yield return new WaitForSeconds(0.1f);
        player = null;
    }

    void CallCouroutines(GameObject other)
    {
        StartCoroutine("PreventOther");
        StartCoroutine("MoveOver", other);
    }
}
