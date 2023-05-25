using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehaviour : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    MeshRenderer meshRen;

    AudioClip sePunch, seWhistle;

    public enum Estado
    {
        Guardian, Bloque
    }
    public Estado estado = Estado.Guardian;

    float flVelocity = 30;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshRen = GetComponentInChildren<MeshRenderer>();

        sePunch = Resources.Load<AudioClip>("Audio/se/se_Punch");
        seWhistle = Resources.Load<AudioClip>("Audio/se/se_GuardWhistle");

        vPosicionarse();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (estado)
        {
            case Estado.Guardian:
                Quaternion quat = new Quaternion();
                quat = Quaternion.LookRotation(Vector3.Lerp(transform.forward, CarritoBehaviour.scr.transform.position - transform.position, 1f));
                quat.x = quat.z = 0;
                quat = quat.normalized;
                rb.MoveRotation(quat);

                rb.AddForce((transform.forward * flVelocity));

                if (transform.position.y > CarritoBehaviour.scr.transform.position.y)
                    rb.AddForce((-transform.up * 1000));

                rb.useGravity = true;
                rb.freezeRotation = true;
                anim.SetFloat("speed", 2);
                break;

            case Estado.Bloque:
                rb.useGravity = true;
                rb.freezeRotation = false;
                anim.SetFloat("speed", 0);
                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.position.y > transform.position.y)
        {
            switch (collision.gameObject.tag)
            {
                case "Guard":
                    break;

                default:
                    if ((estado == Estado.Guardian) && (meshRen.isVisible))
                    {
                        StartCoroutine(ienVolar());
                        StartCoroutine(ienLlamarMas());
                        GameManager.scr.PlaySE(sePunch, true);
                        estado = Estado.Bloque;

                        string path = "Models/Char/Textures/mater_GuardHit";
                        Material mater = Resources.Load<Material>(path);
                        meshRen.material = mater;
                    }
                    break;
            }
        }
    }

    void vPosicionarse()
    {
        Vector3 eleccion = CarritoBehaviour.scr.transform.position + new Vector3(Random.Range(-10f, 10f), 0, -10 - Random.Range(5f, 15f));
        transform.position = new Vector3(eleccion.x, transform.position.y, eleccion.z);
        estado = Estado.Guardian;
        string path = "Models/Char/Textures/mater_Guard";
        Material mater = Resources.Load<Material>(path);
        meshRen.material = mater;
        flVelocity = Random.Range(28f, 40f);
        //GameManager.scr.PlaySE(seWhistle, true);
    }

    IEnumerator ienVolar()
    {
        for (int i = 0; i < 5; i++)
        {
            rb.MoveRotation(Quaternion.LookRotation(Vector3.Lerp(transform.forward, transform.up, 0.15f)));
            rb.AddRelativeForce(0, 100, 0);
            yield return new WaitForSeconds(0.05f);
        }
        rb.AddForce(0, -1000, 0);
    }

    IEnumerator ienLlamarMas()
    {
        float espera = 0;
        if (Random.Range(0.1f, 3f) > 2f)
        {
            espera = Random.Range(5f, 15.5f);
            yield return new WaitForSeconds(espera);
            LevelManager.scr.vLlamarGuardia();
        }

        yield return new WaitForSeconds(8f);
        GameManager.scr.InstantiateParticles("GuardDie", transform.position);
        vPosicionarse();
    }
}
