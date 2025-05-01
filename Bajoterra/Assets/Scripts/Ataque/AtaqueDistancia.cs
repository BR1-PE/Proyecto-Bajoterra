using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueDistancia : MonoBehaviour
{
    public GameObject Objetivo;
    public Transform CentroObjeto;
    private bool apuntado;

    void Start()
    {
        apuntado = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!apuntado)
        {
            if (other.CompareTag("Personaje") || other.CompareTag("BabosaTransformada"))
            {
                Vector3 direccion = (other.transform.position - CentroObjeto.position).normalized;

                if (Physics.Raycast(CentroObjeto.position, direccion, out RaycastHit primerGolpe, 50f))
                {
                    if (primerGolpe.collider.gameObject == other.gameObject)
                    {
                        Objetivo = primerGolpe.collider.gameObject;
                        apuntado = true;
                    }
                }
            }
            else if (other.CompareTag("Destruible"))
            {
                Objetivo = other.gameObject;
            }
        }
    }

    /*void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Objetivo)
        {
            Objetivo = null;
            apuntado = false;
        }
    }*/
}