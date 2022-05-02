using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceBar : MonoBehaviour
{
    public float iceAmount;
    [SerializeField] Slider slide;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddBar(int amount)
    {
        iceAmount += amount;
        slide.value = iceAmount;

        if (iceAmount >= 100)
        {
            GetComponent<PlayerHP>().Death();
        }
    }

    public IEnumerator ResetBar()
    {
        while (iceAmount >= 0)
        {
            iceAmount -= 1 * speed;
            slide.value = iceAmount;

            yield return new WaitForSeconds(0.01f);
        }
        iceAmount = 0;
        StopCoroutine("ResetBar");
    }
}
