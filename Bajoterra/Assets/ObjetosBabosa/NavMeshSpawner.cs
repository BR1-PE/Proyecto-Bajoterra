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

    void OnEnable()
    {
        areaCollider = GetComponent<Collider>();

        if (areaCollider == null)
        {
            Debug.LogError("Este objeto necesita un Collider para determinar el volumen.");
            return;
        }

        StartCoroutine(InstanciarObjetosEnNavMesh());
    }
    /// <summary>
    /// Busca posiciones aleatorias dentro del área de navegación e instancia objetos
    /// </summary>
    private IEnumerator InstanciarObjetosEnNavMesh()
    {
        int instanciados = 0;
        GameObject Instancia;

        while (instanciados < cantidad)
        {
            foreach (GameObject item in objetoAInstanciar)
            {
                Vector3 posicionAleatoria = GenerarPosicionAleatoriaEnCollider();

                if (NavMesh.SamplePosition(posicionAleatoria, out NavMeshHit hit, rangoNavMesh, NavMesh.AllAreas))
                {
                    Instancia = Instantiate(item, hit.position, Quaternion.identity);
                    CerebroBabosa CB = Instancia.GetComponent<CerebroBabosa>();
                    if (CB != null)
                    {
                        yield return null;
                        CB.CambiarMaquinaEstados(estado);
                    }
                    instanciados++;
                }

            }

            yield return null;
        }
        this.enabled = false;
    }
    /// <summary>
    /// Aleatoriza posiciones en 3d
    /// </summary>
    /// <returns>Devuelve un Vector3 con la posición elegida</returns>
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
