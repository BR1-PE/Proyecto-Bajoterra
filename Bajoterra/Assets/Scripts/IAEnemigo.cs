using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAEnemigo : MonoBehaviour
{
    NavMeshAgent agent;
    public float rango = 5f;
    public float velocidad = 0.1f;
    public Transform player;

    public float velocidadJugador;
    public float distanciaJugador;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocidad;
    }

    private void Update()
    {
        velocidadJugador = player.gameObject.GetComponent<ThirdPersonController>().velocidadPlayer;

        distanciaJugador = Vector3.Distance(transform.position, player.position);

        if (distanciaJugador > rango)
        {
            agent.SetDestination(player.position);
            velocidad = 4;
        }
        else
        {
            agent.SetDestination(player.position);
            velocidad = 0.1f;
        }
        agent.speed = velocidad;
    }
}