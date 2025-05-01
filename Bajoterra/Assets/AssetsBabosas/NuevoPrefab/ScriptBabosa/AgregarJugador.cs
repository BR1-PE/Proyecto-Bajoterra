using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgregarJugador : MonoBehaviour
{
    public Transform Jugador;

    void Update()
    {
        if (Jugador != null){
            if (GetComponent<CerebroBabosa>().player == null){
                GetComponent<CerebroBabosa>().player = Jugador;
                this.enabled = false;
            }
        }
    }
}
