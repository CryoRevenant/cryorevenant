using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetect : MonoBehaviour
{
    [Header("Détection")]
    public float radiusPlayer;
    public float radiusEnemy;
    public bool otherDetect;
    public float waitTime;
    public bool playerDetected;

    bool detect;

    [Header("Scripts")]
    public BossMove move;
    public BossAttack attack;

    [Header("PlayerRef")]
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        attack = GetComponent<BossAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<BossAttack>() != null && !attack.isPlayerNear)
        {
            attack.CheckAttack();
        }

        move.StopCoroutine("MoveOver");
        move.StartCoroutine("MoveOver", player);
    }

    public GameObject ReturnPlayerRef()
    {
        return player;
    }
}
