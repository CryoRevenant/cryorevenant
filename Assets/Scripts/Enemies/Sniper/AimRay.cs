using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimRay : MonoBehaviour
{
    GameObject player;
    LineRenderer line;

    [SerializeField] GameObject head;

    bool isAiming;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        line = GetComponent<LineRenderer>();
        StopAim();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAiming)
        {
            Vector2 dir = player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
            head.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, player.transform.position);
        }
    }

    public void Aim()
    {
        AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

        for (int a = 0; a < audioS.Length; a++)
        {
            if (audioS[a].clip.name == "charge-laser")
            {
                if (!audioS[a].enabled)
                {
                    audioS[a].enabled = true;
                    FindObjectOfType<AudioManager>().Play("chargeLaser");
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("chargeLaser");
                }
            }
        }
        line.enabled = true;
        isAiming = true;
    }

    public void StopAim()
    {
        AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

        for (int a = 0; a < audioS.Length; a++)
        {
            if (audioS[a].clip.name == "charge-laser")
            {
                audioS[a].Stop();
            }
        }

        line.enabled = false;
        isAiming = false;
    }
}
