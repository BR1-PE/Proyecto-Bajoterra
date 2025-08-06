using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavegacionParedes : MonoBehaviour
{
    public float velocidad;
    public float cambioAltura;
    private Vector3 dir;
    public int x = 1;
    public float random;
    private bool rotar;

    void Update()
    {
        if (rotar) Rotar();
    }
    private bool DetectarFrente()
    {
        Ray rayo = new Ray(transform.position + transform.up * 0.2f, transform.forward);
        //Detecta si hay algo entre el punto de partida y el punto de llegada
        Debug.DrawRay(rayo.origin, rayo.direction * 0.3f, Color.red);
        if (Physics.Raycast(rayo, out RaycastHit hit, 0.3f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (!rotar)
            {
                float tiempo = Random.Range(1, 3);
                StartCoroutine(Rotacion(tiempo));
                rotar = true;
            }
            return true;
        }
        return false;
    }
    private bool DetectarPisoFrente()
    {
        //Divide la zona de búsqueda para buscar con mayor presición
        List<Vector3> puntos = new List<Vector3>();
        for (int i = 1; i <= 2; i++)
        {
            Vector3 a = transform.position + transform.up * 0.05f + transform.forward * 0.1f * i;
            Ray pts = new Ray(a, -transform.up);
            Debug.DrawRay(pts.origin, pts.direction * 0.2f, Color.yellow);
            //Detecta si no hay suelo
            if (!Physics.Raycast(pts, out RaycastHit hit, 0.2f, ~0, QueryTriggerInteraction.Ignore))
            {
                if (!rotar)
                {
                    float tiempo = Random.Range(1, 3);
                    StartCoroutine(Rotacion(tiempo));
                    rotar = true;
                }
                return true;
            }
        }
        return false;
    }
    public bool Avanzar()
    {
        if (!DetectarFrente())
        {
            if (!DetectarPisoFrente())
            {
                transform.position += transform.forward * Time.deltaTime * velocidad;
            }
        }
        return Altura();
    }
    private bool Altura()
    {
        Ray rayo = new Ray(transform.position + transform.up * 0.05f, -transform.up);
        Debug.DrawRay(rayo.origin, rayo.direction * 1f, Color.black, 0.1f);
        if (Physics.Raycast(rayo, out RaycastHit hit, 0.1f, ~0, QueryTriggerInteraction.Ignore))
        {
            float distancia = hit.distance;

            if (distancia < 0.065f)
            {
                transform.position += transform.up * Time.deltaTime * cambioAltura;
            }
            else if (distancia > 0.085f)
            {
                transform.position -= transform.up * Time.deltaTime * cambioAltura;
            }
            return true;
        }
        Debug.Log("No hay piso");
        return false;
    }
    private IEnumerator Rotacion(float t)
    {
        rotar = true;
        yield return new WaitForSeconds(t);
        rotar = false;
    }
    private void Rotar()
    {
        transform.Rotate(Vector3.up * random, Space.Self);
    }
}