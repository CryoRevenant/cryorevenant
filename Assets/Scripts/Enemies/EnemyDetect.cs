using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    [SerializeField] float radius;
    bool detect;
    public bool otherDetect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider2D detectCircle = Physics2D.OverlapCircle(transform.position, radius, layerMask: 3);
        if (detectCircle != null)
        {
            Debug.DrawLine(gameObject.transform.position, detectCircle.transform.position, Color.magenta, 0.5f);
            Debug.Log(detectCircle.transform.position);
            otherDetect = true;
            StartCoroutine("PreventOther");
        }
    }

    IEnumerator PreventOther()
    {
        Collider2D[] detectEnemy = Physics2D.OverlapCircleAll(transform.position, radius);
        if (detectEnemy != null)
        {
            foreach (Collider2D other in detectEnemy)
            {
                if (other.gameObject.GetComponent<EnemyDetect>() != null && other.gameObject.GetComponent<EnemyDetect>().otherDetect==false)
                {
                    Debug.DrawLine(transform.position, other.transform.position, Color.cyan, 1);
                    if (Physics2D.Linecast(transform.position, other.transform.position))
                    {
                        other.gameObject.GetComponent<EnemyDetect>().otherDetect = true;
                        other.gameObject.GetComponent<EnemyDetect>().StartCoroutine("PreventOther");
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
}
