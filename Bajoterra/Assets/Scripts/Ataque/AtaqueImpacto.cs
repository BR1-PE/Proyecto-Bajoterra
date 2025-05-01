using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueImpacto : MonoBehaviour
{
    public GameObject Objetivo;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Personaje") || !other.gameObject.CompareTag("BabosaTransformada"))
        {
            Objetivo = other.gameObject;
        }
    }
}
