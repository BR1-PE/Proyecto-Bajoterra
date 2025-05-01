using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABabosaMovimiento : MonoBehaviour
{
    public float velocidad = 0.3f;            // Velocidad de movimiento del personaje
    public float rapidez;
    public float altura;
    public float velocidadRotacion = 100f;
    public Vector3 ajusteAngulo;
    public Vector3 salto = new Vector3 (0 ,5 , 0);
    private Animator animator;
    private float posicionInicialY;
    public bool impulso = false;
    private Rigidbody rb;
    private Vector3 posicionAnterior;
    private Quaternion rotacionInicial;

    bool golpeado = false;

    void Start()
    {
        if (transform.parent == null)
        {
            GetComponent<AsignarNavMesh>().enabled = true;
            this.enabled = false;
        }
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rotacionInicial = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        posicionAnterior = transform.position;

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    void Update()
    {   
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Restringir altura mínima
        Vector3 posicion = transform.localPosition;
        if (posicion.y < -1)
        {
            posicion.y = 0;
            transform.localPosition = posicion;
        }

        //Cálculo de velocidad general
        float distancia = Vector3.Distance(transform.position, posicionAnterior);
        velocidad = distancia / Time.deltaTime;

        //Cálculo de velocidad en los ejes
        float velocidadY = Mathf.Abs((transform.position.y - posicionAnterior.y) / Time.deltaTime);
        posicionAnterior = transform.position;

        // Movimiento general
        if (velocidad == 0)
        {
            animator.SetBool("Caminando", false);
            animator.SetBool("Corriendo", false);
            animator.SetBool("Saltando", false);
        }
        else if (GetComponentInParent<IABabosa>() != null)
        {
            if (GetComponentInParent<IABabosa>().velocidad == 0.1f)
            {
                animator.SetBool("Caminando", true);
            }
            else
            {
                animator.SetBool("Caminando", false);
            }

            if (GetComponentInParent<IABabosa>().velocidad == 1f)
            {
                animator.SetBool("Corriendo", true);
            }
            else
            {
                animator.SetBool("Corriendo", false);
            }

            if (GetComponentInParent<IABabosa>().velocidad == 5f)
            {
                animator.SetBool("Saltando", true);
            }
            else
            {
                animator.SetBool("Saltando", false);
            }
        }

        // Inicio de animación de salto
        if (stateInfo.IsName("Saltando") && !impulso)
        {
            rb.AddForce(salto, ForceMode.Impulse);
            animator.SetBool("Cayendo1", false);
            animator.SetBool("Cayendo2", false);
            impulso = true;
        }

        // Animación de altura máxima
        if ((velocidadY < 0.5f) && stateInfo.IsName("Subiendo"))
        {
            animator.SetBool("Flotando", true);
        }

        // Animación de caída
        if (rb.velocity.y < 0)
        {
            animator.SetBool("Flotando", false);
            animator.SetBool("Cayendo1", true);
        }

        // Fin de animación de salto
        if (stateInfo.IsName("Cayendo") || stateInfo.IsName("Aterrizaje1") || stateInfo.IsName("Aterrizaje2"))
        {
            impulso = false;
        }
        
        if (stateInfo.IsName("GolpeFinal") || stateInfo.IsName("Aterrizaje2"))
        {
            if (GetComponentInParent<IABabosa>() != null)
            {
                GetComponentInParent<IABabosa>().velocidad = 0f;
            }
            GetComponent<Animator>().SetBool("Disparando", false);
            golpeado = true;
        }
        else if (stateInfo.IsName("Existiendo") && golpeado)
        {
            if (GetComponentInParent<IABabosa>() != null)
            {
                GetComponentInParent<IABabosa>().velocidad = 0.1f;
            }
        }
    }
}