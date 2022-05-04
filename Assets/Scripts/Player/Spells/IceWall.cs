using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    void Awake()
    {
        Destroy(gameObject, 3);
    }
}
