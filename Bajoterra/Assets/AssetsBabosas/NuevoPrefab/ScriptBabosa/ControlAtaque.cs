using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAtaque : MonoBehaviour
{
    public GameObject babosa;
    public GameObject objetivo;
    public Vector3 direccionBabosa;
    public Vector3 direccionObjetivo;
    public float velocidad;
    public bool ataque = false;
    public MonoBehaviour script;

    void Update()
    {
        if (ataque)
        {
            script.enabled = true;
            this.enabled = false;
        }
    }
}
