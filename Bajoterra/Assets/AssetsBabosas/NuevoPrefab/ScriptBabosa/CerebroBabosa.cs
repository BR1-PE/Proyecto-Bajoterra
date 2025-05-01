using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class CerebroBabosa : MonoBehaviour
{
    private IModoBabosa modoAnimInfo;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        Debug.Log("Babosa instanciada");
    }

    private void Update()
    {
        modoAnimInfo?.Update();

    }

    public void CambiarModo(IModoBabosa nuevoModo)
    {
        modoAnimInfo?.SalirModo();    // Sal del modo anterior
        modoAnimInfo = nuevoModo;     // Asigna el nuevo modo
        modoAnimInfo.EntrarModo();    // Entra al nuevo modo
    }

    //---------------------------
    // VARIABLES
    //---------------------------
    public GameObject navBot;
    public GameObject neoBot;
    public NavMeshAgent agente;
    public Transform player;
    public bool playerCerca = false;
    public bool playerMuyCerca = false;
    public bool matarCoroutine = false;
    public bool ejecutadoBabosa = false;
    public bool ejecutado = false;
    public bool ejecutado1 = false;
    public bool ejeSalta = false;
    public float rango = 3f;
    public float vel0 = 0.0f;
    public float vel1 = 0.1f;
    public float vel2 = 1.0f;
    public float vel3 = 5.0f;
    public Rigidbody Rb;
    public Animator Anim;
    public AnimatorStateInfo AnimInfo;
    public bool AnimTran;
    public Vector3 fuerza = new Vector3(0, 5, 0);
    public Transform destino;
    public bool enMano = false;
    public Transform comida;
    public bool tiempo;

    //---------------------------
    // FUNCIONES
    //---------------------------
    public GameObject bot()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        NavMeshHit navMeshHit;
        if (NavMesh.Raycast(ray.origin, ray.direction * 0.1f, out navMeshHit, NavMesh.AllAreas))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            GameObject neoBot = Instantiate(navBot, transform.position, transform.rotation);
            transform.parent = neoBot.transform;
            transform.position = neoBot.transform.position;
            transform.rotation = neoBot.transform.rotation;
            agente = neoBot.GetComponent<NavMeshAgent>();
            StartCoroutine(espera());
            Debug.Log("Asignando NavMesh");
            return neoBot;
        }
        Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red);
        return null;
    }
    //Genera un bot de navegacion para el movimiento de la babosa......................................
    public void restablecerRuta()
    {
        agente.ResetPath();
    }
    public IEnumerator espera()
    {
        if (matarCoroutine)
        {
            yield break;
        }
        animar("Existiendo", 0.05f);
        float num = Random.Range(0, 6);
        agente.speed = vel0;
        yield return new WaitForSeconds(num);
        if (matarCoroutine)
        {
            yield break;
        }
        if (navBot != null && !matarCoroutine)
        {
            animar("Caminando", 0.05f);
            agente.speed = vel1;
            Vector3 randomDirection = Random.insideUnitSphere * rango;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, rango, NavMesh.AllAreas))
            {
                agente.SetDestination(hit.position);
            }
        }
        ejecutado = false;
    }
    //Hace que la babosa no se mueva durante un par de segundos y luego comienze a caminar.............
    public void corre()
    {
        agente.speed = vel2;
        if (player != null)
        {
            animar("Corriendo", 0.05f);
            Vector3 dirHuir = (transform.position - player.position).normalized * rango;
            agente.SetDestination(transform.position + dirHuir);
        }
    }
    //Hace que la babosa corra en direccion contraria al jugador.....................................
    public void huye()
    {
        agente.speed = vel3;
        Vector3 randomDirection = Random.onUnitSphere * rango * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, rango, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }
    //Hace que la babosa se mueva mas rapido y aleatoriamente........................................
    public void salta()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.1f, ~0, QueryTriggerInteraction.Ignore) && !ejeSalta)
        {
            animar("Subiendo", 0.05f);
            ejeSalta = true;
            Rb.velocity = new Vector3(0f, 0f, 0f);
            Rb.AddForce(fuerza, ForceMode.Impulse);
        }
        else if (ejeSalta && Rb.velocity.y < 1f && !Physics.Raycast(ray, out hit, 0.1f, ~0, QueryTriggerInteraction.Ignore))
        {
            animar("Cayendo", 0.3f);
            ejeSalta = false;
        }
        Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red);
    }
    //Hace que la babosa salte.......................................................................
    public float camino(Transform objetivo)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(agente.transform.position, objetivo.position, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                float recorrido = 0f;
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    recorrido += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }
                return recorrido;
            }
            else
            {
                return -1f;
            }
        }
        else
        {
            return -1f;
        }
    }
    //Mide el recorrido de una ruta supuesta verificando si es accesible o no........................
    public void ir(Transform objetivo, int vel)
    {
        switch (vel)
        {
            case 0: agente.speed = vel0; animar("Existiendo", 0.15f); break;
            case 1: agente.speed = vel1; animar("Caminando", 0.15f); break;
            case 2: agente.speed = vel2; animar("Corriendo", 0.15f); break;
            case 3: agente.speed = vel3; break;
            default: break;
        }
        agente.SetDestination(objetivo.position);
    }
    //Hace que la babosa vaya siga a un objetivo.....................................................
    public void destruir(bool yo, bool navMesh)
    {
        if (navMesh)
        {
            Destroy(neoBot);
        }
        if (yo)
        {
            Destroy(gameObject);
        }
    }
    //Destruye los objetos seleccionados.............................................................
    public void alternarRb(bool cambiar)
    {
        if (cambiar)
        {
            Rb.velocity = new Vector3(0f, 0f, 0f);
            Rb.useGravity = false;
            Rb.isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            Rb.useGravity = true;
            Rb.isKinematic = false;
            GetComponent<BoxCollider>().enabled = true;
        }
    }
    //Interpola los estados del rigidbody............................................................
    public void animar(string animacion, float t)
    {
        AnimInfo = Anim.GetCurrentAnimatorStateInfo(0);
        AnimTran = Anim.IsInTransition(0) && Anim.GetNextAnimatorStateInfo(0).IsName(animacion);

        if (!AnimInfo.IsName(animacion) && !AnimTran)
        {
            Anim.CrossFade(animacion, t);
        }
    }
    //Ejecuta una animacion solo si no se esta reproduciendo la misma animacion......................
    public IEnumerator temporizador(float t)
    {
        tiempo = false;
        yield return new WaitForSeconds(t);
        tiempo = true;
    }
    //Es un temporizador que vuelve positiva una variable en un tiempo determinado..................
}