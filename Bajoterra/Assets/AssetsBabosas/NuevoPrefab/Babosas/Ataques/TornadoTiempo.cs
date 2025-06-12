using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoTiempo : MonoBehaviour
{
    [SerializeField] private int pesoAtaque;
    [SerializeField] private float radioTornado;
    [SerializeField] private float alturaTornado;
    [SerializeField] private float fuerzaAngular;
    [SerializeField] private float fuerzaArraste;
    [SerializeField] private float fuerzaElevacion;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private GameObject[] FX;
    [SerializeField] private AudioClip efecto;
    [SerializeField] private GameObject reproductorAudio;
    GameObject babosa;

    void Start()
    {
        babosa = GetComponent<ControlAtaque>().babosa;

        babosa.GetComponent<CerebroBabosa>().contadorAtaques += pesoAtaque;
        StartCoroutine(babosa.GetComponent<CerebroBabosa>().temporizadorAtaque(tiempoEspera, 2));
        StartCoroutine(acabar());

        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (babosa != null)
        {
            babosa.transform.position = transform.position;
        }
    }

    void FixedUpdate()
    {
        enTornado();
        altura();
    }

    public void enTornado()
    {
        Collider[] objetos = Physics.OverlapCapsule(transform.position - new Vector3(0f, 5f, 0f), transform.position + new Vector3(0f, alturaTornado + 5f, 0f), radioTornado);

        foreach (Collider col in objetos)
        {
            if (!col.CompareTag("Protoforma"))
            {
                if (col.gameObject.GetComponent<Rigidbody>() != null)
                {
                    if (col.gameObject != babosa)
                    {
                        fuerzaTornado(col.transform);
                    }
                }
            }
        }
    }

    private void fuerzaTornado(Transform objeto)
    {
        Vector3 direccionAngular = tangenteXZ(objeto);
        Vector3 direccionXZ = new Vector3(direccionAngular.x, 0f, direccionAngular.z);
        Vector3 direccionY = new Vector3(0f, direccionAngular.y, 0f);
        Vector3 direccionCentro = centroXZ(objeto);

        objeto.gameObject.GetComponent<Rigidbody>().AddForce(direccionXZ * fuerzaAngular, ForceMode.Force);
        objeto.gameObject.GetComponent<Rigidbody>().AddForce(direccionCentro * fuerzaArraste, ForceMode.Force);
        objeto.gameObject.GetComponent<Rigidbody>().AddForce(direccionY * fuerzaElevacion, ForceMode.Force);

    }

    private Vector3 tangenteXZ(Transform posicion)
    {
        float altura = posicion.position.y - transform.position.y;
        Vector3 normalizado = (posicion.position - transform.position).normalized;
        Vector3 tangente = new Vector3(-normalizado.z, alturaTornado - altura, normalizado.x);
        return tangente;
    }

    private Vector3 centroXZ(Transform posicion)
    {
        Vector3 normalizado = (posicion.position - transform.position).normalized;
        Vector3 sinAltura = new Vector3(normalizado.x, 0f, normalizado.z);
        float distancia = sinAltura.magnitude;
        Vector3 centro = sinAltura * (1 / Mathf.Max(distancia * distancia, 0.1f));
        return centro;
    }

    private void altura()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 5f);

        if (hits.Length > 0)
        {
            RaycastHit impactoMasLejano = hits[0];

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<Rigidbody>() == null)
                {
                    if (hit.distance > impactoMasLejano.distance)
                    {
                        impactoMasLejano = hit;
                    }
                }
            }

            float alturaObjetivo = impactoMasLejano.point.y + 4f;
            Vector3 posicionObjetivo = new Vector3(transform.position.x, alturaObjetivo, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, posicionObjetivo, Time.deltaTime * 1f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.up * 1f, Time.deltaTime * 1f);
        }
    }

    IEnumerator acabar()
    {
        yield return new WaitForSeconds(tiempoEspera);
        babosa.GetComponent<Rigidbody>().velocity = Vector3.zero;
        babosa = null;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Define los extremos de la cápsula
        Vector3 puntoInferior = transform.position - new Vector3(0f, 5f, 0f);
        Vector3 puntoSuperior = transform.position + new Vector3(0f, alturaTornado + 5f, 0f);

        // Color del Gizmo
        Gizmos.color = Color.cyan;

        // Dibuja la cápsula (en realidad, simularemos con esferas y líneas)
        Gizmos.DrawWireSphere(puntoInferior, radioTornado);
        Gizmos.DrawWireSphere(puntoSuperior, radioTornado);
        Gizmos.DrawLine(puntoInferior + Vector3.forward * radioTornado, puntoSuperior + Vector3.forward * radioTornado);
        Gizmos.DrawLine(puntoInferior - Vector3.forward * radioTornado, puntoSuperior - Vector3.forward * radioTornado);
        Gizmos.DrawLine(puntoInferior + Vector3.right * radioTornado, puntoSuperior + Vector3.right * radioTornado);
        Gizmos.DrawLine(puntoInferior - Vector3.right * radioTornado, puntoSuperior - Vector3.right * radioTornado);
    }

}