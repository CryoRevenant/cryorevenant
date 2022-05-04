using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSniper : EnemyDetect
{
    [SerializeField] float maxStoppingDetect;

    public override IEnumerator DetectAround()
    {
        Collider2D detectCircle = Physics2D.OverlapCircle(transform.position, radiusPlayer, 1 << 0);
        if (detectCircle != null)
        {
            RaycastHit2D hit;
            if (hit = Physics2D.Linecast(transform.position, detectCircle.gameObject.transform.position))
            {
                Debug.DrawLine(gameObject.transform.position, detectCircle.transform.position, Color.magenta, 0.5f);

                otherDetect = true;

                attack.BeginAttack();
                GetComponent<EnemyMove>().maxStoppingDist = maxStoppingDetect;

                move.StopCoroutine("MoveOver");
                move.StartCoroutine("MoveOver", detectCircle.gameObject);

                StartCoroutine("PreventOther");
            }
        }
        else
        {
            GetComponent<EnemyMove>().maxStoppingDist = 1;
            otherDetect = false;
        }
        yield return new WaitForSeconds(waitTime);
        StartCoroutine("DetectAround");
    }
}

