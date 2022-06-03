using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFalling : MonoBehaviour
{
    void FallSFX()
    {
        Debug.Log("boxFall");
        FindObjectOfType<AudioManager>().Play("boxFall");
    }
}
