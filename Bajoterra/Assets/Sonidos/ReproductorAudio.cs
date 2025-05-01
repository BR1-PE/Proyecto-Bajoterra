using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductorAudio : MonoBehaviour
{
    public AudioSource Reproductor;
    public AudioClip[] sonido;

    public void activarArma()
    {
        Reproductor.PlayOneShot(sonido[0]);
        Destroy(gameObject, sonido[0].length);
    }
    public void disparar()
    {
        Reproductor.PlayOneShot(sonido[1]);
        Destroy(gameObject, sonido[1].length);
    }
    public void transformar()
    {
        Reproductor.PlayOneShot(sonido[2]);
        Destroy(gameObject, sonido[2].length);
    }
    public void sonidoExtra()
    {
        if(sonido[3] != null){
            Reproductor.PlayOneShot(sonido[3]);
            Destroy(gameObject, sonido[3].length);
        }
    }
}
