using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera playerCamera; // Referencia a la cámara principal
    public Transform container; // Referencia al objeto contenedor que rota

    void Update()
    {
        // Asegúrate de que la cámara y el contenedor estén asignados
        if (playerCamera != null && container != null)
        {
            // Obtener la dirección hacia la que la cámara está mirando
            Vector3 targetDirection = playerCamera.transform.forward;

            // Hacer que el contenedor rote hacia esa dirección
            container.rotation = Quaternion.LookRotation(targetDirection);
        }
    }
}
