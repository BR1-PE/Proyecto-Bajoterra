using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroshockDisparo : MonoBehaviour
{
    [SerializeField] private int pesoAtaque;
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float velocidadExtra;
    [SerializeField] private float velocidadInstantanea;
    [SerializeField] private float daño;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private GameObject[] FX;
    [SerializeField] private AudioClip efecto;
    [SerializeField] private GameObject reproductorAudio;
    GameObject babosa;
    Vector3 direccionObjetivo;
    float velocidad;
    
    void Start()
    {
        babosa = GetComponent<ControlAtaque>().babosa;
        velocidad = GetComponent<ControlAtaque>().velocidad;
        direccionObjetivo = GetComponent<ControlAtaque>().direccionObjetivo;

        babosa.GetComponent<CerebroBabosa>().contadorAtaques += pesoAtaque;
        StartCoroutine(babosa.GetComponent<CerebroBabosa>().temporizadorAtaque(tiempoEspera, 0));

        transform.rotation = Quaternion.LookRotation(direccionObjetivo);
        GetComponent<Rigidbody>().AddForce((velocidad + velocidadExtra) * transform.forward, ForceMode.VelocityChange);

        ignorarColision();

        GetComponent<SphereCollider>().enabled = true;
    }

    void OnCollisionEnter(Collision other){
        if (other.gameObject != babosa){
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Collider[] colliders = Physics.OverlapSphere(transform.position, rangoExplosion);
            foreach (Collider objetosCercanos in colliders)
            {
                if (!objetosCercanos.CompareTag("Protoforma"))
                {
                    Rigidbody rb = objetosCercanos.GetComponent<Rigidbody>();
                    if (objetosCercanos.GetComponent<Vida>() != null)
                    {
                        objetosCercanos.GetComponent<Vida>().aplicarDañoInstantaneo(daño);
                    }
                    if (rb != null && objetosCercanos.gameObject != babosa)
                    {
                        rb.AddExplosionForce(velocidadInstantanea, transform.position, rangoExplosion, 0f, ForceMode.VelocityChange);
                    }
                }
            }
            foreach (GameObject efecto in FX){
                efecto.SetActive(true);
            }
            GameObject audio = Instantiate(reproductorAudio, transform.position, transform.rotation);
            audio.GetComponent<ReproductorAudio>().sonido[3] = efecto;
            audio.GetComponent<ReproductorAudio>().sonidoExtra();
            Destroy(gameObject, 1);
        }
    }

    void ignorarColision()
    {
        Collider[] colliders = GetComponents<Collider>();
        Collider[] collidersBabosa = babosa.GetComponents<Collider>();

        foreach (Collider miCol in colliders)
        {
            foreach (Collider colBabosa in collidersBabosa)
            {
                Physics.IgnoreCollision(miCol, colBabosa);
            }
        }
    }
}