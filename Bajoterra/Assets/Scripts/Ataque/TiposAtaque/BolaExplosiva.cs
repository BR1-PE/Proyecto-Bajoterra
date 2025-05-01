using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaExplosiva : MonoBehaviour
{
    [SerializeField] private GameObject[] TriggersVuelo;
    [SerializeField] private GameObject[] TriggersExplosion;
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float fuerzaExplosion;
    [SerializeField] private float daño;
    public GameObject NoColisionar;
    bool impacto;

    void Start()
    {
        impacto = false;
        foreach (GameObject trigger in TriggersVuelo)
        {
            trigger.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != NoColisionar)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            if (!impacto)
            {
                impacto	= true;
                foreach (GameObject particulas in TriggersVuelo)
                {
                    particulas.GetComponent<ParticleSystem>().Stop();
                }
                foreach (GameObject trigger in TriggersExplosion)
                {
                    trigger.SetActive(true);
                }

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
                        rb.AddExplosionForce(fuerzaExplosion, transform.position, rangoExplosion);
                    }
                }
                GetComponent<SphereCollider>().enabled = false;
                Destroy(gameObject, 3);
            }
        }
    }
}
