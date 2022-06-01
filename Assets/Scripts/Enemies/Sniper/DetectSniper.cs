using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSniper : MonoBehaviour
{
    [SerializeField] float maxStoppingDetect;

    public float radiusPlayer;
    public bool otherDetect;
    public float waitTime;

    [Header("Scripts")]
    public EnemyAttack attack;

    private void Start()
    {
        StartCoroutine("DetectAround");
    }

    public IEnumerator DetectAround()
    {
        Collider2D detectCircle = Physics2D.OverlapCircle(transform.position, radiusPlayer, 1 << 0);
        if (detectCircle != null)
        {
            RaycastHit2D hit;
            if (hit = Physics2D.Linecast(transform.position, detectCircle.gameObject.transform.position))
            {
                Debug.DrawLine(gameObject.transform.position, detectCircle.transform.position, Color.magenta, 0.5f);

                otherDetect = true;
            }
        }
        else
        {
            otherDetect = false;
        }
        attack.CheckAttack();
        yield return new WaitForSeconds(waitTime);
        StartCoroutine("DetectAround");
    }
}

