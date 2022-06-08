using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("M_volume");
    }

    public void UpdateMusic()
    {
        this.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("M_volume");
    }
}
