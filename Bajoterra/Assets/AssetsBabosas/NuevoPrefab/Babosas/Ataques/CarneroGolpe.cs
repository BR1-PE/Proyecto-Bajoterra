using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarneroGolpe : MonoBehaviour
{
    [SerializeField] private int pesoAtaque;
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float velocidadExtra;
    [SerializeField] private float daño;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private GameObject[] FX;
    [SerializeField] private AudioClip efecto;
    [SerializeField] private GameObject reproductorAudio;
    GameObject babosa;
    float velocidad;
    Vector3 direccionObjetivo;

    void Start()
    {
        babosa = GetComponent<ControlAtaque>().babosa;
        velocidad = GetComponent<ControlAtaque>().velocidad;
        direccionObjetivo = GetComponent<ControlAtaque>().direccionObjetivo;

        babosa.GetComponent<CerebroBabosa>().contadorAtaques += pesoAtaque;
        StartCoroutine(babosa.GetComponent<CerebroBabosa>().temporizadorAtaque(tiempoEspera, 1));
        StartCoroutine(golpear(tiempoEspera));
    }

    IEnumerator golpear(float tiempo){
        Ray ray = new Ray(transform.position, direccionObjetivo);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != babosa)
            {
                transform.position = hit.transform.position;
                break;
            }
        }
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, rangoExplosion);
        Vector3 aleatorioHorizontal = new Vector3(Random.Range(-1f, 1f), 0f, 0f).normalized;
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
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.AddForce((velocidad + velocidadExtra) * transform.forward, ForceMode.VelocityChange);
                    rb.AddForce(velocidadExtra * aleatorioHorizontal * 0.5f, ForceMode.VelocityChange);
                }
            }
        }
        foreach (GameObject efecto in FX){
            efecto.SetActive(true);
        }
        GameObject audio = Instantiate(reproductorAudio, transform.position, transform.rotation);
        audio.GetComponent<ReproductorAudio>().sonido[3] = efecto;
        audio.GetComponent<ReproductorAudio>().sonidoExtra();
        babosa = null;
        yield return new WaitForSeconds(tiempo);
        Destroy(gameObject, 1);
    }
}