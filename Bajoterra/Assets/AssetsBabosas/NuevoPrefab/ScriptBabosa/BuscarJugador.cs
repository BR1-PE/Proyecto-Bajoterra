using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuscarJugador : MonoBehaviour
{
    [SerializeField] private Transform camara;
    [SerializeField] private float intensidad;
    [SerializeField] private float frecuencia;
    [SerializeField] private float tiempo;

    private void Start(){
        GameObject cam = GameObject.Find("CamaraJugador");
        camara = cam.transform;
        Vector3 distancia = camara.position - transform.position;
        float intensidadDistancia;
        if (distancia.magnitude <= 3){
            intensidadDistancia = intensidad;
        }
        else if (distancia.magnitude >= intensidad){
            intensidadDistancia = 0;
        }
        else{
            intensidadDistancia = intensidad - distancia.magnitude;
        }
        TemblorCamara.Instance.MoverCamara(intensidadDistancia, frecuencia, tiempo);

        StartCoroutine(EscalarACero());
    }

    IEnumerator EscalarACero()
    {
        Vector3 escalaInicial = transform.localScale;
        float duracion = 0f;

        while (duracion < tiempo)
        {
            float t = duracion / tiempo;
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t);
            duracion += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }
}
