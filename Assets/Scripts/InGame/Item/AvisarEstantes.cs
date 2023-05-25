using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvisarEstantes : MonoBehaviour
{
    public int ID = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in GetComponentsInChildren<Estante>())
        {
            item.itemNum = ID;
        }
    }
}
