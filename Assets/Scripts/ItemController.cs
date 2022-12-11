using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemId id;
    public List<GameObject> listChar;

    private void Start()
    {
        foreach (GameObject o in listChar)
        {
            o.SetActive(false);
        }
        switch (id)
        {
            case ItemId.Item1:
                listChar[0].SetActive(true);
                break;
            case ItemId.Item2:
                listChar[1].SetActive(true);
                break;
            case ItemId.Item3:
                listChar[2].SetActive(true);
                break;
        }
    }
}

public enum ItemId
{
    Item1,
    Item2,
    Item3
}