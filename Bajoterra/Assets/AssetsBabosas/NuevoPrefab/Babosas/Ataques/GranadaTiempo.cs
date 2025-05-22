using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadaTiempo : MonoBehaviour
{
    [SerializeField] private int pesoAtaque;
    [SerializeField] private float velDispersion;
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float fuerzaExplosion;
    [SerializeField] private float daño;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private bool lanzador;
    [SerializeField] private GameObject[] minas;
    [SerializeField] private GameObject[] FX;
    [SerializeField] private AudioClip efecto;
    [SerializeField] private GameObject reproductorAudio;
    GameObject babosa;

    void Start()
    {
        if(lanzador){
            babosa = GetComponent<ControlAtaque>().babosa ?? GetComponentInParent<ControlAtaque>().babosa;
            babosa.GetComponent<CerebroBabosa>().contadorAtaques += pesoAtaque;
            StartCoroutine(babosa.GetComponent<CerebroBabosa>().temporizadorAtaque(0.1f, 3));

            foreach (GameObject explosivo in minas){
                explosivo.SetActive(true);
                explosivo.GetComponent<Rigidbody>().AddExplosionForce(velDispersion, transform.position, rangoExplosion, 0.5f, ForceMode.VelocityChange);
                explosivo.transform.SetParent(null, true);
            }
            foreach (GameObject efecto in FX){
            efecto.SetActive(true);
            }
            Destroy(gameObject, 1f);
        }
        else{
            StartCoroutine(destruirMina(tiempoEspera));
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Personaje")){
            foreach (GameObject efecto in FX){
                efecto.SetActive(true);
            }

            GetComponent<MeshRenderer>().enabled = false;
            GameObject audio = Instantiate(reproductorAudio, transform.position, transform.rotation);
            audio.GetComponent<ReproductorAudio>().sonido[3] = efecto;
            audio.GetComponent<ReproductorAudio>().sonidoExtra();

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
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        rb.AddExplosionForce(fuerzaExplosion, transform.position, rangoExplosion, 0.5f, ForceMode.Impulse);
                    }
                }
            }
            Destroy(gameObject, 3);
        }
    }
    
    IEnumerator destruirMina(float tiempo){
        yield return new WaitForSeconds(tiempo);

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;

        foreach (GameObject efecto in FX){
            efecto.SetActive(true);
        }

        GetComponent<MeshRenderer>().enabled = false;
        GameObject audio = Instantiate(reproductorAudio, transform.position, transform.rotation);
        audio.GetComponent<ReproductorAudio>().sonido[3] = efecto;
        audio.GetComponent<ReproductorAudio>().sonidoExtra();

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
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddExplosionForce(fuerzaExplosion, transform.position, rangoExplosion, 0.5f, ForceMode.Impulse);
                }
            }
        }
        Destroy(gameObject, 3);
    }
}