using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Instancia tubos y babosas para dispararlos
/// </summary>
public class Disparo : MonoBehaviour
{
    public Transform Cartucho;
    public Transform CartuchoTubo;
    public GameObject Tubo;
    public GameObject Lanzador;
    public float velocidadDisparo = 30f;
    public float velocidadDescargado = 3f;
    public Inventario listaDeBabosasScript;
    public bool armaActiva = false;
    public GameObject Audio;
    private GameObject babosaInstanciada = null;
    private GameObject tuboInstanciado = null;
    private GameObject reproductorAudio;
    private bool disparable = false;
    private Rigidbody RbTubo;
    private Rigidbody Rb;
    private CerebroBabosa CB;
    void Update()
    {
        if (armaActiva)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (listaDeBabosasScript.BuscarTubos())
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    if (listaDeBabosasScript.SacarTuboInventario())
                    {
                        RecargarTubo();
                        listaDeBabosasScript.ActualizarInventarioTubos();

                        if (listaDeBabosasScript.BuscarBabosas())
                        {
                            StartCoroutine(RecargarBabosa());
                        }
                    }
                }
                listaDeBabosasScript.ActualizarInventarioBabosas();
                listaDeBabosasScript.ActualizarMochila();
            }
            if (Input.GetMouseButtonDown(0) && disparable && babosaInstanciada != null)
            {
                DispararArma();
            }
        }
        if (!armaActiva)
        {
            if (Lanzador.GetComponent<MeshRenderer>().enabled)
            {
                Lanzador.GetComponent<MeshRenderer>().enabled = false;

                tuboInstanciado?.SetActive(false);
                babosaInstanciada?.SetActive(false);
            }
        }
        else
        {
            if (!Lanzador.GetComponent<MeshRenderer>().enabled)
            {
                reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
                reproductorAudio.GetComponent<ReproductorAudio>().activarArma();
                reproductorAudio = null;

                Lanzador.GetComponent<MeshRenderer>().enabled = true;

                tuboInstanciado?.SetActive(true);
                babosaInstanciada?.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Expulsa un tubo y una babosa si es que los hay e instancia un tubo para babosa
    /// </summary>
    private void RecargarTubo()
    {
        VaciarArma();
        tuboInstanciado = Instantiate(Tubo, CartuchoTubo.position, CartuchoTubo.rotation);
        RbTubo = tuboInstanciado.GetComponent<Rigidbody>();
        RbTubo.useGravity = false;
        tuboInstanciado.transform.SetParent(transform, true);
        tuboInstanciado.GetComponentInChildren<MeshCollider>().enabled = false;
    }
    /// <summary>
    /// Instancia una babosa
    /// </summary>
    private IEnumerator RecargarBabosa()
    {
        GameObject Babosa;
        Babosa = listaDeBabosasScript.obtenerBabosaInventario();
        int numeroEstado = listaDeBabosasScript.obtenerEstadoBabosaInventario();
        if (listaDeBabosasScript.SacarBabosaInventario())
        {
            babosaInstanciada = Instantiate(Babosa, Cartucho.position, Cartucho.rotation);
            yield return null;
            CB = babosaInstanciada.GetComponent<CerebroBabosa>();
            CB.CambiarModo(new BabosaArmada(CB));
            Rb = babosaInstanciada.GetComponent<Rigidbody>();
            babosaInstanciada.transform.position = Cartucho.position;
            babosaInstanciada.transform.rotation = Cartucho.rotation;
            babosaInstanciada.transform.SetParent(Cartucho, true);
            babosaInstanciada.GetComponent<Item>().Estado = numeroEstado;
            disparable = true;
        }
    }
    /// <summary>
    /// Expulsa un tubo y una babosa si estaban redisparables
    /// </summary>
    private void VaciarArma()
    {
        if (babosaInstanciada != null)
        {
            CB.soltada = true;
            CB.ActivarFisicas();
            Rb.AddForce(CartuchoTubo.forward * velocidadDescargado, ForceMode.VelocityChange);
            babosaInstanciada.transform.SetParent(null, true);
            babosaInstanciada = null;
        }
        if (tuboInstanciado != null)
        {
            tuboInstanciado.transform.SetParent(null, true);
            RbTubo.AddForce(CartuchoTubo.forward * velocidadDescargado, ForceMode.VelocityChange);
            RbTubo.useGravity = true;
            tuboInstanciado.GetComponentInChildren<MeshCollider>().enabled = true;
            tuboInstanciado.GetComponent<PickableObject>().isPickable = true;
            tuboInstanciado = null;
        }
    }
    /// <summary>
    /// Dispara a la babosa
    /// </summary>
    private void DispararArma()
    {
        reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
        reproductorAudio.GetComponent<ReproductorAudio>().disparar();
        reproductorAudio = null;
        babosaInstanciada.transform.SetParent(null, true);
        CB.disparada = true;
        CB.ActivarFisicas();
        CB.velocidadDisparo = velocidadDisparo;
        Rb.AddForce(CartuchoTubo.up * velocidadDisparo, ForceMode.VelocityChange);
        disparable = false;

        StartCoroutine(EsperaRecarga());
        babosaInstanciada = null;
    }
    /// <summary>
    /// Tiempo de retraso entre disparos
    /// </summary>
    private IEnumerator EsperaRecarga()
    {
        yield return new WaitForSeconds(0.1f);
        disparable = true;
    }
}