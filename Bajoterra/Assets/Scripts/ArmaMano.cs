using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Activa y desactiva el arma
/// </summary>
public class ArmaMano : MonoBehaviour
{
    public GameObject Arma;
    public GameObject Mano;
    public bool intercambioArmaMano = false;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Mano.GetComponent<PickUpObject>().armaActiva = false;
        Arma.GetComponent<Disparo>().armaActiva = false;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!intercambioArmaMano)
            {
                Mano.GetComponent<PickUpObject>().armaActiva = true;
                Arma.GetComponent<Disparo>().armaActiva = true;
                intercambioArmaMano = true;
            }
            else
            {
                Mano.GetComponent<PickUpObject>().armaActiva = false;
                Arma.GetComponent<Disparo>().armaActiva = false;
                intercambioArmaMano = false;
            }
        }
    }
}
