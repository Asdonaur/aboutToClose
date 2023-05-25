using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSprite : MonoBehaviour
{
    GameObject obCam;
    SpriteRenderer sprRen;

    [SerializeField] Color colorBlink;
    public int itemID;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        obCam = Camera.main.gameObject;
        sprRen = GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(0.2f);
        sprRen.sprite = LevelManager.scr.itemsSprite[itemID];

        float timespan = 0.2f;
        while (true)
        {
            transform.rotation = obCam.transform.rotation;
            yield return null;

            bool thisIs = false;
            foreach (var item in LevelManager.scr.intsRecoger)
            {
                if (item == itemID)
                {
                    thisIs = true;
                }
            }

            sprRen.color = (thisIs) ? ((sprRen.color == colorBlink) ? Color.white : colorBlink) : Color.white;

            yield return new WaitForSeconds(timespan);
        }
    }
}
