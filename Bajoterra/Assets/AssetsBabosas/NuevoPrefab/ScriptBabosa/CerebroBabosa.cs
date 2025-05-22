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
    public bool playerDemasiadoCerca = false;
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
    public bool tiempo1;
    public Item item;
    public bool disparada;
    public bool arrojada;
    public bool contacto = false;
    public float anguloContacto;
    public Vector3 velocidadDisparo;
    public GameObject Velociforma;
    public GameObject Megaforma;
    public GameObject transformacion;
    public GameObject objetivo;
    public LayerMask mascaraObjetivo;
    public LayerMask mascaraObstaculo;
    public float distanciaObjetivo;
    public Vector3 direccionObjetivo;
    public int contadorAtaques = 0;
    public GameObject Apuntar;
    public GameObject Golpear;
    public GameObject Impactar;
    public GameObject Natural;
    public bool ataqueApuntar = true;
    public bool ataqueGolpear = true;
    public bool ataqueImpactar = true;
    public bool ataqueNatural = true;
    public float tiempoTransformacion;

    //---------------------------
    // FUNCIONES
    //---------------------------
    public void bot()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        NavMeshHit navMeshHit;
        if (NavMesh.Raycast(ray.origin, ray.direction * 0.1f, out navMeshHit, NavMesh.AllAreas))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            restringeTransform();
            neoBot = Instantiate(navBot, transform.position, transform.rotation);
            transform.position = neoBot.transform.position;
            transform.rotation = neoBot.transform.rotation;
            transform.SetParent(neoBot.transform, true);
            agente = neoBot.GetComponent<NavMeshAgent>();
            StartCoroutine(espera());
            Debug.Log("Asignando NavMesh");
        }
        Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red);
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
        if (neoBot != null && !matarCoroutine)
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
    public void destruir(bool yo, bool navMesh, bool trans)
    {
        if (navMesh && neoBot != null)
        {
            Destroy(neoBot);
        }
        if (yo)
        {
            Destroy(gameObject);
        }
        if (trans && transformacion != null)
        {
            Destroy(transformacion);
        }
    }
    //Destruye los objetos seleccionados.............................................................
    public void alternarRb(bool cambiar)
    {
        Rb = GetComponent<Rigidbody>();
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
    public void colocar(Transform padre, bool a, bool b)
    {
        transform.position = padre.position;
        transform.rotation = padre.rotation;
        if (a)
        {
            transform.SetParent(destino, true);
        }
        if (b)
        {
            destino.SetParent(transform, true);
        }
    }
    //Coloca a la babosa en la misma posicion y rotacion del objeto establecido......................
    public void animar(string animacion, float t)
    {
        Anim = GetComponent<Animator>();
        AnimInfo = Anim.GetCurrentAnimatorStateInfo(0);
        AnimTran = Anim.IsInTransition(0) && Anim.GetNextAnimatorStateInfo(0).IsName(animacion);

        if (!AnimInfo.IsName(animacion) && !AnimTran)
        {
            Anim.CrossFade(animacion, t);
        }
    }
    //Ejecuta una animacion solo si no se esta reproduciendo la misma animacion......................
    public bool finAnimar(string animacion)
    {
        Anim = GetComponent<Animator>();
        AnimInfo = Anim.GetCurrentAnimatorStateInfo(0);
        return AnimInfo.IsName(animacion) && AnimInfo.normalizedTime >= 1f;
    }
    //Indica si la animacion seleccionada finaliza...................................................
    public IEnumerator temporizador(float t, int i)
    {
        switch (i)
        {
            case 0:
                tiempo = false;
                yield return new WaitForSeconds(t);
                tiempo = true;
                break;
            case 1:
                tiempo1 = false;
                yield return new WaitForSeconds(t);
                Debug.Log("Fin Ataque");
                tiempo1 = true;
                break;
        }
    }
    //Es un temporizador que vuelve positiva una variable especifica en un tiempo determinado.......
    public int estadoItem(int? i)
    {
        item = GetComponent<Item>();

        if (i.HasValue)
        {
            item.Estado = i.Value;
            Debug.Log("cambiar estado a: " + i.Value);
        }

        return item.Estado;
    }
    //Cambia el valor de Estado del script Item de la babosa o devuelve el estado actual............
    public void darFuerza(Vector3 f, int g)
    {
        switch (g)
        {
            case 0: Rb.AddForce(transform.rotation*f, ForceMode.Force); break;
            case 1: Rb.AddForce(transform.rotation*f, ForceMode.Impulse); break;
            case 2: Rb.AddForce(transform.rotation*f, ForceMode.VelocityChange); break;
            case 3: Rb.AddForce(transform.rotation*f, ForceMode.Acceleration); break;
        }
    }
    //Agrega una fuerza instantanea en la direccion deseada.........................................
    public void detectarObjetivo(float radioVision, float anguloVision, float radioInicio)
    {
        objetivo = null;
        distanciaObjetivo = float.MaxValue;
        float mejorPuntaje = float.MinValue;
        Vector3 dirObjetivoSeleccionado = Vector3.zero;

        Collider[] candidatos = Physics.OverlapSphere(transform.position, radioVision, mascaraObjetivo);

        int puntosOrigen = 8;
        Vector3[] puntosInicio = new Vector3[puntosOrigen];
        Quaternion rotacion = Quaternion.LookRotation(transform.up, transform.forward);
        for (int i = 0; i < puntosOrigen; i++)
        {
            float angulo = i * Mathf.PI * 2f / puntosOrigen;
            Vector3 dirEnCirculo = new Vector3(Mathf.Cos(angulo), 0f, Mathf.Sin(angulo));
            Vector3 offset = rotacion * dirEnCirculo * radioInicio;
            puntosInicio[i] = transform.position + offset;
        }

        foreach (Collider col in candidatos)
        {
            foreach (Vector3 origen in puntosInicio)
            {
                Vector3 dirAlObjetivo = (col.transform.position - origen).normalized;
                float distancia = Vector3.Distance(origen, col.transform.position);
                float anguloRelativo = Vector3.Angle(transform.forward, dirAlObjetivo);

                if (anguloRelativo <= anguloVision / 2f)
                {
                    bool sinObstaculo = !Physics.Raycast(origen, dirAlObjetivo, distancia, mascaraObstaculo);
                    Color color = sinObstaculo ? Color.green : Color.red;
                    Debug.DrawRay(origen, dirAlObjetivo * distancia, color);

                    if (sinObstaculo)
                    {
                        float anguloNorm = 1f - (anguloRelativo / (anguloVision / 2f));
                        float distanciaNorm = 1f - (distancia / radioVision);
                        float puntaje = (anguloNorm * 0.2f) + (distanciaNorm * 0.8f);
                        puntaje += Random.Range(-0.05f, 0.05f);

                        if (puntaje > mejorPuntaje)
                        {
                            mejorPuntaje = puntaje;
                            distanciaObjetivo = distancia;
                            objetivo = col.gameObject;
                            dirObjetivoSeleccionado = dirAlObjetivo;
                        }
                    }
                }
            }
        }

        Vector3 derecha = Quaternion.Euler(0, anguloVision / 2f, 0) * transform.forward;
        Vector3 izquierda = Quaternion.Euler(0, -anguloVision / 2f, 0) * transform.forward;

        Debug.DrawLine(transform.position, transform.position + derecha * radioVision, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + izquierda * radioVision, Color.yellow);
        Debug.DrawRay(transform.position, transform.forward * radioVision, Color.cyan);
        for (int i = 0; i < puntosOrigen; i++)
        {
            int siguiente = (i + 1) % puntosOrigen;
            Debug.DrawLine(puntosInicio[i], puntosInicio[siguiente], Color.cyan);
        }
        if (objetivo != null)
        {
            Debug.DrawRay(transform.position, dirObjetivoSeleccionado * distanciaObjetivo, Color.blue);
        }
        direccionObjetivo = dirObjetivoSeleccionado;
    }
    //Detecta posibles objetivos para la babosa transformada........................................
    public GameObject instanciar(GameObject a, Vector3 b, Quaternion c)
    {
        return Instantiate(a, b, c);
    }
    //Instancia un GameObject segun los parametros indicados........................................
    public IEnumerator temporizadorAtaque(float t, int i)
    {
        switch (i)
        {
            case 0: ataqueApuntar = false; yield return new WaitForSeconds(t); ataqueApuntar = true; break;
            case 1: ataqueGolpear = false; yield return new WaitForSeconds(t); ataqueGolpear = true; break;
            case 2: ataqueImpactar = false; yield return new WaitForSeconds(t); ataqueImpactar = true; break;
            case 3: ataqueNatural = false; yield return new WaitForSeconds(t); ataqueNatural = true; break;
        }
    }
    //Establece un tiempo de espera antes de que la babosa vuelva a efectuar un ataque..............
    public void restringeTransform()
    {
        Rb = GetComponent<Rigidbody>();
        transform.localScale = new Vector3(1f, 1f, 1f);
        Rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    //Bloquea la posicion y rotacion de la babosa en el mundo.......................................
    public void liberaTransform()
    {
        Rb = GetComponent<Rigidbody>();
        transform.localScale = new Vector3(1f, 1f, 1f);
        Rb.constraints = RigidbodyConstraints.None;
    }
    //Libera la posicion y rotacion de la babosa en el mundo........................................
    public void cambiarTag(string tag)
    {
        gameObject.tag = tag;
    }
    //Cambia el tag de la babosa....................................................................
    public void pasarDatos(GameObject objeto)
    {
        ControlAtaque ataqueScript = objeto.GetComponent<ControlAtaque>();
        ataqueScript.babosa = gameObject;
        ataqueScript.objetivo = objetivo ?? null;
        ataqueScript.direccionBabosa = transform.forward;
        ataqueScript.direccionObjetivo = direccionObjetivo;
        ataqueScript.velocidad = velocidadDisparo.y;
        ataqueScript.ataque = true;
    }
    //Brinda toda la informacion necesaria para el ataque de la babosa..............................

    //---------------------------
    // METODOS
    //---------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (disparada)
        {
            ContactPoint contact = collision.contacts[0];
            anguloContacto = Vector3.Angle(contact.normal, Vector3.up);
            contacto = true;
        }
        if (arrojada)
        {
            ContactPoint contact = collision.contacts[0];
            anguloContacto = Vector3.Angle(contact.normal, Vector3.up);
            contacto = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (disparada)
        {
            contacto = false;
        }
        if (arrojada)
        {
            contacto = false;
        }
    }
}