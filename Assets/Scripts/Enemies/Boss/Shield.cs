using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IceSpike>())
        {
            //Debug.Log("Hit by spike");
            gameObject.GetComponentInParent<BossMove>().slowness++;
            Destroy(collision.gameObject);
        }
    }
}
