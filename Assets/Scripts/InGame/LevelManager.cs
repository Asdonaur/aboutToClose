using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager scr;
    [SerializeField] TextMeshProUGUI tmpTextoTiempo, tmpTextoItems;
    [SerializeField] bool ejecutar = true;

    LineRenderer line;
    CinemachineVirtualCamera vcam;

    AudioClip bgmRun, seClock, seWin, seLose;

    public Sprite[] itemsSprite;
    public bool[] itemsGot;

    public int[] intsRecoger = { 0, 0, 0 };
    [SerializeField] Image[] imgItems;
    [SerializeField] int inItemsRecogidos, inItemsMax = 24;

    [SerializeField] int inTime, inTimeMax = 20;

    int inGuardCount, inGuardMax = 60;

    int inSegundosEspera = 5;

    public bool blCanMove = true;
    bool resultado;

    string IntToTime(int index)
    {
        int seg = index, min = 0;
        while (seg >= 60)
        {
            seg -= 60;
            min++;
        }

        string str1 = ((min < 10) ? "0" : "") + min,
            str2 = ((seg < 10) ? "0" : "") + seg,
            resultado = str1 + ":" + str2;
        return resultado;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        scr = this;
        line = GetComponent<LineRenderer>();
        GameObject vcamobj = GameObject.Find("CM vcam1");

        if (vcamobj)
            vcam = vcamobj.GetComponent<CinemachineVirtualCamera>();

        List<bool> listBool = new List<bool>();
        foreach (var item in itemsSprite)
        {
            listBool.Add(false);
        }
        itemsGot = listBool.ToArray();

        bgmRun = Resources.Load<AudioClip>("Audio/bgm/bgm_RunAmok");
        seClock = Resources.Load<AudioClip>("Audio/se/se_Clock");
        seWin = Resources.Load<AudioClip>("Audio/se/se_FanfareWin");
        seLose = Resources.Load<AudioClip>("Audio/se/se_FanfareLose");

        yield return null;
        if (ejecutar)
        {
            StartCoroutine(ienTiempo());
            GameManager.scr.StopBGM();
            yield return new WaitForSeconds(inSegundosEspera);

            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(15f + Random.Range(0, 5) - (i * 2));
                vLlamarGuardia();
            }
        }
    }

    int CambiarObjeto()
    {
        int output = 0;
        do
        {
            output = Random.Range(1, itemsSprite.Length);
        } while (itemsGot[output]);
        return output;
    }

    public void vLlamarGuardia()
    {
        if ((inGuardCount < inGuardMax) && (!resultado))
        {
            GameObject guardia = Instantiate(Resources.Load<GameObject>("Prefabs/InGame/ob_Guard"));
            inGuardCount += 1;
        }
    }

    void Resultado(bool gano)
    {
        blCanMove = false;
        resultado = true;
        GameManager.scr.StopBGM();
        GameManager.scr.MusicPitch();

        StartCoroutine((gano) ? ienWon() : ienLost());
    }

    // todos = ¿Cambiar todos los objetos o solo el indicado?
    // ID = Si no son todos, la funcion busca el objeto con esa ID para cambiarlo
    public IEnumerator ienActualizarObjetos(bool todos = false, int ID = 0)
    {
        int num = 0;
        yield return null;
        if (todos)
        {
            for (int i = 0; i < 3; i++)
            {
                intsRecoger[num] = CambiarObjeto();
                imgItems[num].sprite = itemsSprite[intsRecoger[num]];
                itemsGot[intsRecoger[num]] = true;
                num++;
            }
        }
        else
        {
            bool found = false;
            foreach (var item in intsRecoger)
            {
                if (!found)
                {
                    if (item != ID)
                    {
                        num++;
                    }
                    else
                    {
                        found = true;
                    }
                }
            }

            if (num <= 2)
            {
                inItemsRecogidos++;

                if (inItemsRecogidos >= inItemsMax - intsRecoger.Length)
                {
                    intsRecoger[num] = 0;
                    imgItems[num].sprite = itemsSprite[0];
                    imgItems[num].color = new Color(0, 0, 0, 0);
                }
                else
                {
                    intsRecoger[num] = CambiarObjeto();
                    imgItems[num].sprite = itemsSprite[intsRecoger[num]];
                }

                itemsGot[intsRecoger[num]] = true;

                if (inItemsRecogidos == 5)
                {
                    vLlamarGuardia();
                }

                if (inItemsRecogidos == inItemsMax - 1)
                {
                    resultado = true;
                    print("SIIIII");
                }

                
            }

            float espaciado = 8;
            tmpTextoItems.text = inItemsRecogidos + " / " + (inItemsMax - 1);
            tmpTextoItems.characterSpacing += espaciado;
            yield return new WaitForSeconds(0.05f);
            tmpTextoItems.characterSpacing -= espaciado;
        }
    }

    IEnumerator ienTiempo()
    {
        CinemachineBasicMultiChannelPerlin cbmcp = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        tmpTextoTiempo.text = tmpTextoItems.text = "";
        for (int i = 0; i < inSegundosEspera; i++)
        {
            yield return new WaitForSeconds(1f);
            GameManager.scr.PlaySE(seClock);
        }
        StartCoroutine(ienActualizarObjetos(true));
        GameManager.scr.PlaySE(Resources.Load<AudioClip>("Audio/se/se_GuardWhistle"));
        tmpTextoItems.text = "0 / " + (inItemsMax - 1);

        inTime = inTimeMax;
        tmpTextoTiempo.text = IntToTime(inTime);

        float flT = 0f, flTM = 1f, flF = 0.2f;
        int shakeFactor = 1;

        yield return new WaitForSeconds(1f);
        GameManager.scr.PlayBGM(bgmRun);

        while ((inTime > 0) && (!resultado))
        {
            yield return new WaitForSecondsRealtime(flF);

            flT += flF;
            if (flT >= flTM)
            {
                flT = 0f;
                inTime--;
                tmpTextoTiempo.text = IntToTime(inTime);

                if (inTime < 30)
                {
                    GameManager.scr.audSrcBGM.pitch += 0.017f;
                    shakeFactor++;
                    cbmcp.m_AmplitudeGain += 0.005f * shakeFactor;
                }
            }
        }
        yield return new WaitForSeconds(1f);
        cbmcp.m_AmplitudeGain = 0f;
        Resultado(resultado);
    }

    IEnumerator ienLost()
    {
        GameManager.scr.PlaySE(Resources.Load<AudioClip>("Audio/se/se_GuardWhistle"));
        yield return new WaitForSeconds(0.2f);
        GameManager.scr.PlaySE(seLose);
        CarritoBehaviour.scr.vPose(false);
        yield return new WaitForSeconds(4f);
        GameManager.scr.LoadSceneTrans("MainMenu");
    }

    IEnumerator ienWon()
    {
        GameManager.scr.PlaySE(Resources.Load<AudioClip>("Audio/se/se_GuardWhistle"));
        yield return new WaitForSeconds(0.2f);
        GameManager.scr.PlaySE(seWin);
        CarritoBehaviour.scr.vPose();
        GameManager.scr.InstantiateParticles("Confeti", new Vector3(0, 11, 0));
        yield return new WaitForSeconds(4f);
        GameManager.scr.LoadSceneTrans("MainMenu");
    }
}
