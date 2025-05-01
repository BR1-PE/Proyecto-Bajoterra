using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    public Transform Cartucho;
    public Transform CartuchoTubo;
    public GameObject Tubo;
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
        if (!cargado && controladorArmaMano)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (tuboInstanciado != null && listaDeBabosasScript.BuscarTubos())
                {
                    tuboInstanciado.transform.SetParent(null, true);
                    tuboInstanciado.GetComponent<Rigidbody>().AddForce(CartuchoTubo.forward * 30f);
                    tuboInstanciado.GetComponent<Rigidbody>().useGravity = true;
                    tuboInstanciado.GetComponent<BoxCollider>().enabled = true;
                    tuboInstanciado.GetComponent<CapsuleCollider>().enabled = true;
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
                        tuboInstanciado.GetComponent<BoxCollider>().enabled = false;
                        tuboInstanciado.GetComponent<CapsuleCollider>().enabled = false;

                        listaDeBabosasScript.ActualizarInventarioTubos();

                        if (listaDeBabosasScript.BuscarBabosas())
                        {
                            int numeroEstado = listaDeBabosasScript.obtenerEstadoBabosaInventario();
                            if (listaDeBabosasScript.SacarBabosaInventario())
                            {
                                babosaInstanciada = Instantiate(Babosa, Cartucho.position, Cartucho.rotation);
                                babosaInstanciada.GetComponent<Item>().Estado = numeroEstado;
                                babosaInstanciada.GetComponent<Animator>().SetBool("Disparando", true);
                                babosaInstanciada.transform.SetParent(transform, true);
                                babosaInstanciada.GetComponent<BoxCollider>().enabled = false;
                                babosaInstanciada.GetComponent<IABabosaMovimiento>().enabled = false;
                                babosaInstanciada.GetComponent<PickableObject>().isPickable = false;
                                babosaInstanciada.GetComponent<Rigidbody>().useGravity = false;
                                cargado = true;

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
            reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
            reproductorAudio.GetComponent<ReproductorAudio>().disparar();
            reproductorAudio = null;

            transform.GetChild(0).gameObject.SetActive(true);
            babosaInstanciada.GetComponent<BabosaEsDisparada>().enabled = true;
            babosaInstanciada.GetComponent<BabosaEsDisparada>().babosaDisparada = true;
            babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
            babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            babosaInstanciada.GetComponent<BoxCollider>().enabled = true;
            babosaInstanciada.GetComponent<Rigidbody>().AddForce(Cartucho.up*fuerzaDisparo);
            babosaInstanciada.transform.SetParent(null, true);

            StartCoroutine(esperaRecarga());
            babosaInstanciada = null;
        }

        if (!controladorArmaMano)
        {
            if (transform.parent.GetComponent<MeshRenderer>().enabled)
            {
                transform.parent.GetComponent<MeshRenderer>().enabled = false;

                if (tuboInstanciado != null)
                {
                    tuboInstanciado.SetActive(false);
                }
            }
        }
        else
        {
            if (!transform.parent.GetComponent<MeshRenderer>().enabled)
            {
                reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
                reproductorAudio.GetComponent<ReproductorAudio>().activarArma();
                reproductorAudio = null;

                transform.parent.GetComponent<MeshRenderer>().enabled = true;

                if (tuboInstanciado != null)
                {
                    tuboInstanciado.SetActive(true);
                }
            }
        }
    }

    IEnumerator esperaRecarga()
    {
        yield return new WaitForSeconds(0.5f);
        cargado = false;
    }
}