using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineaObjeto : MonoBehaviour
{
    LineRenderer line;
    int numero = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        Estante estante = GetComponentInParent<Estante>();
        AvisarEstantes avisarE = GetComponentInParent<AvisarEstantes>();

        if (estante)
        {
            numero = estante.itemNum;
        }
        else if (avisarE)
        {
            numero = avisarE.ID;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool found = false;
        foreach (var item in LevelManager.scr.intsRecoger)
        {
            if (!found)
            {
                if (item == numero)
                {
                    found = true;
                }
            }
        }

        if (found)
        {
            line.SetPositions(new Vector3[] {
                new Vector3(transform.position.x, 1, transform.position.z),
                new Vector3(CarritoBehaviour.scr.transform.position.x, 1, CarritoBehaviour.scr.transform.position.z)
            });
        }
        else
        {
            {
                line.SetPositions(new Vector3[] {
                    new Vector3(transform.position.x, 1, transform.position.z),
                    new Vector3(transform.position.x, 1, transform.position.z)
                });
            }
        }
    }
}
