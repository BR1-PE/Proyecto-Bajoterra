using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueGolpe : MonoBehaviour
{
    public GameObject Objetivo;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Personaje") || other.CompareTag("BabosaTransformada") || other.CompareTag("Destruible"))
        {
            Objetivo = other.gameObject;
        }
    }
}