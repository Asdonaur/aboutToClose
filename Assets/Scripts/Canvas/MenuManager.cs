using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void vBoton(string accion = "")
    {
        StartCoroutine(Boton(accion));
    }

    IEnumerator Boton(string act = "")
    {
        GameManager.scr.PlaySE(Resources.Load<AudioClip>("Audio/se/se_Clock"));
        yield return new WaitForSeconds(0.6f);
        switch (act)
        {
            case "start":
                GameManager.scr.LoadSceneTrans("SampleScene");
                break;

            case "screen":
                Screen.fullScreen = !Screen.fullScreen;
                break;

            case "credits":
                anim.SetInteger("pos", 1);
                break;

            case "credBack":
                anim.SetInteger("pos", 0);
                break;

            case "quit":
                Application.Quit();
                break;

            default:
                print("Me pulsaste! No pense que alguien me fuera a pulsar...");
                break;
        }
    }
}
