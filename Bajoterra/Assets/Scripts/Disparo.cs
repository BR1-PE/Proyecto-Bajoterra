using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    public Transform Cartucho;
    public Transform CartuchoTubo;
    public GameObject Tubo;
    public GameObject Lanzador;
    public float fuerzaDisparo = 500.0f;
    public bool cargado = false;
    public Inventario listaDeBabosasScript;
    GameObject babosaInstanciada = null;
    GameObject tuboInstanciado = null;
    public bool controladorArmaMano = false;
    public GameObject Audio;
    GameObject reproductorAudio;
    void Update()
    {
        if (controladorArmaMano)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (tuboInstanciado != null && listaDeBabosasScript.BuscarTubos())
                {
                    if (babosaInstanciada != null)
                    {
                        CerebroBabosa cerebro = babosaInstanciada.GetComponent<CerebroBabosa>();
                        cerebro.ejecutadoBabosa = true;        
                        babosaInstanciada = null;
                        cargado = false;
                    }
                    tuboInstanciado.transform.SetParent(null, true);
                    tuboInstanciado.GetComponent<Rigidbody>().AddForce(CartuchoTubo.forward * 30f);
                    tuboInstanciado.GetComponent<Rigidbody>().useGravity = true;
                    tuboInstanciado.GetComponentInChildren<MeshCollider>().enabled = true;
                    tuboInstanciado.GetComponent<PickableObject>().isPickable = true;

                    tuboInstanciado = null;
                }

                if (listaDeBabosasScript.BuscarTubos())
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    GameObject Babosa;
                    Babosa = listaDeBabosasScript.obtenerBabosaInventario();
                    
                    if (listaDeBabosasScript.SacarTuboInventario())
                    {
                        tuboInstanciado = Instantiate(Tubo, CartuchoTubo.position, CartuchoTubo.rotation);
                        tuboInstanciado.GetComponent<Rigidbody>().useGravity = false;
                        tuboInstanciado.transform.SetParent(transform, true);
                        tuboInstanciado.GetComponentInChildren<MeshCollider>().enabled = false;

                        listaDeBabosasScript.ActualizarInventarioTubos();

                        if (listaDeBabosasScript.BuscarBabosas())
                        {
                            int numeroEstado = listaDeBabosasScript.obtenerEstadoBabosaInventario();
                            if (listaDeBabosasScript.SacarBabosaInventario())
                            {
                                babosaInstanciada = Instantiate(Babosa, Cartucho.position, Cartucho.rotation);
                                CerebroBabosa cerebro = babosaInstanciada.GetComponent<CerebroBabosa>();
                                cerebro.destino = transform;
                                cerebro.CambiarModo(new BabosaArmada(cerebro));
                                cargado = true;
                                babosaInstanciada.GetComponent<Item>().Estado = numeroEstado;

                                listaDeBabosasScript.ActualizarInventarioBabosas();
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Debes tener babosas y tubos p crack");
                }

                listaDeBabosasScript.ActualizarMochila();
            }
        }
        if ((Input.GetMouseButtonDown(0) || !controladorArmaMano) && cargado && babosaInstanciada != null)
        {
            cargado = false;
            reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
            reproductorAudio.GetComponent<ReproductorAudio>().disparar();
            reproductorAudio = null;
            babosaInstanciada.transform.SetParent(null, true);
            babosaInstanciada.transform.localScale = new Vector3(1f, 1f, 1f);

            StartCoroutine(esperaRecarga());
            babosaInstanciada = null;
        }

        if (!controladorArmaMano)
        {
            if (Lanzador.GetComponent<MeshRenderer>().enabled)
            {
                Lanzador.GetComponent<MeshRenderer>().enabled = false;

                if (tuboInstanciado != null)
                {
                    tuboInstanciado.SetActive(false);
                }
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

                if (tuboInstanciado != null)
                {
                    tuboInstanciado.SetActive(true);
                }
            }
        }
    }

    IEnumerator esperaRecarga()
    {
        yield return new WaitForSeconds(0.1f);
        cargado = true;
    }
}