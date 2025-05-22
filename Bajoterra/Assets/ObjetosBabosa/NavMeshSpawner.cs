using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSpawner : MonoBehaviour
{
    public GameObject[] objetoAInstanciar; // El objeto que será instanciado
    public int cantidad = 10; // Cantidad de objetos a instanciar
    public float rangoNavMesh = 1f; // Rango para verificar el NavMesh
    public int estado;

    private Collider areaCollider; // El collider que define el volumen

    private void Start()
    {
        areaCollider = GetComponent<Collider>();

        if (areaCollider == null)
        {
            Debug.LogError("Este objeto necesita un Collider para determinar el volumen.");
            return;
        }

        StartCoroutine(InstanciarObjetosEnNavMesh());
    }

    private IEnumerator InstanciarObjetosEnNavMesh()
    {
        int instanciados = 0;
        GameObject Instancia;

        while (instanciados < cantidad)
        {
            foreach (var item in objetoAInstanciar)
            {
                Vector3 posicionAleatoria = GenerarPosicionAleatoriaEnCollider();

                if (NavMesh.SamplePosition(posicionAleatoria, out NavMeshHit hit, rangoNavMesh, NavMesh.AllAreas))
                {
                    Instancia = Instantiate(item, hit.position, Quaternion.identity);
                    if (Instancia.GetComponent<CerebroBabosa>() != null)
                    {
                        CerebroBabosa cerebro = Instancia.GetComponent<CerebroBabosa>();
                        switch (estado)
                        {
                            case 0: cerebro.CambiarModo(new BabosaSalvaje(cerebro)); break;
                            case 1: cerebro.CambiarModo(new BabosaInteresada(cerebro)); break;
                            case 2: cerebro.CambiarModo(new BabosaDomesticada(cerebro)); break;
                            case 3: cerebro.CambiarModo(new BabosaAmigable(cerebro)); break;
                        }
                    }
                    instanciados++;
                }

            }

            yield return null; // Esperar al siguiente frame para evitar bloqueos
        }
    }

    private Vector3 GenerarPosicionAleatoriaEnCollider()
    {
        // Obtener los límites del collider
        Bounds bounds = areaCollider.bounds;

        // Generar coordenadas aleatorias dentro del collider
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }
}
