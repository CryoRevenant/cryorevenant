using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHeight : MonoBehaviour
{
    [SerializeField] private Transform offset;
    [SerializeField] private float newHeight;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("enter");
            offset.position = new Vector3(offset.position.x, offset.position.y - newHeight, offset.position.z);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("exit");
            offset.position = new Vector3(offset.position.x, offset.position.y + newHeight, offset.position.z);
        }
    }
}
