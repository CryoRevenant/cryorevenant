using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossCam : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera baseVCam;
    [SerializeField] private CinemachineVirtualCamera bossVCam;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("hit");
        if (collision.gameObject.CompareTag("Player"))
        {
            baseVCam.gameObject.SetActive(false);
            bossVCam.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            baseVCam.gameObject.SetActive(true);
            bossVCam.gameObject.SetActive(false);
        }
    }
}
