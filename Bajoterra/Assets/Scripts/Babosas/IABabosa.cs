using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IABabosa : MonoBehaviour
{
    NavMeshAgent agent;
    public float rango = 5;
    public float quieto = 0.9f;
    public float posicion = 5;
    public float velocidad = 0.1f;
    public float distanciaEscape = 15;
    public Transform player;
    public bool esperando;

    private int numRandom;
    public float velocidadJugador;
    public float distanciaJugador;

    private List<Transform> objetosConLayer = new List<Transform>();

    // Estados
    [SerializeField] private bool tranquilo = true;
    [SerializeField] private bool preocupado = false;
    [SerializeField] private bool asustado = false;

    public int Estado;
    public bool Agarrado;
    bool Ejecutado = false;
    bool coroutineDomesticada = false;
    bool coroutineSalvaje = false;

    public bool hambre = false;
    public GameObject objetoComida;

    [SerializeField] private GameObject[] efectosDomesticar;

    private void Start()
    {
        hambre = false;
        esperando = false;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 0.1f;

        foreach (var Transform in objetosConLayer)
        {
            objetosConLayer.RemoveAt(0);
        }
        CargarObjetosConLayer();

        moverAlPunto();

        GetComponentInChildren<AgregarJugador>().enabled = true;
    }

    private void Update()
    {
        if (player != null){
            velocidadJugador = player.gameObject.GetComponent<ThirdPersonController>().velocidadPlayer;
            distanciaJugador = Vector3.Distance(transform.position, player.position);

            if (transform.childCount > 0)
            {
                Estado = GetComponentInChildren<Item>().Estado;
                Agarrado = GetComponentInChildren<PickableObject>().isPickable;

                if (!Ejecutado)
                {
                    if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Existiendo"))
                    {
                        Ejecutado = true;
                    }
                    else
                    {
                        Estado = -1;
                        velocidad = 0.0f;
                    }
                }
            }

            if (hambre && objetoComida != null && !asustado)
            {
                float distanciaComida = Vector3.Distance(transform.position, objetoComida.transform.position);

                if (distanciaComida < 10f)
                {
                    agent.SetDestination(objetoComida.transform.position);

                    if (GetComponentInChildren<Item>().Estado == 0)
                    {
                        velocidad = 0.1f;

                        if (distanciaJugador < 5f)
                        {
                            if (velocidadJugador > 4f)
                            {
                                hambre = false;
                                objetoComida = null;
                                StartCoroutine(salvajismo());
                            }
                        }
                    }
                    else if (GetComponentInChildren<Item>().Estado == 1)
                    {
                        velocidad = 1f;

                        if (distanciaJugador > 15f)
                        {
                            hambre = false;
                            objetoComida = null;
                        }
                    }

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!coroutineDomesticada)
                        {
                            StartCoroutine(domesticacion());
                            coroutineDomesticada = true;
                        }
                    }
                }
                else
                {
                    hambre = false;
                    objetoComida = null;
                }
            }
            else
            {
                hambre = false;
            }

            if (Estado == 0 && !hambre)
            {
                if (tranquilo)
                {
                    velocidad = 0.1f;

                    if (!esperando && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (Random.value < quieto)
                        {
                            StartCoroutine(esperaYMueve());
                        }
                        else
                        {
                            moverAlPunto();
                        }
                    }
                }
                if (preocupado && !asustado)
                {
                    tranquilo = false;

                    if (distanciaJugador < 5f)
                    {
                        Vector3 direccionHuir = (transform.position - player.position).normalized * posicion;
                        agent.SetDestination(transform.position + direccionHuir);
                        velocidad = 1f;
                    }
                    else
                    {
                        if (!esperando && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !asustado)
                        {
                            velocidad = 0.1f;
                            StartCoroutine(esperaYMueve());
                            preocupado = false;
                        }
                    }
                }
                if (asustado || !Agarrado)
                {
                    tranquilo = false;
                    preocupado = false;

                    if (distanciaJugador < 15f && agent.remainingDistance <= agent.stoppingDistance)
                    {
                        StartCoroutine(esperaYHuye());                
                        velocidad = 5f;
                    }
                    else
                    {
                        if (!esperando && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                        {
                            velocidad = 0.1f;
                            StartCoroutine(esperaYMueve());
                            asustado = false;
                        }
                        else
                        {
                            velocidad = 5f;
                        }
                    }
                }
                if (distanciaJugador < 5f)
                {
                    if (velocidadJugador < 0.0f){
                        tranquilo = true;    
                    }
                    else{
                        tranquilo = false;
                    }
                    

                    if (distanciaJugador < 2.5f){
                        if (velocidadJugador > 2.5f){
                            asustado = true;
                            preocupado = false;
                        }
                        else{
                            if (velocidadJugador > 1.5f){
                                preocupado = true;
                            }
                        }
                    }
                    else{
                        if (velocidadJugador > 1.5f){
                            preocupado = true;
                        }
                        else{
                            if (velocidadJugador < 0.1f && numRandom == 1 && !preocupado && !asustado){
                                preocupado = false;
                                asustado = false;
                            }
                        }
                    }
                }
                else if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    tranquilo = true;
                }
            }
            else if (Estado == 1 && !hambre)
            {
                if (distanciaJugador < 50f)
                {
                    agent.SetDestination(player.position);
                
                    if (distanciaJugador < 2.5f)
                    {
                        velocidad = 0f;
                    }
                    else
                    {
                        velocidad = 5f;
                    }
                }

                if (!coroutineSalvaje)
                {
                    StartCoroutine(salvajismo());
                    coroutineSalvaje = true;
                }
                
            }
            else if(Estado == 2)
            {
                agent.SetDestination(player.position);
                
                if (distanciaJugador < 2.5f)
                {
                    velocidad = 0f;
                }
                else
                {
                    velocidad = 5f;
                }
            }
        }

        if (agent.remainingDistance <= agent.stoppingDistance){
            esperando = false;
        }
        
        agent.speed = velocidad;
    }

    void moverAlPunto()
    {
        numRandom = Random.Range(0, 6);
        Vector3 randomDirection = Random.insideUnitSphere * rango;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, rango, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void alejarse()
    {
        Transform nearestTransform = FindNearestTransformWithLayer();

        if (nearestTransform != null)
        {
            float huida = Vector3.Distance(transform.position, nearestTransform.position);

            if (huida < distanciaEscape)
            {
                agent.SetDestination(nearestTransform.position);
                agent.speed = 5f;
                return;
            }
            else
            {
                Vector3 randomDirection = Random.insideUnitSphere * rango * 10;
                randomDirection += transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, rango, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }
    }

    IEnumerator esperaYMueve()
    {
        esperando = true;
        yield return new WaitForSeconds(3);
        moverAlPunto();
        esperando = false;
    }

    IEnumerator esperaYHuye()
    {
        alejarse();
        yield return new WaitForSeconds(1);
        esperando = false;
    }

    private void CargarObjetosConLayer()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("Escape"))
            {
                objetosConLayer.Add(obj.transform);
            }
        }
    }

    Transform FindNearestTransformWithLayer()
    {
        float closestDistance = Mathf.Infinity;
        Transform nearestTransform = null;

        foreach (Transform objTransform in objetosConLayer)
        {
            float distance = Vector3.Distance(transform.position, objTransform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestTransform = objTransform;
            }
        }
        return nearestTransform;
    }

    IEnumerator salvajismo()
    {
        yield return new WaitForSeconds(15);
        if (GetComponentInChildren<Item>().Estado == 1)
        {
            if (!hambre)
            {
                GetComponentInChildren<Item>().Estado = 0;

                GameObject cambiarDomesticado;
                cambiarDomesticado = Instantiate(efectosDomesticar[2], transform.position, transform.rotation);
                cambiarDomesticado.transform.SetParent(transform, true);
                cambiarDomesticado.SetActive(true);
                Destroy(cambiarDomesticado, cambiarDomesticado.GetComponent<ParticleSystem>().main.startLifetime.constantMax + 0.5f);    
            }
            coroutineSalvaje = false;   
        }
    }

    IEnumerator domesticacion()
    {
        if (objetoComida == player.gameObject.GetComponent<PickUpObject>().PickedObject){
            player.gameObject.GetComponent<PickUpObject>().soltar = true;
            objetoComida.GetComponent<PickableObject>().isPickable = false;
        }

        GameObject comiendo;
        comiendo = Instantiate(efectosDomesticar[3], transform.position, transform.rotation);
        comiendo.transform.SetParent(transform, true);
        comiendo.SetActive(true);

        yield return new WaitForSeconds(5);
        if (objetoComida != null)
        {
            if (hambre)
            {
                GameObject cambiarDomesticado;

                if (GetComponentInChildren<Item>().Estado == 0)
                {
                    GetComponentInChildren<Item>().Estado = 1;

                    cambiarDomesticado = Instantiate(efectosDomesticar[0], transform.position, transform.rotation);
                    cambiarDomesticado.transform.SetParent(transform, true);
                    cambiarDomesticado.SetActive(true);
                    Destroy(cambiarDomesticado, cambiarDomesticado.GetComponent<ParticleSystem>().main.startLifetime.constantMax + 0.5f);
                }
                else if (GetComponentInChildren<Item>().Estado == 1)
                {
                    GetComponentInChildren<Item>().Estado = 2;

                    cambiarDomesticado = Instantiate(efectosDomesticar[1], transform.position, transform.rotation);
                    cambiarDomesticado.transform.SetParent(transform, true);
                    cambiarDomesticado.SetActive(true);
                    Destroy(cambiarDomesticado, cambiarDomesticado.GetComponent<ParticleSystem>().main.startLifetime.constantMax + 0.5f);
                }

                //objetoComida.GetComponent<Hambreado>().despawnear();
                
                hambre = false;
                objetoComida = null;
            }
        }
        Destroy(comiendo, comiendo.GetComponent<ParticleSystem>().main.startLifetime.constantMax + 0.5f);
        coroutineDomesticada = false;
    }
}