using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider GV_Slider;
    [SerializeField] Slider MV_Slider;

    private void Awake()
    {
        GV_Slider.value = PlayerPrefs.GetFloat("G_volume");
        MV_Slider.value = PlayerPrefs.GetFloat("M_volume");
    }

    public void GeneralVolume(Slider slider)
    {
        PlayerPrefs.SetFloat("G_volume",slider.value);
        PlayerPrefs.SetFloat("M_volume", slider.value);
        //Debug.Log(PlayerPrefs.GetFloat("G_volume"));
        //Debug.Log(PlayerPrefs.GetFloat("M_volume"));
    }
}
