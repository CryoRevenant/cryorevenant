using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider GV_Slider;
    [SerializeField] Slider MV_Slider;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("G_volume") && !PlayerPrefs.HasKey("M_volume"))
        {
            StartGeneralVolume(0.5f);
            StartMusicVolume(0.5f);
        }

        //Debug.Log("Reset");
        //Debug.Log(SceneManager.sceneCount);

        if (PlayerPrefs.HasKey("G_volume") && PlayerPrefs.HasKey("M_volume"))
        {
            GV_Slider.value = PlayerPrefs.GetFloat("G_volume");
            MV_Slider.value = PlayerPrefs.GetFloat("M_volume");
        }
    }

    private void Update()
    {
        //Debug.Log("MV_Slider.value = " + MV_Slider.value);
        //Debug.Log("PlayerPrefsMV.value = " + PlayerPrefs.GetFloat("M_volume"));
    }

    public void GeneralVolume(Slider slider)
    {
        PlayerPrefs.SetFloat("G_volume",slider.value);
        if (FindObjectOfType<AudioManager>())
        {
            foreach (AudioSource src in FindObjectsOfType<AudioSource>())
            {
                if (!src.GetComponent<Music>())
                {
                    src.volume = PlayerPrefs.GetFloat("G_volume");
                }
            }
        }
        //Debug.Log(PlayerPrefs.GetFloat("G_volume"));
    }

    public void MusicVolume(Slider slider)
    {
        PlayerPrefs.SetFloat("M_volume", slider.value);
        if (FindObjectOfType<Music>())
        {
            //Debug.Log("Music find");
            FindObjectOfType<Music>().UpdateMusic();
        }
        //Debug.Log(PlayerPrefs.GetFloat("M_volume"));
    }

    private void StartGeneralVolume(float value)
    {
        PlayerPrefs.SetFloat("G_volume", value);
        GV_Slider.value = value;
        if (FindObjectOfType<AudioManager>())
        {
            foreach (AudioSource src in FindObjectsOfType<AudioSource>())
            {
                if (!src.GetComponent<Music>())
                {
                    src.volume = PlayerPrefs.GetFloat("G_volume");
                }
            }
        }
    }

    private void StartMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("M_volume", value);
        MV_Slider.value = value;
        if (FindObjectOfType<Music>())
        {
            //Debug.Log("Music find");
            FindObjectOfType<Music>().UpdateMusic();
        }
    }
}
