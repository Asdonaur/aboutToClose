using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCarrito : MonoBehaviour
{
    int ID = 0;
    Sprite sprObjeto;
    SpriteRenderer sprRen;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        sprRen = GetComponent<SpriteRenderer>();

        yield return null;
        ID = Random.Range(1, LevelManager.scr.itemsSprite.Length - 1);
        sprObjeto = LevelManager.scr.itemsSprite[ID];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool Got()
        {
            if (LevelManager.scr.itemsGot[ID])
            {
                bool nadie = true;

                foreach (var item in LevelManager.scr.intsRecoger)
                {
                    if (item == ID)
                    {
                        nadie = false;
                    }
                }

                return nadie;
            }
            else
            {
                return false;
            }
        }

        if (Got())
        {
            sprRen.sprite = sprObjeto;
        }
    }
}
