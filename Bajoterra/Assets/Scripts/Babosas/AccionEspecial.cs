using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccionEspecial : MonoBehaviour
{
    public GameObject accion;
    GameObject ejecutarAccion;
    public GameObject realizarAccion()
    {
        if (accion != null)
        {
            ejecutarAccion = Instantiate(accion, transform.position + new Vector3(0, 0.2f, 0), transform.rotation);
            ejecutarAccion.transform.SetParent(transform, true);
            ejecutarAccion.SetActive(true);
            GetComponent<Animator>().Play("Transformando1-1");

            return ejecutarAccion;
        }
        else
        {
            return null;
        }
    }

    public void terminarAccion()
    {
        if (ejecutarAccion != null)
        {
            ejecutarAccion.GetComponent<ParticleSystem>().Stop();
            Destroy(ejecutarAccion, ejecutarAccion.GetComponent<ParticleSystem>().main.startLifetime.constantMax);
        }
    }
}
