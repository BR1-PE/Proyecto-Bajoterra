using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarBabosas : MonoBehaviour
{
    public bool triggerCerca = false;
    public bool triggerMuyCerca = false;
    public bool triggerDemasiadoCerca = false;
    public bool escondite = false;
    public bool comida = false;
    [SerializeField] private GameObject babcom;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<AgregarJugador>() != null)
        {
            if (other.gameObject.GetComponent<AgregarJugador>().enabled)
            {
                other.gameObject.GetComponent<AgregarJugador>().Jugador = transform.parent;
            }
        }
        if (triggerCerca)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (!other.gameObject.GetComponent<CerebroBabosa>().playerCerca)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().playerCerca = true;
                }
            }
        }
        if (triggerMuyCerca)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (!other.gameObject.GetComponent<CerebroBabosa>().playerMuyCerca)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().playerMuyCerca = true;
                }
            }
        }
        if (triggerDemasiadoCerca)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (!other.gameObject.GetComponent<CerebroBabosa>().playerDemasiadoCerca)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().playerDemasiadoCerca = true;
                }
            }
        }
        if (escondite)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (other.gameObject.GetComponent<CerebroBabosa>().destino == null)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().destino = transform;
                }
            }
        }
        if (comida)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (other.gameObject.GetComponent<CerebroBabosa>().comida == null)
                {
                    if (babcom == null)
                    {
                        other.gameObject.GetComponent<CerebroBabosa>().comida = transform;
                        babcom = other.gameObject;    
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (triggerCerca)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (other.gameObject.GetComponent<CerebroBabosa>().playerCerca)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().playerCerca = false;
                }
            }
        }
        if (triggerMuyCerca)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (other.gameObject.GetComponent<CerebroBabosa>().playerMuyCerca)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().playerMuyCerca = false;
                }
            }
        }
        if (triggerDemasiadoCerca)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (other.gameObject.GetComponent<CerebroBabosa>().playerDemasiadoCerca)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().playerDemasiadoCerca = false;
                }
            }
        }
        if (escondite)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (other.gameObject.GetComponent<CerebroBabosa>().destino != null)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().destino = null;
                }
            }
        }
        if (comida)
        {
            if (other.gameObject.GetComponent<CerebroBabosa>() != null)
            {
                if (other.gameObject == babcom)
                {
                    other.gameObject.GetComponent<CerebroBabosa>().comida = null;
                    babcom = null; 
                }
            }
        }
    }
}
