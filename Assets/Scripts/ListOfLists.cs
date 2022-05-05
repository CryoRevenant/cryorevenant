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
}
