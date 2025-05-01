using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaTransformadaMovimiento : MonoBehaviour
{
    public Vector3 ajusteAngulo;
    public float rapidez;
    private Vector3 posicionAnterior;
    void Update()
    {   
        Vector3 desplazamiento = transform.position - posicionAnterior;

        Vector3 direccionMovimiento = desplazamiento.normalized;
        rapidez = desplazamiento.magnitude / Time.deltaTime;

        posicionAnterior = transform.position;
        
        if (direccionMovimiento != Vector3.zero)
        {
            // Calcula la rotación objetivo en el eje Y
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
            Quaternion ajuste = Quaternion.Euler(ajusteAngulo);
            rotacionObjetivo *= ajuste;

            // Interpola hacia la rotación objetivo
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionObjetivo, rapidez * Time.deltaTime);
        }
    }
}