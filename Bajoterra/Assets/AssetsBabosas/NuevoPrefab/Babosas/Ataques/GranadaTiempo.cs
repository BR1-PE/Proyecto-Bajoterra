using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadaTiempo : MonoBehaviour
{
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float fuerzaExplosion;
    [SerializeField] private float daño;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private bool lanzador;
    [SerializeField] private GameObject[] minas;
    [SerializeField] private GameObject[] FX;
    [SerializeField] private AudioClip efecto;
    [SerializeField] private GameObject reproductorAudio;
    public GameObject NoColisionar;

    void Start()
    {
        if(lanzador){
            transform.parent.gameObject.GetComponentInParent<Rigidbody>().isKinematic = false;
            transform.parent.gameObject.GetComponentInParent<Rigidbody>().useGravity = true;
            foreach (GameObject explosivo in minas){
                explosivo.SetActive(true);
                explosivo.GetComponent<Rigidbody>().AddForce(explosivo.transform.up * fuerzaExplosion);
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
                Rigidbody rb = objetosCercanos.GetComponent<Rigidbody>();
                if (rb != null && objetosCercanos.gameObject != NoColisionar)
                {
                    if (objetosCercanos.GetComponent<Vida>() != null)
                    {
                        objetosCercanos.GetComponent<Vida>().aplicarDañoInstantaneo(daño);
                    }
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddExplosionForce(fuerzaExplosion, transform.position, rangoExplosion, 0.5f, ForceMode.Impulse);
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
            Rigidbody rb = objetosCercanos.GetComponent<Rigidbody>();
            if (rb != null && objetosCercanos.gameObject != NoColisionar)
            {
                if (objetosCercanos.GetComponent<Vida>() != null)
                {
                    objetosCercanos.GetComponent<Vida>().aplicarDañoInstantaneo(daño);
                }
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddExplosionForce(fuerzaExplosion, transform.position, rangoExplosion, 0.5f, ForceMode.Impulse);
            }
        }
        Destroy(gameObject, 3);
    }
}