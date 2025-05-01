using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public bool isPickable = true;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerInteractionZone" )
        {
            GameObject jugador = other.GetComponent<AgacharPickable>().playerController;
            jugador.GetComponent<PickUpObject>().ObjectToPickUp = this.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerInteractionZone" )
        {
            GameObject jugador = other.GetComponent<AgacharPickable>().playerController;
            jugador.GetComponent<PickUpObject>().ObjectToPickUp = null;
        }
    }
}