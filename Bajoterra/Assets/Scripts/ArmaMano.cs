using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaMano : MonoBehaviour
{
    public GameObject Arma;
    public GameObject Mano;
    public bool intercambioArmaMano = false;

    void Start()
    {
        Mano.GetComponent<PickUpObject>().controladorArmaMano = false;
        Arma.GetComponent<Disparo>().controladorArmaMano = false;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!intercambioArmaMano)
            {
                Mano.GetComponent<PickUpObject>().controladorArmaMano = true;
                Arma.GetComponent<Disparo>().controladorArmaMano = true;
                intercambioArmaMano = true;
            }
            else
            {
                Mano.GetComponent<PickUpObject>().controladorArmaMano = false;
                Arma.GetComponent<Disparo>().controladorArmaMano = false;
                intercambioArmaMano = false;
            }
        }
    }
}
