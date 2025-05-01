using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    public float mouseSensitivity = 80f;  // Sensibilidad del ratón
    public Transform playerBody;           // Referencia al cuerpo del jugador
    private float xRotation = 0f;

    void Start()
    {
        // Bloquea y oculta el cursor para una experiencia inmersiva
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Captura los movimientos del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Controla la rotación vertical (eje Y)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limita la rotación para evitar giros completos

        // Aplica las rotaciones
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
