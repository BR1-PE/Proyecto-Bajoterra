using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElegirAtaque : MonoBehaviour
{
    public AtaqueDistancia AtaqueDistancia;
    public AtaqueGolpe AtaqueGolpe;
    public AtaqueImpacto AtaqueImpacto;
    [Header("Tiempo para destransformacion")]
    public float tiempoDistancia;
    public float tiempoGolpe;
    public float tiempoImpacto;
    public float tiempoTiempo;
    [Header("Tiempo antes que actue por inaccion")]
    public float esperaTiempo;
    [SerializeField] private Transform DistanciaTransform;
    [SerializeField] private Transform GolpeTransform;
    [SerializeField] private Transform ImpactoTransform;
    [SerializeField] private float fuerza;
    public GameObject Disparo;
    public GameObject Golpe;
    public GameObject Impacto;
    public GameObject Tiempo;
    public bool destransformar;
    bool atacando;

    void Start()
    {
        atacando = false;
        StartCoroutine(AtaqueTemporizado());
    }

    void Update()
    {
        if (!atacando)
        {
            if (AtaqueDistancia.Objetivo != null && Disparo != null)
            {
                AtaqueDisparar();
            }
            if(AtaqueGolpe.Objetivo != null && Golpe != null)
            {
                AtaqueGolpear();
            }
            if (AtaqueImpacto.Objetivo != null && Impacto != null)
            {
                AtaqueChocar();
            }
        }
    }

    public void AtaqueDisparar()
    {
        if (Disparo != null)
        {
            atacando = true;
            Vector3 direccion = (DistanciaTransform.position - ImpactoTransform.position).normalized;
            GameObject ObjetoDisparar = Instantiate(Disparo, GolpeTransform.position, transform.rotation);
            if (ObjetoDisparar.GetComponent<BolaExplosiva>().NoColisionar != null)
            {
                ObjetoDisparar.GetComponent<BolaExplosiva>().NoColisionar = gameObject;
            }
            ObjetoDisparar.SetActive(true);
            ObjetoDisparar.GetComponent<SphereCollider>().enabled = true;
            ObjetoDisparar.GetComponent<Rigidbody>().AddForce(direccion * fuerza, ForceMode.Impulse);
            StartCoroutine(desTransformar(tiempoDistancia));
        }
    }

    public void AtaqueGolpear()
    {
        if (Golpe != null)
        {
            atacando = true;
            Vector3 direccion = (DistanciaTransform.position - ImpactoTransform.position).normalized;
            GameObject ObjetoGolpear = Instantiate(Golpe, GolpeTransform.position, transform.rotation);
            ObjetoGolpear.transform.SetParent(transform, true);
            ObjetoGolpear.SetActive(true);
            Debug.Log("GOLPEANDO");
            StartCoroutine(desTransformar(tiempoGolpe));
        }
    }

    public void AtaqueChocar()
    {
        if (Impacto != null)
        {
            GameObject objetoImpactado = AtaqueImpacto.Objetivo;
            atacando = true;
            GameObject ObjetoImpactar = Instantiate(Impacto, GolpeTransform.position, transform.rotation);
            ObjetoImpactar.transform.SetParent(transform, true);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            //ObjetoImpactar.SetActive(true);
            transform.SetParent(objetoImpactado.transform, true);
            Debug.Log("IMPACTO");
            StartCoroutine(desTransformar(tiempoImpacto));
        }
    }
    
    IEnumerator AtaqueTemporizado()
    {
        if (Tiempo != null)
        {
            yield return new WaitForSeconds(esperaTiempo);
            if (!atacando)
            {
                atacando = true;
                GameObject ObjetoTiempo = Instantiate(Tiempo, GolpeTransform.position, transform.rotation);
                ObjetoTiempo.transform.SetParent(transform, true);
                ObjetoTiempo.SetActive(true);
                Debug.Log("TIEMPO");
                StartCoroutine(desTransformar(tiempoTiempo));
            }
        }
    }

    IEnumerator desTransformar(float a)
    {
        yield return new WaitForSeconds(a);
        destransformar = true;
    }
}