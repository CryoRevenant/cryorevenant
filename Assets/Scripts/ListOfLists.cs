using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListOfLists
{
    public List<GameObject> newIceList = new List<GameObject>();

    public void Add(GameObject newObject)
    {
        newIceList.Add(newObject);
    }

    public void Remove(GameObject newObject)
    {
        newIceList.Remove(newObject);
    }

    public int Count()
    {
        return newIceList.Count;
    }

    public bool CheckActive()
    {
        int i = 0;
        foreach (GameObject enemy in newIceList)
        {
            if (enemy.activeSelf)
            {
                i++;
            }
        }
        if (i == newIceList.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetActive(int index, bool state)
    {
        newIceList[index].SetActive(state);
    }
}
