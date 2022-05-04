using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    [Header("Détection")]
    public float radiusPlayer;
    public float radiusEnemy;
    public bool otherDetect;
    public float waitTime;

    bool detect;

    [Header("Scripts")]
    public EnemyMove move;
    public EnemyAttack attack;

    // Start is called before the first frame update
    void Start()
    {
        move = GetComponent<EnemyMove>();
        attack = GetComponent<EnemyAttack>();
        StartCoroutine("DetectAround");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Si le joueur est détecté, alerte les ennemis à proximité et les fait venir
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

    //Détection du joueur, appel du déplacement vers lui, appel alerte autres ennemis et vérification de la distance pour attaquer
    public virtual IEnumerator DetectAround()
    {
        LayerMask layerMask = ~LayerMask.GetMask("Box");
        Collider2D detectCircle = Physics2D.OverlapCircle(transform.position, radiusPlayer, 1 << 0);
        if (detectCircle != null)
        {
            RaycastHit2D hit;
            if (hit = Physics2D.Linecast(transform.position, detectCircle.gameObject.transform.position, layerMask))
            {
                Debug.DrawLine(gameObject.transform.position, detectCircle.transform.position, Color.magenta, 0.5f);
                if (hit.transform.gameObject.layer == 0)
                {
                    otherDetect = true;

                    attack.CheckAttack();

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
        yield return new WaitForSeconds(waitTime);
        StartCoroutine("DetectAround");
    }
}
