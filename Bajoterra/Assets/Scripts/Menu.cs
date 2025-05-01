using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject MenuPausa;
    public GameObject Mochila;
    public CameraController scriptCamara;
    private bool juegoPausado = false;
    private bool mochilaAbierta = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Despausar();
            }
            else
            {
                Pausar();
            }
        }

        if (!juegoPausado)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!mochilaAbierta)
                {
                    abrirMochila();
                }
                else
                {
                    cerrarMochila();
                }
            }
        }
    }

    public void abrirMochila()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        scriptCamara.enabled = false;

        mochilaAbierta = true;
        Mochila.SetActive(true);
    }

    public void cerrarMochila()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        scriptCamara.enabled = true;

        mochilaAbierta = false;
        Mochila.SetActive(false);
    }

    public void Pausar()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        scriptCamara.enabled = false;

        juegoPausado = true;
        MenuPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Despausar()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        scriptCamara.enabled = true;

        juegoPausado = false;
        MenuPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitarJuego()
    {
        juegoPausado = false;
        MenuPausa.SetActive(true);
        Application.Quit();
    }
}
