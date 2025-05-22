using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadaImpacto : MonoBehaviour
{
    [SerializeField] private int pesoAtaque;
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float fuerzaExplosion;
    [SerializeField] private float daño;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private GameObject[] FX;
    [SerializeField] private AudioClip efecto;
    [SerializeField] private GameObject reproductorAudio;
    GameObject babosa;
    GameObject objetivo;

    void Start()
    {
        babosa = GetComponent<ControlAtaque>().babosa;
        objetivo = GetComponent<ControlAtaque>().objetivo;

        babosa.GetComponent<CerebroBabosa>().contadorAtaques += pesoAtaque;
        StartCoroutine(babosa.GetComponent<CerebroBabosa>().temporizadorAtaque(tiempoEspera, 2));
        transform.SetParent(objetivo.transform, true);

        StartCoroutine(tiempoExplotar(tiempoEspera));
    }
    void Update()
    {
        if (babosa != null)
        {
            babosa.transform.position = transform.position;
            babosa.transform.rotation = transform.rotation;            
        }
    }

    IEnumerator tiempoExplotar(float tiempo){
        yield return new WaitForSeconds(tiempo);

        transform.SetParent(null, true);

        foreach (GameObject efecto in FX){
            efecto.SetActive(true);
        }

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
                if (rb != null && objetosCercanos.gameObject != babosa)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddExplosionForce(fuerzaExplosion, transform.position, rangoExplosion, 0f, ForceMode.Impulse);
                }
            }
        }
        babosa.GetComponent<CerebroBabosa>().cambiarTag("Protoforma");
        babosa = null;
        Destroy(gameObject, 3);
    }
}