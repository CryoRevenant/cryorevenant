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
            int layerMask = ~LayerMask.GetMask("Box");
            if (hit = Physics2D.Raycast(transform.position, detectCircle.gameObject.transform.position - transform.position, radiusPlayer, layerMask))
            {
                Debug.DrawRay(gameObject.transform.position, detectCircle.gameObject.transform.position - transform.position, Color.magenta, 0.5f);
                if (hit.transform.gameObject.layer == 0)
                {
                    otherDetect = true;
                    attack.CheckAttack();
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

