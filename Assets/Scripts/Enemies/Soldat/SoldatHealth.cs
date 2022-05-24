using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldatHealth : EnemyHealth
{
    [Header("ChangeSpeed")]
    [SerializeField] RuntimeAnimatorController normalSpeed;
    [SerializeField] RuntimeAnimatorController slowSpeed;
    [SerializeField] GameObject iceSlowVFX;
    [SerializeField] Vector3 freezeColor;
    GameObject instance;
    bool canGoDown;
    [SerializeField] float timeBlocking;
    public float timer;

    private void Update()
    {
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
            Destroy(instance, 4f);
        }
        if (instance == null)
        {
            instance = Instantiate(iceSlowVFX, transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(instance, 4f);
        }

        GetComponentInChildren<BoxCollider2D>().enabled = false;

        anim.runtimeAnimatorController = slowSpeed;

        ChangeColor(freezeColor);

        Invoke("NormalSpeed", 4f);
    }

    public override void Block()
    {
        base.Block();
        timer = timeBlocking;
        canGoDown = true;
        GetComponent<SoldatAttack>().StopCoroutine("PreAttack");
        GetComponent<SoldatAttack>().mustBlock = true;
    }

    public void NormalSpeed()
    {
        anim.runtimeAnimatorController = normalSpeed;
        //ChangeColor(new Vector3(255, 255, 255));
    }

    // private void FixedUpdate()
    // {
    //     if (instance != null)
    //     {
    //         instance.transform.position = transform.position;
    //     }
    // }

    // private void Update()
    // {
    //     if (!gameObject.activeSelf)
    //     {
    //         Destroy(instance);
    //     }
    // }

    void ChangeColor(Vector3 colorVector)
    {
        Color newColor = new Color(colorVector.x, colorVector.y, colorVector.z);
        GetComponent<SpriteRenderer>().color = newColor;
    }
}
