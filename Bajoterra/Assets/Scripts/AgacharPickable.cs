using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgacharPickable : MonoBehaviour
{
    public GameObject playerController;
    public float alturaParado;
    public float alturaAgachado;

    /*void Update()
    {
        bool agachado = playerController.GetComponent<ThirdPersonController>().isCrouching;
        Vector3 currentPosition = transform.position;

        if (!agachado)
        {
            currentPosition.y = alturaParado;
            transform.position = currentPosition;
        }
        else
        {
            currentPosition.y = alturaAgachado;
            transform.position = currentPosition;
        }
    }*/
}
