using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarritoBehaviour : MonoBehaviour
{
    public static CarritoBehaviour scr;

    [SerializeField] MeshRenderer meshRenChar;
    Rigidbody rb;
    Animator anim;

    Vector3 v3Front, v3Avance;

    AudioClip seItemGet, seEstanteBreak;

    [SerializeField]
    float flMoveSpeed,
        flRotateSpeed;
    float flDistCarrito;

    float flX = 0, flY = 0;

    // Start is called before the first frame update
    void Start()
    {
        scr = this;

        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        seItemGet = Resources.Load<AudioClip>("Audio/se/se_ItemGet");
        seEstanteBreak = Resources.Load<AudioClip>("Audio/se/se_EstanteBreak");

        StartCoroutine(ienUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.scr.blCanMove)
        {
            flY = Input.GetAxis("Vertical");
            flX = Input.GetAxis("Horizontal");

            anim.SetFloat("speed", Mathf.Abs(flY));
        }
        else
        {
            flY = flX = 0;
        }

        v3Front = transform.forward * flY;

        v3Avance = (v3Front) * flMoveSpeed;

        Quaternion quat = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        if (Mathf.Abs(flX) >= 0.8f)
        {
            quat = Quaternion.LookRotation(Vector3.Lerp(transform.forward, transform.right * Mathf.Round(flX), flRotateSpeed * Mathf.Abs(flX) * Time.deltaTime));
            quat.x = quat.z = 0;

        }
        rb.MoveRotation(quat);

        //transform.forward = Vector3.Lerp(transform.forward, transform.right * Mathf.Round(flX), flRotateSpeed * Mathf.Abs(flX) * Time.deltaTime);
    }

    IEnumerator ienUpdate()
    {
        float timespan = 0.1f;

        while (true)
        {
            rb.AddForce((v3Avance + (Vector3.down * (1 + 10 * Mathf.Abs(rb.velocity.y)))) * 10);

            Vector3 velocidad = rb.velocity;
            float maxSpeed = 30;

            if (Mathf.Abs(velocidad.x) > maxSpeed)
            {
                velocidad.x = (Mathf.Abs(velocidad.x) / velocidad.x) * maxSpeed;
            }
            if (Mathf.Abs(velocidad.z) > maxSpeed)
            {
                velocidad.z = (Mathf.Abs(velocidad.z) / velocidad.z) * maxSpeed;
            }
            rb.velocity = velocidad;
            yield return new WaitForSeconds(timespan);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody body = collision.collider.attachedRigidbody;
        float pushForce = 4;

        if ((body != null) && (!body.isKinematic))
        {
            Vector3 punto = collision.contacts[0].point;
            foreach (var item in collision.contacts)
            {
                punto = Vector3.Lerp(punto, item.point, 0.5f);
            }
            Vector3 pushDir = punto - transform.position;

            body.velocity = pushForce * pushDir;
        }

        switch (collision.gameObject.tag)
        {
            case "Estante":
                EstanteAccion(collision.gameObject);
                if (Random.Range(0.1f, 5f) > 3f)
                {
                    GameManager.scr.PlaySE(Resources.Load<AudioClip>("Audio/se/se_Punch"), true);
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Estante":
                EstanteAccion(other.gameObject);
                if (Random.Range(0.1f, 5f) > 4.2f)
                {
                    GameManager.scr.PlaySE(seEstanteBreak, true);
                }
                break;

            default:
                break;
        }
    }

    void EstanteAccion(GameObject obEstante)
    {
        Estante scrEstante = obEstante.GetComponent<Estante>();

        bool esIgual = false;
        int valor = 0;
        foreach (var item in LevelManager.scr.intsRecoger)
        {
            if (scrEstante.itemNum == item)
            {
                esIgual = true;
                valor = scrEstante.itemNum;
            }
        }

        if (esIgual)
        {
            StartCoroutine(LevelManager.scr.ienActualizarObjetos(false, valor));
            GameManager.scr.InstantiateParticles("ItemGet", obEstante.transform.position);
            GameManager.scr.PlaySE(seItemGet);
        }
    } 

    public void vPose(bool won = true)
    {
        string which = (won) ? "Happy" : "Sad",
            path = string.Format("Models/Char/Textures/mater_Mother{0}", which);

        Material mater = Resources.Load<Material>(path);
        meshRenChar.material = mater;
        anim.speed += (won) ? 0.25f : -0.1f;
        anim.SetFloat("speed", (won) ? 2 : 0);

    }
}
