using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daño : MonoBehaviour
{
    public float dañoInstantaneo;
    public float dañoContinuo;
    public bool aplicarDaño;
    private GameObject jugador;
    private bool ejecutado;
    void Start()
    {
        dañoContinuo = dañoInstantaneo * 0.2f;
        ejecutado = false;
        aplicarDaño = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (!ejecutado)
        {
            if (other.gameObject != null)
            {
                if (other.gameObject.GetComponent<Vida>() != null)
                {
                    jugador = other.gameObject;
                    jugador.GetComponent<Vida>().aplicarDañoInstantaneo(dañoInstantaneo);
                    jugador.GetComponent<Vida>().dañoPorSegundo += dañoContinuo;
                    aplicarDaño = true;
                    ejecutado = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != null)
        {
            if (other.gameObject.GetComponent<Vida>() != null)
            {
                jugador = other.gameObject;
                if (aplicarDaño)
                {
                    jugador.GetComponent<Vida>().dañoPorSegundo -= dañoContinuo;
                    aplicarDaño = false;
                    StartCoroutine(ejecutar());
                }
            }
        }
    }

    void OnDisable()
    {
        if (aplicarDaño)
        {
            if (jugador != null)
            {
                jugador.GetComponent<Vida>().dañoPorSegundo -= dañoContinuo;
            }
        }
    }

    public void dañoPorGuardar()
    {
        if (jugador != null)
        {
            jugador.GetComponent<Vida>().aplicarDañoInstantaneo(dañoInstantaneo);
        }
    }

    IEnumerator ejecutar()
    {
        yield return new WaitForSeconds(1);
        ejecutado = false;
    }
}