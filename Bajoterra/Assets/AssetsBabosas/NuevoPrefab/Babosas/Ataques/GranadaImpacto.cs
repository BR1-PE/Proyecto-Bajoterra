using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadaImpacto : MonoBehaviour
{
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float fuerzaExplosion;
    [SerializeField] private float daño;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private GameObject[] FX;
    [SerializeField] private AudioClip efecto;
    [SerializeField] private GameObject reproductorAudio;
    public GameObject NoColisionar;

    void Start()
    {
        StartCoroutine(tiempoExplotar(tiempoEspera));
    }

    IEnumerator tiempoExplotar(float tiempo){
        yield return new WaitForSeconds(tiempo);

        transform.SetParent(null, true);

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;

        foreach (GameObject efecto in FX){
            efecto.SetActive(true);
        }

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
                rb.AddExplosionForce(fuerzaExplosion, transform.position, rangoExplosion, 0f, ForceMode.Impulse);
            }
        }
        Destroy(gameObject, 3);
    }
}