using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject[] TriggersExplosion;
    [SerializeField] private float rangoExplosion;
    [SerializeField] private float fuerzaExplosion;
    [SerializeField] private float daño;
    public GameObject NoColisionar;

    void Start()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
        
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
        Destroy(gameObject, 3);
    }
}