using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger : MonoBehaviour
{
    SoldatAttack parent;

    private void Start()
    {
        parent = GetComponentInParent<SoldatAttack>();
    }

    void Check()
    {
        parent.CheckAttack();
        parent.index++;
        Debug.Log("check anim");
    }

    void ResetAnim()
    {
        parent.Reset();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHP>().Death();
        }
    }
}
