using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager scr;
    public AudioSource audSrcSE, audSrcBGM;

    public string strIdioma = "eng";
    public string[] strFrases;

    public bool blSound = true, blMusic = true;

    // EXCLUSIVO DE ESTE JUEGO
    float pitchy;


    // Start is called before the first frame update
    void Start()
    {
        if (scr)
        {
            Destroy(gameObject);
        }
        else
        {
            scr = this;
            DontDestroyOnLoad(gameObject);

            audSrcSE = GameObject.Find("audSrc_SE").GetComponent<AudioSource>();
            audSrcBGM = GameObject.Find("audSrc_BGM").GetComponent<AudioSource>();
            pitchy = audSrcBGM.pitch;

            blSound = PlayerPrefs.GetString("snd", "T") == "T" ? true : false;
            blMusic = PlayerPrefs.GetString("mus", "T") == "T" ? true : false;

            if (!(PlayerPrefs.HasKey("rec")))
            {
                BorrarDatos();
            }

        }
    }

    private void Update()
    {
        audSrcBGM.mute = !blMusic;
    }

    void BorrarDatos()
    {
        PlayerPrefs.SetInt("rec", 18000);
    }

    #region Cargar objetos
    public void InstantiateParticles(string cual, Vector3 position)
    {
        string path = string.Format("Prefabs/Particles/part_{0}", cual);
        GameObject particulas = Instantiate( Resources.Load<GameObject>(path) );
        particulas.transform.position = position;
    }
    #endregion

    #region Idioma
    /*
    public void LoadLanguage()
    {
        if (PlayerPrefs.HasKey("lang"))
        {
            strIdioma = PlayerPrefs.GetString("lang", "eng");
        }
        else
        {
            strIdioma = "eng";
        }

        string path = string.Format("Texts/{0}/general", strIdioma);

        TextAsset texto = Resources.Load(path) as TextAsset;
        string guion = texto.text;

        strFrases = guion.Split('\n');

        foreach (var item in GameObject.FindGameObjectsWithTag("FraseIdioma"))
        {
            FraseIdioma frase = item.GetComponent<FraseIdioma>();
            frase.Actualizar();
        }
    }

    public void UpdateLanguage()
    {
        strIdioma = (strIdioma == "eng") ? "esp" : "eng";
        PlayerPrefs.SetString("lang", strIdioma);
        LoadLanguage();
    }
    */
    #endregion

    #region Sonidos y musica
    // PlaySE()
    // randomPitch = Altera ligeramente el sonido de manera aleatoria
    // forced = El sonido se va a reproducir independientemente de que si está muteado o no
    public void PlaySE(AudioClip sonido, bool randomPitch = false, bool forced = false)
    {
        if (randomPitch)
        {
            audSrcSE.pitch = 1 + Random.Range(-1.5f, 1.5f);
        }
        else
        {
            audSrcSE.pitch = 1;
        }

        if (forced)
        {
            audSrcSE.PlayOneShot(sonido);
        }
        else
        {
            if (blSound)
            {
                audSrcSE.PlayOneShot(sonido);
            }
        }
        audSrcSE.pitch = 1;
    }

    public void PlayBGM(AudioClip musica)
    {
        audSrcBGM.Stop();
        audSrcBGM.clip = musica;
        audSrcBGM.Play();
    }

    public void StopBGM(bool repentino = true)
    {
        if (repentino)
        {
            audSrcBGM.Stop();
        }
        else
        {
            StartCoroutine(ienStopBGM());
        }
    }

    IEnumerator ienStopBGM()
    {
        float volumenAntes = audSrcBGM.volume;
        float factor = 0.1f;

        while (audSrcBGM.volume > 0)
        {
            audSrcBGM.volume -= factor;
            yield return new WaitForSeconds(0.05f);
        }
        audSrcBGM.Stop();
        audSrcBGM.volume = volumenAntes;
    }

    public void MusicPitch()
    {
        audSrcBGM.pitch = pitchy;
    }

    // NOTA: MODIFICAR DEPENDIENDO DEL JUEGO
    public void MusicaSegunEscena()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            default:
                break;
        }
    }

    #endregion

    #region Escenas

    public void LoadSceneTrans(string escena = "a")
    {
        MusicPitch();

        string esc = "e";
        if (escena == "a")
        {
            esc = SceneManager.GetActiveScene().name;
        }
        else
        {
            esc = escena;
        }


        Transicion trans = Instantiate(Resources.Load<GameObject>("Prefabs/transicion")).GetComponent<Transicion>();
        trans.escena = escena;
        
    }

    public void LoadScene(string escena = "a")
    {
        MusicPitch();

        string esc = "e";
        if (escena == "a")
        {
            esc = SceneManager.GetActiveScene().name;
        }
        else
        {
            esc = escena;
        }
        SceneManager.LoadScene(esc);

    }
    #endregion
}
