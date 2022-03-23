using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRay : MonoBehaviour
{
    [SerializeField] GameObject otherObj;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, otherObj.transform.position, Color.blue, 0.5f);
        Debug.DrawLine(transform.position, otherObj.transform.position, Color.red, 0.5f);
    }
}
