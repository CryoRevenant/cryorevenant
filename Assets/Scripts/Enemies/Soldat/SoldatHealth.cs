using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatHealth : EnemyHealth2
{
    [Header("ChangeSpeed")]
    [SerializeField] RuntimeAnimatorController normalSpeed;
    [SerializeField] RuntimeAnimatorController slowSpeed;
    [SerializeField] float timeFreezing;
    [SerializeField] GameObject iceSlowVFX;
    [SerializeField] Vector3 freezeColor;
    GameObject instance;

    bool canGoDown;
    [SerializeField] float timeBlocking;
    float timer;

    [Header("FX")]
    [SerializeField] GameObject vfxBlock;
    [SerializeField] GameObject vfxShield;

    private void Update()
    {
        //if (vfxBlockInstance != null)
        //{
        //    vfxBlockInstance.transform.position = this.transform.position;
        //}

        if (canGoDown)
        {
            timer -= Time.deltaTime;
        }

        if (timer < 0)
        {
            canGoDown = false;
            timer = timeBlocking;
            GetComponent<SoldatAttack>().mustBlock = false;
        }
    }

    public void Slowed()
    {
        if (instance != null)
        {
            Destroy(instance);
            instance = Instantiate(iceSlowVFX, transform.position, Quaternion.Euler(-90, 0, 0));
            instance.transform.parent = gameObject.transform;
            Destroy(instance, timeFreezing);
        }
        if (instance == null)
        {
            instance = Instantiate(iceSlowVFX, transform.position, Quaternion.Euler(-90, 0, 0));
            instance.transform.parent = gameObject.transform;
            Destroy(instance, timeFreezing);
        }

        GetComponent<HitTrigger>().trigger.enabled = false;

        anim.runtimeAnimatorController = slowSpeed;
        anim.SetTrigger("forceReco");

        ChangeColor(freezeColor);

        Invoke("NormalSpeed", timeFreezing);
    }

    public override void Block()
    {
        AudioSource[] audioS = FindObjectOfType<AudioManager>().gameObject.GetComponents<AudioSource>();

        for (int i = 0; i < audioS.Length; i++)
        {
            if (audioS[i].clip.name == "ice-sword")
            {
                audioS[i].Stop();
            }

            if (audioS[i].clip.name == "ice-sword2")
            {
                audioS[i].Stop();
            }

            if (audioS[i].clip.name == "ice-sword-damage")
            {
                audioS[i].Stop();
            }

            if (audioS[i].clip.name == "ice-sword-damage2")
            {
                audioS[i].Stop();
            }

            if (audioS[i].clip.name == "ice-sword-block")
            {
                audioS[i].Stop();
            }

            if (audioS[i].clip.name == "ice-sword-block2")
            {
                audioS[i].Stop();
            }
        }

        float random = Random.value;
        if (random <= 0.5f)
        {
            FindObjectOfType<AudioManager>().Play("iceSwordBlock");
        }
        else if (random > 0.5f)
        {
            FindObjectOfType<AudioManager>().Play("iceSwordBlock2");
        }

        base.Block();
        timer = timeBlocking;
        canGoDown = true;
        GetComponent<SoldatAttack>().StopCoroutine("PreAttack");
        GetComponent<SoldatAttack>().mustBlock = true;

        //Debug.Log(transform.rotation.y);
        switch (transform.rotation.y)
        {
            case 0:
                Debug.Log("Spawn Right");
                GameObject vfxBlockInstanceR = Instantiate(vfxBlock, transform.position, Quaternion.identity);
                vfxBlockInstanceR.GetComponent<SpriteRenderer>().flipX = false;
                Destroy(vfxBlockInstanceR, 0.25f);

                GameObject instanceR = Instantiate(vfxShield, transform.position, Quaternion.identity);
                instanceR.GetComponent<SpriteRenderer>().flipX = true;
                instanceR.transform.SetParent(transform);
                Destroy(instanceR, 0.25f);
                break;
            case 1:
                Debug.Log("Spawn Left");
                GameObject vfxBlockInstanceL = Instantiate(vfxBlock, transform.position, Quaternion.identity);
                vfxBlockInstanceL.GetComponent<SpriteRenderer>().flipX = true;
                Destroy(vfxBlockInstanceL, 0.25f);

                GameObject instanceL = Instantiate(vfxShield, transform.position, Quaternion.identity);
                instanceL.GetComponent<SpriteRenderer>().flipX = false;
                instanceL.transform.SetParent(transform);
                Destroy(instanceL, 0.25f);
                break;
        }
    }

    public void NormalSpeed()
    {
        anim.runtimeAnimatorController = normalSpeed;
        //ChangeColor(new Vector3(255, 255, 255));
    }

    void ChangeColor(Vector3 colorVector)
    {
        Color newColor = new Color(colorVector.x, colorVector.y, colorVector.z);
        GetComponent<SpriteRenderer>().color = newColor;
    }
}
