using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsferaEmpujadora : MonoBehaviour
{
    public float fuerzaImpacto = 10f; // Cantidad de fuerza que la esfera aplicará en la colisión

    void OnCollisionEnter(Collision collision)
    {
        // Recorre todos los puntos de contacto de la colisión
        foreach (ContactPoint contacto in collision.contacts)
        {
            // Aplica una fuerza al objeto colisionado en la dirección de la normal del punto de contacto
            if (collision.rigidbody != null) // Asegura que el objeto tiene un Rigidbody
            {
                collision.rigidbody.AddForce(contacto.normal * fuerzaImpacto, ForceMode.Impulse);
            }
        }
    }
}
