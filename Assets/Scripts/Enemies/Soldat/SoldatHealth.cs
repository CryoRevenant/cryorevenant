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

        GetComponentInChildren<CircleCollider2D>().enabled = false;

        anim.runtimeAnimatorController = slowSpeed;

        ChangeColor(freezeColor);

        Invoke("NormalSpeed", 4f);
    }

    public void NormalSpeed()
    {
        anim.runtimeAnimatorController = normalSpeed;
        //ChangeColor(new Vector3(255, 255, 255));
    }

    private void FixedUpdate()
    {
        if (instance != null)
        {
            instance.transform.position = transform.position;
        }
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
        {
            Destroy(instance);
        }
    }

    void ChangeColor(Vector3 colorVector)
    {
        Debug.Log("new color = " + colorVector);
        Color newColor = new Color(colorVector.x, colorVector.y, colorVector.z);
        GetComponent<SpriteRenderer>().color = newColor;
    }
}
