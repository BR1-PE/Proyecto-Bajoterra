using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaMovimiento : MonoBehaviour
{
    public float velocidad = 0.3f;            // Velocidad de movimiento del personaje
    public float fuerzaSalto = 7f;        // Fuerza de salto
    public float rapidez;
    public float altura;
    public float velocidadRotacion = 100f;
    public Vector3 ajusteAngulo;
    public LayerMask groundLayer;       // Capa del suelo para detectar si está tocando el suelo
    private Animator animator;          // Referencia al Animator del personaje
    
    private float posicionInicialY;
    private Rigidbody rb;               // Referencia al Rigidbody del personaje
    private Vector3 posicionAnterior;
    private Quaternion rotacionInicial;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rotacionInicial = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void Update()
    {   
        // Rotación en el eje Y controlada por el eje Horizontal
        float rotacionHorizontal = Input.GetAxis("Horizontal") * velocidadRotacion * Time.deltaTime;
        transform.Rotate(0f, rotacionHorizontal, 0f, Space.Self);

        // ..............................................         
        Vector3 desplazamiento = transform.position - posicionAnterior;
        
        // Divide por el tiempo entre frames para obtener la velocidad en unidades por segundo
        rapidez = desplazamiento.magnitude / Time.deltaTime;

        //Debug.Log("Velocidad: " + rapidez + " unidades/segundo");

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            // Actualiza la rotacionInicial después de rotar
            rotacionInicial = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            // Resetea la rotación cuando no está en las animaciones específicas
            transform.rotation = rotacionInicial;
            // Actualiza la posición anterior para el próximo cálculo
            posicionAnterior = transform.position;
            // ...............................................

            animator.SetBool("Suelo", true);
            animator.SetBool("Tope", true);

            if ((stateInfo.IsName("Existiendo") || stateInfo.IsName("Caminando") || stateInfo.IsName("Corriendo")) && Input.GetButtonDown("Jump"))
            {
                Vector3 dirSalto= new Vector3 (0 ,7 , 1);
                rb.AddForce(dirSalto, ForceMode.Impulse);
                animator.SetBool("Saltar", true);
                animator.SetBool("Suelo", false);
                posicionInicialY = transform.position.y;

            }
            else
            {
                animator.SetBool("Saltar", false);
            }
        }
        else
        {
            animator.SetBool("Suelo", false);

            // Rotación en dirección del movimiento solo en ciertas animaciones y cuando está en el aire
            if (stateInfo.IsName("Volando") || stateInfo.IsName("Choque") || stateInfo.IsName("Transformando1") || stateInfo.IsName("Transformando2"))
            {
                Vector3 direccionMovimiento = desplazamiento.normalized;

                if (direccionMovimiento != Vector3.zero)
                {
                    // Calcula la rotación objetivo en el eje Y
                    Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
                    Quaternion ajuste = Quaternion.Euler(ajusteAngulo);
                    rotacionObjetivo *= ajuste;

                    // Interpola hacia la rotación objetivo
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionObjetivo, rapidez * Time.deltaTime);

                    // Mantiene el valor de rotación en los ejes X y Z
                    Vector3 rotacionActual = transform.rotation.eulerAngles;
                    transform.rotation = Quaternion.Euler(rotacionActual.x, transform.rotation.eulerAngles.y, rotacionActual.z);
                }
            }
        }

        // Movimiento hacia adelante
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            float move = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(0, 0, move) * velocidad * Time.deltaTime;
            transform.Translate(movement);

            bool Avanzar = move != 0;
            animator.SetBool("Avanzar", Avanzar);
            animator.SetBool("Correr", false);
        }
        else
        {
            float move = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(0, 0, move) * velocidad * 10 * Time.deltaTime;
            transform.Translate(movement);
            animator.SetBool("Correr", true);
        }

        // Cálculo de la altura para el parámetro de animación
        altura = transform.position.y - posicionInicialY;
        animator.SetFloat("Rapidez", rapidez);

        if (altura > -5)
        {
            animator.SetBool("Rapido", false);
        }
        else
        {
            animator.SetBool("Rapido", true);
        }
  
        if (stateInfo.IsName("Transformando1") || stateInfo.IsName("Transformando2")){
            
            bool isMovingDown = rb.velocity.y < 0.1;
            animator.SetBool("Abajo", isMovingDown);
        } else {

            animator.SetBool("Abajo", false);
        }

        if (stateInfo.IsName("Cayendo")){
            animator.SetBool("Tope", false);
        }

        if (stateInfo.IsName("CayendoMal") || stateInfo.IsName("Existiendo") || stateInfo.IsName("Subiendo") || stateInfo.IsName("Cayendo")){
            animator.SetBool("Pared", false);
        }

    }

    void OnCollisionEnter(Collision groundLayer){

        animator.SetBool("Pared", true);
    }
}