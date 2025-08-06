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
        Col = GetComponents<Collider>();
        Cap = GetComponent<CapsuleCollider>();
        Anim = GetComponent<Animator>();
        Item = GetComponent<Item>();
        Pick = GetComponent<PickableObject>();
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
    /// <summary>Transform del jugador.</summary>
    public Transform player;
    /// <summary>Prefab del bot de navegación por navMesh.</summary>
    public GameObject navMeshBot;
    /// <summary>Prefab del bot de navegación.</summary>
    public GameObject navBot;
    /// <summary>Instancia del bot de navegación.</summary>
    public GameObject neoBot;
    /// <summary>Agente de navegación del bot de navegación por NavMesh.
    /// Vea <see cref="navMeshBot"/></summary>
    public NavMeshAgent navMeshAgente;
    /// <summary>Agente de navegación del bot de navegación.
    /// Vea <see cref="navBot"/></summary>
    public NavegacionParedes navAgente;
    /// <summary>Indica poca cercanía al jugador.
    /// Vea <see cref="playerMuyCerca"/> y <see cref="playerDemasiadoCerca"/>.</summary>
    public bool playerCerca = false;
    /// <summary>Indica cercanía al jugador.
    /// Vea <see cref="playerCerca"/> y <see cref="playerDemasiadoCerca"/>.</summary>
    public bool playerMuyCerca = false;
    /// <summary>Indica mucha cercanía al jugador.
    /// Vea <see cref="playerCerca"/> y <see cref="playerMuyCerca"/>.</summary>
    public bool playerDemasiadoCerca = false;
    /// <summary>Velocidad neutra.</summary>
    private readonly float vel0 = 0.0f;
    /// <summary>Velocidad base, despacio.</summary>
    private readonly float vel1 = 0.1f;
    /// <summary>Velocidad simple, mediana.</summary>
    private readonly float vel2 = 1.0f;
    /// <summary>Velocidad rápida, acelerada.</summary>
    private readonly float vel3 = 5.0f;
    /// <summary>Rigidbody de la babosa.</summary>
    public Rigidbody Rb;
    /// <summary>Colliders de la babosa.</summary>
    public Collider[] Col;
    /// <summary>Capsule Collider de la babosa.</summary>
    public CapsuleCollider Cap;
    /// <summary>Animator de la babosa.</summary>
    public Animator Anim;
    /// <summary>Item de la babosa.</summary>
    public Item Item;
    /// <summary>PickableObject de la babosa.</summary>
    public PickableObject Pick;
    /// <summary>Transform del escondite.</summary>
    public Transform escondite;
    /// <summary>Transform de la comida.</summary>
    public Transform comida;
    /// <summary>Indica si esta agarrada en mano.</summary>
    public bool sujetada;
    /// <summary>Indica si salió del arma sin ser disparada.</summary>
    public bool soltada;
    /// <summary>Indica si fue disparada.</summary>
    public bool disparada;
    /// <summary>Indica si se puede transformar.</summary>
    public bool transformar;
    /// <summary>Punto de contacto.</summary>
    public Vector3 contacto;
    /// <summary>Angulo de rotacion.</summary>
    public Quaternion rotacion;
    /// <summary>Dirección de la normal de un punto de contacto.</summary>
    public Vector3 normal;
    /// <summary>Velocidad a la que fue disparada.</summary>
    public float velocidadDisparo;
    /// <summary>Prefab de la transformación de la babosa.</summary>
    public GameObject velociforma;
    /// <summary>Prefab de la mega transformación de la babosa.</summary>
    public GameObject megaforma;
    /// <summary>Transformación actual de la babosa.</summary>
    public GameObject transformacion;
    /// <summary>Cantidad de ataques disponibles de la babosa.</summary>
    private int contadorAtaques = 10;
    public GameObject objetivo;
    public LayerMask mascaraObjetivo;
    public LayerMask mascaraObstaculo;
    public float distanciaObjetivo;
    public Vector3 direccionObjetivo;
    public GameObject Apuntar;
    public GameObject Golpear;
    public GameObject Impactar;
    public GameObject Natural;
    public bool ataqueApuntar = true;
    public bool ataqueGolpear = true;
    public bool ataqueImpactar = true;
    public bool ataqueNatural = true;
    public Vector3[] a;
    public float tiempoTransformacion;

    /// <summary>
    /// Busca posiciones válidas para generar el bot de movimiento (piso)
    /// </summary>
    public IEnumerator GenerarBot()
    {
        while (neoBot == null)
        {
            yield return null;
            if (ComprobarNavMesh())
            {
                RestringeMovimiento();
                Rb.velocity = Vector3.zero;
                neoBot = Instantiate(navMeshBot, transform.position, transform.rotation);
                transform.position = neoBot.transform.position;
                transform.rotation = neoBot.transform.rotation;
                transform.SetParent(neoBot.transform, true);
                navMeshAgente = neoBot.GetComponent<NavMeshAgent>();
                yield break;
            }
            else if (ComprobarSuperficie())
            {
                Rb.constraints = RigidbodyConstraints.FreezeAll;
                Rb.velocity = Vector3.zero;
                neoBot = Instantiate(navBot, contacto, rotacion);
                transform.position = neoBot.transform.position;
                transform.rotation = neoBot.transform.rotation;
                transform.SetParent(neoBot.transform, true);
                navAgente = neoBot.GetComponent<NavegacionParedes>();
            }
        }
    }
    /// <summary>
    /// Verifica la existencia de un NavMesh cercano
    /// </summary>
    /// <returns>Devuelve verdadero si encuentra el area</returns>
    private bool ComprobarNavMesh()
    {
        // Inicializa rayos en todas las direcciones
        Ray[] rayos = {
            new Ray(transform.TransformPoint(a[0]), transform.up),
            new Ray(transform.TransformPoint(a[1]), -transform.up),
            new Ray(transform.TransformPoint(a[2]), transform.forward),
            new Ray(transform.TransformPoint(a[3]), -transform.forward),
            new Ray(transform.TransformPoint(a[4]), transform.right),
            new Ray(transform.TransformPoint(a[5]), -transform.right)
        };
        foreach (Ray ray in rayos)
        {
            // Dibuja los rayos
            Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red, 0.1f);

            if (NavMesh.Raycast(ray.origin, ray.direction * 0.1f, out NavMeshHit navMeshHit, NavMesh.AllAreas))
            {
                return true; // Entró en contacto con un NavMesh
            }
        }
        return false; // No encontró nada
    }
    /// <summary>
    /// Verifica la existencia de un terreno distinto a NavMesh
    /// </summary>
    /// <returns>Devuelve verdadero si encuentra el area y no tiene un NavMesh</returns>
    public bool ComprobarSuperficie()
    {
        Ray[] rayos = {
            new Ray(transform.TransformPoint(a[0]), transform.up),
            new Ray(transform.TransformPoint(a[1]), -transform.up),
            new Ray(transform.TransformPoint(a[2]), transform.forward),
            new Ray(transform.TransformPoint(a[3]), -transform.forward),
            new Ray(transform.TransformPoint(a[4]), transform.right),
            new Ray(transform.TransformPoint(a[5]), -transform.right)
        };
        foreach (Ray ray in rayos)
        {
            Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red, 0.1f);
            if (Physics.Raycast(ray, out RaycastHit hit, 0.1f, ~0, QueryTriggerInteraction.Ignore))
            {
                normal = hit.normal;
                contacto = hit.point;
                rotacion = Quaternion.FromToRotation(Vector3.up, normal);
                bool enNavMesh = NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.1f, NavMesh.AllAreas);
                if (!enNavMesh)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool IntercambiarBot()
    {
        Ray rayo = new Ray(transform.TransformPoint(a[2]), transform.forward);
        if (Physics.Raycast(rayo, out RaycastHit hit, 0.1f, ~0, QueryTriggerInteraction.Ignore))
        {
            normal = hit.normal;
            contacto = hit.point;
            rotacion = Quaternion.FromToRotation(Vector3.up, normal);
            bool enNavMesh = NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.05f, NavMesh.AllAreas);
            if (!enNavMesh)
            {
                if (navMeshAgente != null)
                {
                    transform.SetParent(null, true);
                    EliminarBot();
                    Rb.constraints = RigidbodyConstraints.FreezeAll;
                    Rb.velocity = Vector3.zero;
                    neoBot = Instantiate(navBot, contacto, rotacion);
                    transform.position = neoBot.transform.position;
                    transform.rotation = neoBot.transform.rotation;
                    transform.SetParent(neoBot.transform, true);
                    navAgente = neoBot.GetComponent<NavegacionParedes>();
                    return true;
                }
            }
            else
            {
                if (navAgente != null)
                {
                    transform.SetParent(null, true);
                    RestringeMovimiento();
                    Rb.velocity = Vector3.zero;
                    neoBot = Instantiate(navMeshBot, transform.position, transform.rotation);
                    transform.position = neoBot.transform.position;
                    transform.rotation = neoBot.transform.rotation;
                    transform.SetParent(neoBot.transform, true);
                    navMeshAgente = neoBot.GetComponent<NavMeshAgent>();
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Intercambia entre una caminata y un descanso
    /// </summary>
    public IEnumerator Mueve()
    {
        while (true)
        {
            if (navMeshAgente != null)
            {
                Ir(transform.position, 0);
                float num = Random.Range(0, 6);
                yield return new WaitForSeconds(num);

                navMeshAgente.ResetPath();
                Vector3 randomDirection = Random.insideUnitSphere * 3f;
                randomDirection += transform.position;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 3f, NavMesh.AllAreas)) Ir(hit.position, 1);
                yield return new WaitUntil(() => RutaCompletada());
            }
            else if (navAgente != null)
            {
                navAgente.velocidad = 0.1f;
                soltada = !navAgente.Avanzar();
                Animar("Caminando", 0f);
                yield return null;
            }
        }
    }
    /// <summary>
    /// La babosa evita al jugador
    /// </summary>
    public IEnumerator Corre()
    {
        while (true){
            navMeshAgente.ResetPath();
            Vector3 dirHuir = (transform.position - player.position).normalized * 3f;
            if (NavMesh.SamplePosition(transform.position + dirHuir, out NavMeshHit hit, 3f, NavMesh.AllAreas)) Ir(hit.position, 2);
            yield return new WaitUntil(() => RutaCompletada());
        }
    }
    /// <summary>
    /// La babosa se mueve en un patrón impredecible
    /// </summary>
    public IEnumerator Huye()
    {
        while (true)
        {
            navMeshAgente.ResetPath();
            Vector3 randomDirection = Random.insideUnitSphere * 3f * 5f;
            randomDirection += transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 3f, NavMesh.AllAreas)) Ir(hit.position, 3);
            yield return new WaitUntil(() => RutaCompletada());
        }
    }

    /// <summary>
    /// La babosa salta
    /// </summary>
    public IEnumerator Salta()
    {
        bool ejeSalta = false;
        while (true)
        {
            Ray ray = new Ray(transform.TransformPoint(a[1]), -transform.up);
            bool tocaSuelo = Physics.Raycast(ray, out RaycastHit hit, 0.1f, ~0, QueryTriggerInteraction.Ignore);
            if (tocaSuelo && !ejeSalta)
            {
                Animar("Subiendo", 0.05f);
                ejeSalta = true;
                Rb.velocity = Vector3.zero;
                Vector3 fuerzaSalto = Vector3.up * 5f;
                Rb.AddForce(fuerzaSalto, ForceMode.Impulse);
            }
            else if (ejeSalta && Rb.velocity.y < 1f && !tocaSuelo)
            {
                Animar("Cayendo", 0.1f);
                ejeSalta = false;
            }
            Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red);
            yield return null;
        }
    }
    /// <summary>
    /// Verifica el recorrido hacia un punto
    /// </summary>
    /// <param name="objetivo">Punto objetivo</param>
    /// <returns>Devuelve el largo del recorrido hasta el objetivo</returns>
    private float Camino(Vector3 objetivo)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(navMeshAgente.transform.position, objetivo, NavMesh.AllAreas, path))
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
        }
        return -1f;
    }
    /// <summary>
    /// Asigna un objetivo y le da una velocidad
    /// </summary>
    /// <param name="objetivo">El punto al cual se dirige (en NavMesh)</param>
    /// <param name="vel">La velocidad con la que irá (0 = Sin velocidad, 1 = Caminar, 2 = Correr, 3 = Velocidad para saltos)</param>
    public void Ir(Vector3 objetivo, int vel)
    {
        switch (vel)
        {
            case 0: navMeshAgente.speed = vel0; Animar("Existiendo", 0.15f); break;
            case 1: navMeshAgente.speed = vel1; Animar("Caminando", 0.15f); break;
            case 2: navMeshAgente.speed = vel2; Animar("Corriendo", 0.15f); break;
            case 3: navMeshAgente.speed = vel3; break;
            default: break;
        }
        navMeshAgente.SetDestination(objetivo);
    }
    /// <summary>
    /// Verifica la existencia de un escondite, comprueba si el trayecto es válido, menor o igual a una distancia y redirige a esa posicion
    /// </summary>
    /// <param name="tolerancia">Distancia máxima para el recorrido</param>
    /// <returns>Devuelve verdadero si se encontró la ruta y va en camino</returns>
    public bool BuscarEscondite(float tolerancia)
    {
        if (escondite != null)
        {
            float camino = Camino(escondite.position);
            if (camino >= 0 && camino <= tolerancia)
            {
                Ir(escondite.position, 3);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Comprueba si llegó al escondite
    /// </summary>
    /// <returns>Devuelve verdadero si la distancia al escondite es mínima</returns>
    public bool LlegoEscondite()
    {
        if (escondite != null)
        {
            float restante = (transform.position - escondite.position).magnitude;
            if (restante <= navMeshAgente.stoppingDistance) return true;
        }
        return false;
    }
    /// <summary>
    /// Verifica la existencia de comida, comprueba si el trayecto es válido, menor o igual a una distancia y redirige a esa posicion
    /// </summary>
    /// <param name="tolerancia">Distancia máxima para el recorrido</param>
    /// <returns>Devuelve verdadero si se encontró la ruta y va en camino</returns>
    public bool BuscarComida(float tolerancia)
    {
        if (comida != null)
        {
            float camino = Camino(comida.position);
            if (camino >= 0 && camino <= tolerancia)
            {
                Ir(comida.position, 1);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Comprueba si llegó a la comida
    /// </summary>
    /// <returns>Devuelve verdadero si la distancia a la comida es mínima</returns>
    public bool LlegoComida()
    {
        if (comida != null)
        {
            float restante = (transform.position - comida.position).magnitude;
            if (restante <= navMeshAgente.stoppingDistance) return true;
        }
        return false;
    }
    /// <summary>
    /// Comprueba si se llegó al destino actual
    /// </summary>
    /// <returns>Devuelve verdadero si completa el camino</returns>
    public bool RutaCompletada()
    {
        return !navMeshAgente.pathPending && navMeshAgente.remainingDistance <= navMeshAgente.stoppingDistance;
    }
    /// <summary>
    /// Destruye el bot de navegación
    /// </summary>
    public void EliminarBot()
    {
        if (neoBot != null) Destroy(neoBot);
        if (navMeshAgente != null) navMeshAgente = null;
        if (navAgente != null) navAgente = null;
    }
    /// <summary>
    /// Destruye la transformación actual
    /// </summary>
    public void EliminarTransformacion()
    {
        if (transformacion != null) Destroy(transformacion);
    }
    /// <summary>
    /// Detiene todas las acciones de la babosa y la destruye junto al bot de navegación y su transformación actual
    /// </summary>
    public void EliminarBabosa()
    {
        StopAllCoroutines();
        EliminarTransformacion();
        EliminarBot();
        Destroy(gameObject);
    }
    /// <summary>
    /// Desactiva las físicas de la babosa
    /// </summary>
    public void DesactivarFisicas()
    {
        Rb.velocity = Vector3.zero;
        Rb.useGravity = false;
        Rb.isKinematic = true;
        foreach (Collider c in Col)
        {
            c.enabled = false;
        }
        RestringeMovimiento();
    }
    /// <summary>
    /// Activa las físicas de la babosa
    /// </summary>
    public void ActivarFisicas()
    {
        Rb.useGravity = true;
        Rb.isKinematic = false;
        foreach (Collider c in Col)
        {
            c.enabled = true;
        }
        PermiteMovimiento();
    }
    /// <summary>
    /// Restringe el desplazamiento y rotación de la babosa
    /// </summary>
    public void RestringeMovimiento()
    {
        Rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    /// <summary>
    /// Permite el desplazamiento y rotación de la babosa
    /// </summary>
    public void PermiteMovimiento()
    {
        Rb.constraints = RigidbodyConstraints.None;
    }
    /// <summary>
    /// Ejecuta una animación o interpola si ya se ejecutaba otra
    /// </summary>
    /// <param name="animacion">Nombre de la animación</param>
    /// <param name="t">Tiempo de interpolación</param>
    public void Animar(string animacion, float t)
    {
        AnimatorStateInfo AnimInfo = Anim.GetCurrentAnimatorStateInfo(0);
        bool AnimTran = Anim.IsInTransition(0) && Anim.GetNextAnimatorStateInfo(0).IsName(animacion);

        if (!AnimInfo.IsName(animacion) && !AnimTran)
        {
            Anim.CrossFade(animacion, t);
        }
    }
    /// <summary>
    /// Comprueba si una animación ya terminó
    /// </summary>
    /// <param name="animacion">Nombre de la animación</param>
    /// <returns>Devuelve verdadero si completó la animación</returns>
    public bool FinAnimar(string animacion)
    {
        AnimatorStateInfo AnimInfo = Anim.GetCurrentAnimatorStateInfo(0);
        return AnimInfo.IsName(animacion) && AnimInfo.normalizedTime >= 1f;
    }
    /// <summary>
    /// Vuelve verdadero el valor de <see cref="transformar"/> al cabo de un tiempo
    /// </summary>
    /// <param name="tiempo">Tiempo de espera</param>
    public IEnumerator TemporizadorTransformar(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        transformar = true;
    }
    /// <summary>
    /// Verifica el estado actual de la babosa
    /// </summary>
    /// <param name="i">Entero o null, si se incluye cambia el estado de la babosa</param>
    /// <returns>Devuelve el estado actual si no se le ingresaron parámetros</returns>
    public int ComprobarEstado(int? i)
    {
        if (i.HasValue)
        {
            Item.Estado = i.Value;
            Debug.Log("cambiar estado a: " + i.Value);
        }

        return Item.Estado;
    }
    /// <summary>
    /// Cambia la máquina de estados actual de la babosa
    /// </summary>
    /// <param name="maquinaEstado">Indica el estado a modificar (0 = Salvaje, 1 = Interesada, 2 = Domesticada, 3 = Amigable)</param>
    public void CambiarMaquinaEstados(int maquinaEstado)
    {
        switch (maquinaEstado)
        {
            case 0: this.CambiarModo(new BabosaSalvaje(this)); break;
            case 1: this.CambiarModo(new BabosaInteresada(this)); break;
            case 2: this.CambiarModo(new BabosaDomesticada(this)); break;
            case 3: this.CambiarModo(new BabosaAmigable(this)); break;
        }
    }
    /// <summary>
    /// Busca colisiones con el entorno a bajas velocidades
    /// </summary>
    /// <returns>Devuelve verdadero si detectó una colisión</returns>
    private bool DetectarEntornoTerrestre()
    {
        // Inicializa rayos en todas las direcciones
        Ray[] rayos = {
            new Ray(transform.TransformPoint(a[0]), transform.up),
            new Ray(transform.TransformPoint(a[1]), -transform.up),
            new Ray(transform.TransformPoint(a[2]), transform.forward),
            new Ray(transform.TransformPoint(a[3]), -transform.forward),
            new Ray(transform.TransformPoint(a[4]), transform.right),
            new Ray(transform.TransformPoint(a[5]), -transform.right)
        };
        foreach (Ray ray in rayos)
        {
            // Dibuja los rayos
            Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red, 0.1f);

            if (Physics.Raycast(ray, out RaycastHit hit, 0.1f, ~0, QueryTriggerInteraction.Ignore))
            {
                normal = hit.normal;
                return true; // Entró en contacto con un NavMesh
            }
        }
        normal = Vector3.zero;
        return false; // No encontró nada
    }
    /// <summary>
    /// Busca colisiones con el entorno a altas velocidades
    /// </summary>
    /// <returns>Devuelve verdadero si detectó una colisión</returns>
    public bool DetectarEntornoVuelo()
    {
        Vector3 centro = transform.TransformPoint(Cap.center);
        Vector3 eje = transform.up;
        Vector3 puntoA = centro + eje * (Cap.height / 2 - Cap.radius);
        Vector3 puntoB = centro - eje * (Cap.height / 2 - Cap.radius);

        Debug.DrawLine(puntoA, puntoB, Color.yellow, 0.1f);
        Debug.DrawRay(puntoA, Vector3.forward * Cap.radius, Color.yellow, 0.1f);
        Debug.DrawRay(puntoA, -Vector3.forward * Cap.radius, Color.yellow, 0.1f);
        Debug.DrawRay(puntoB, Vector3.forward * Cap.radius, Color.yellow, 0.1f);
        Debug.DrawRay(puntoB, -Vector3.forward * Cap.radius, Color.yellow, 0.1f);

        Collider[] hits = Physics.OverlapCapsule(puntoA, puntoB, Cap.radius, ~0, QueryTriggerInteraction.Ignore);

        foreach (Collider c in hits)
        {
            if (c != Cap)
            {
                if (c is TerrainCollider terreno)
                {
                    Vector3 pos = centro;
                    Vector3 normalTerreno = terreno.terrainData.GetInterpolatedNormal(
                        pos.x / terreno.terrainData.size.x,
                        pos.z / terreno.terrainData.size.z
                    );
                    normal = normalTerreno.normalized;
                }
                else
                {
                    Vector3 puntoContacto = c.ClosestPoint(centro);
                    normal = (centro - puntoContacto).normalized;
                }
                return true;
            }
        }
        normal = Vector3.zero;
        return false;
    }
    /// <summary>
    /// Instancia la transformación de la babosa si es que la hay
    /// </summary>
    /// <returns>Devuelve verdadero si logró instanciar</returns>
    public bool Transformar()
    {
        if (velociforma != null || megaforma != null)
        {
            if (velocidadDisparo < 100f)
            {
                transformacion = Instantiate(velociforma, transform.position, transform.rotation);
            }
            else
            {
                transformacion = Instantiate(megaforma, transform.position, transform.rotation);
            }
            return true;
        }
        return false;
    }
    public bool DesTransformar()
    {
        if (contadorAtaques == 0)
        {
            EliminarTransformacion();
            return true;
        }
        return false;
    }
    public void detectarObjetivo(float radioVision, float anguloVision, float radioInicio)
    {
        float medioAnguloRad = Mathf.Deg2Rad * (anguloVision / 2f);
        float desplazamiento = 0.6f / Mathf.Tan(medioAnguloRad);
        Vector3 origen = transform.position - transform.forward * desplazamiento;

        objetivo = null;
        direccionObjetivo = Vector3.zero;
        distanciaObjetivo = float.MaxValue;
        float mejorPuntaje = float.MinValue;

        Collider[] candidatos = Physics.OverlapSphere(transform.position, radioVision + desplazamiento, mascaraObjetivo);

        foreach (Collider col in candidatos)
        {
            Vector3 puntoCercano = col.ClosestPoint(origen);
            Vector3 direccion = (puntoCercano - origen);
            float distancia = direccion.magnitude;

            if (distancia < desplazamiento) continue;

            float angulo = Vector3.Angle(transform.forward, direccion);

            if (angulo > anguloVision / 2f) continue;

            direccion.Normalize();

            bool sinObstaculo = !Physics.Raycast(origen, direccion, distancia, mascaraObstaculo);
            Debug.DrawRay(origen, direccion * distancia, sinObstaculo ? Color.green : Color.red);

            if (!sinObstaculo) continue;

            float anguloNorm = 1f - (angulo / (anguloVision / 2f));
            float distanciaNorm = Mathf.Clamp01(1f - ((distancia - desplazamiento) / radioVision));
            float puntaje = (anguloNorm * 0.2f) + (distanciaNorm * 0.8f) + Random.Range(-0.05f, 0.05f);

            if (puntaje > mejorPuntaje)
            {
                mejorPuntaje = puntaje;
                objetivo = col.gameObject;
                direccionObjetivo = direccion;
                distanciaObjetivo = distancia - desplazamiento;
            }
        }

        Vector3 derecha = Quaternion.Euler(0, anguloVision / 2f, 0) * transform.forward;
        Vector3 izquierda = Quaternion.Euler(0, -anguloVision / 2f, 0) * transform.forward;
        Debug.DrawLine(origen, origen + derecha * radioVision, Color.yellow);
        Debug.DrawLine(origen, origen + izquierda * radioVision, Color.yellow);
        Debug.DrawRay(origen, transform.forward * radioVision, Color.cyan);

        if (objetivo != null)
            Debug.DrawRay(origen, direccionObjetivo * distanciaObjetivo, Color.blue);
    }
    //Detecta posibles objetivos para la babosa transformada........................................
    public bool EjecutarAtaque(GameObject Ataque)
    {
        ControlAtaque CtrlAtaq = Ataque.GetComponent<ControlAtaque>();
        if ((contadorAtaques - CtrlAtaq.pesoAtaque) >= 0) return true;
        else return false;
    }
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
        ataqueScript.velocidad = velocidadDisparo;
        ataqueScript.ataque = true;
    }
    //Brinda toda la informacion necesaria para el ataque de la babosa..............................
}