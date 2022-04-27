using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHeight : MonoBehaviour
{
    [SerializeField] private float yOffset;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControllerV2>().ynewOffset = yOffset;
            StartCoroutine(collision.gameObject.GetComponent<PlayerControllerV2>().MoveCamUp());
        }
    }
}
