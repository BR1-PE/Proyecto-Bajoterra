using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorComida : MonoBehaviour
{
    public GameObject babosa;
    public float tiempo;
    public float rango;
    public LayerMask capas;

    bool agarrado = false;
    bool ejecutado = false;

    // Update is called once per frame
    void Update()
    {
        agarrado = GetComponent<PickableObject>().isPickable;
        if (babosa == null && agarrado)
        {
            if (!ejecutado)
            {
                buscarBabosa();
                StartCoroutine(temporizador());
            }
        }
        if (!agarrado && babosa != null)
        {
            babosa.GetComponent<CerebroBabosa>().comida = null;
            babosa = null;
            ejecutado = false;
        }
    }

    void buscarBabosa()
    {
        float vel = GetComponent<Rigidbody>().velocity.magnitude;
        if (vel <= 0f)
        {
            Collider[] babosas = Physics.OverlapSphere(transform.position, rango, capas);
            if(babosas.Length > 0)
            {
                int a = Random.Range(0, babosas.Length);
                babosa = babosas[a].gameObject;
                babosa.GetComponent<CerebroBabosa>().comida = transform;
            }
        }
    }

    IEnumerator temporizador()
    {
        ejecutado = true;
        yield return new WaitForSeconds(tiempo);
        if (babosa == null)
        {
            ejecutado = false;
        }
    }
}
