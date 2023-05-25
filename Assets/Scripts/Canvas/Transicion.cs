using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transicion : MonoBehaviour
{
    public string escena = "a";
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        yield return new WaitForSeconds(0.55f);
        GameManager.scr.LoadScene(escena);
        yield return new WaitForSeconds(0.55f);
        Destroy(gameObject);
    }
}
