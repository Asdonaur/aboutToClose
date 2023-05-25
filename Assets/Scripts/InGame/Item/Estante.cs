using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estante : MonoBehaviour
{
    public int itemNum;

    // Start is called before the first frame update
    IEnumerator Start()
    {

        yield return null;
        foreach (var item in GetComponentsInChildren<ItemSprite>())
        {
            item.itemID = itemNum;
        }
    }
}
