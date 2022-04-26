using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ascenceur : MonoBehaviour
{
    List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] GameObject otherElevator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
        CheckOpen();
    }

    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }

    void CheckOpen()
    {
        if (enemyList.Count == 0)
        {
            Debug.Log(gameObject.name + " is open");
        }
    }
}
