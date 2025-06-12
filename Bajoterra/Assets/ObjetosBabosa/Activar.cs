using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activar : MonoBehaviour
{
    public NavMeshSpawner script;
    bool activar;

    void Update()
    {
        if (activar && Input.GetKeyDown(KeyCode.Return))
        {
            script.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Personaje"))
        {
            activar = true;
            Debug.Log("Activarlo");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Personaje"))
        {
            activar = false;
        }
    }
}