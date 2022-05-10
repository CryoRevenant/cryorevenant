using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 0)
        {
            other.gameObject.GetComponent<PlayerHP>().Death();
        }
        if (other.gameObject.layer == 0 || other.gameObject.layer == 6)
            Destroy(gameObject);
    }
}
