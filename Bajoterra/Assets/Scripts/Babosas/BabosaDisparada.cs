using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaEsDisparada : MonoBehaviour
{
    public bool babosaDisparada = false;
    public bool puedeTransformar = true;
    public float tiempoVuelo;
    bool generandoNavMesh = false;
    // Start is called before the first frame update
    bool choque = false;
    bool golpe = false;
    public GameObject transformacion;
    bool transformada = false;
    public GameObject Audio;
    GameObject reproductorAudio;
    GameObject babosaTransformada;
    bool ejecutarDestransformacion;

    void Start()
    {
        StartCoroutine(activarCaida());
        ejecutarDestransformacion = false;
    }

    void Update()
    {
        if(choque)
        {
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            choque = false;
        }
        if (babosaTransformada != null)
        {
            if (babosaTransformada.GetComponent<ElegirAtaque>().destransformar && !ejecutarDestransformacion)
            {
                destransformar();
                ejecutarDestransformacion = true;
                transformada = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        golpe = true;
        GetComponent<Animator>().SetBool("CayendoMal", true);
        if (babosaDisparada)
        {
            GetComponent<Rigidbody>().useGravity = true;

            ContactPoint contact = collision.contacts[0];
            float angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle < 45f)
            {
                GetComponent<Animator>().SetBool("TocandoPiso", true);
                GetComponent<Animator>().SetBool("Cayendo2", true);
                GetComponent<Animator>().SetBool("Saltando", false);
                choque = true;
                if(!generandoNavMesh)
                {
                    if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Aterrizaje2") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Cayendo2"))
                    {
                        GetComponent<Animator>().SetBool("CayendoMal", false);
                        GetComponent<Animator>().SetBool("Disparando", false);
                        generandoNavMesh = true;
                        babosaDisparada = false;
                        GetComponent<AsignarNavMesh>().enabled = true;
                        GetComponent<AsignarNavMesh>().CrearNavMesh();
                        this.enabled = false;
                    }
                    else
                    {
                        StartCoroutine(generarNavMesh());
                        generandoNavMesh = true;
                        babosaDisparada = false;
                    }
                }
            }
        }
    }

    IEnumerator activarCaida()
    {
        yield return new WaitForSeconds(0.2f);
        if(!golpe && puedeTransformar)
        {
            transformar();
        }
        else{
            yield return new WaitForSeconds(0.3f);
        }
        if (transformada)
        {
            yield return new WaitForSeconds(tiempoVuelo);
            if (!ejecutarDestransformacion)
            {
                destransformar();
                transformada = false;
            }
        }
        else
        {
            GetComponent<Rigidbody>().useGravity = true;
        }
        
    }

    IEnumerator generarNavMesh()
    {
        yield return new WaitForSeconds(10f);
        GetComponent<AsignarNavMesh>().enabled = true;
        GetComponent<AsignarNavMesh>().CrearNavMesh();
        this.enabled = false;
    }

    void transformar()
    {
        reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
        reproductorAudio.transform.SetParent(transform, true);
        reproductorAudio.GetComponent<ReproductorAudio>().transformar();
        reproductorAudio = null;

        GetComponent<BoxCollider>().enabled = false;
        babosaTransformada = Instantiate(transformacion, transform.position, transform.rotation);
        babosaTransformada.transform.SetParent(transform, true);
        babosaTransformada.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        babosaTransformada.transform.SetParent(null, true);
        transform.SetParent(babosaTransformada.transform, true);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;

        transformada = true;
    }

    void destransformar()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<Rigidbody>().velocity = babosaTransformada.GetComponent<Rigidbody>().velocity * 0.5f;
        transform.SetParent(null, true);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        Destroy(babosaTransformada);
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Animator>().Play("Cayendo2");
    }
}