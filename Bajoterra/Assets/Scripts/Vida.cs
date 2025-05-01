using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Vida : MonoBehaviour
{
    [SerializeField] private Image vida;
    [SerializeField] private Image fondo;
    [SerializeField] private float Largo;
    [SerializeField] private bool objeto;
    public float vidaTotal;
    public float vidaActual;
    public float dañoPorSegundo;
    private bool coroutineEjecutada;

    void Start()
    {
        vidaActual = vidaTotal;
        coroutineEjecutada = false;
    }

    void Update()
    {
        if (dañoPorSegundo > 0f)
        {
            aplicarDañoContinuo(dañoPorSegundo * Time.deltaTime);
        }

        if (vidaActual <= 0 && !coroutineEjecutada)
        {
            StartCoroutine(reiniciar());
        }
    }

    public void aplicarDañoContinuo(float daño)
    {
        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaTotal);
        cambiarVida();
    }

    public void aplicarDañoInstantaneo(float daño)
    {
        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaTotal);
        cambiarVida();
    }

    public void cambiarVida()
    {
        if (!objeto){
            Largo = vidaActual/vidaTotal;
            vida.fillAmount = Largo;
            if (Largo > 0.5f)
            {
                vida.color = Color.Lerp(Color.yellow, Color.green, (Largo - 0.5f) * 2);
            }
            else
            {
                vida.color = Color.Lerp(Color.red, Color.yellow, Largo * 2);
            }
        }
        else if (vidaActual <= 0){
            Destroy(gameObject);
        }
    }

    IEnumerator reiniciar()
    {
        if (!objeto){
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0, 0.8f * Time.unscaledDeltaTime);
            fondo.color = new Color(0, 0, 0, Mathf.Lerp(fondo.color.a, 1, 0.8f * Time.unscaledDeltaTime));
            yield return new WaitForSecondsRealtime(8);
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}