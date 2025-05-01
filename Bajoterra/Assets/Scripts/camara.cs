using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // El transform del jugador
    public GameObject playerController;
    public float alturaParado = 1.5f;
    public float alturaAgachado = 1.5f;
    private float smoothVelocityY = 0.0f; // Velocidad de suavidad para el eje Y

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (!player)
        {
            return;
        }

        bool agachado = playerController.GetComponent<ThirdPersonController>().isCrouching;
        float targetHeight = agachado ? alturaAgachado : alturaParado;

        // Posición deseada de la cámara
        Vector3 targetPosition = player.position + Vector3.up * targetHeight;

        // Actualizar suavemente solo el eje Y
        float smoothY = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref smoothVelocityY, 0.1f);

        // Asignar la posición final
        transform.position = new Vector3(targetPosition.x, smoothY, targetPosition.z);
    }
}