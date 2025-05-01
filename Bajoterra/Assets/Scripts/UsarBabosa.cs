using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsarBabosa : MonoBehaviour
{/*
    public Transform pickableZone;
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
                
            }
        }
        if ((Input.GetMouseButtonDown(0) || !controladorArmaMano) && cargado && babosaInstanciada != null)
        {
            reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
            reproductorAudio.GetComponent<ReproductorAudio>().disparar();
            reproductorAudio = null;

            transform.GetChild(0).gameObject.SetActive(true);
            babosaInstanciada.GetComponent<BabosaDisparada>().enabled = true;
            babosaInstanciada.GetComponent<BabosaDisparada>().babosaDisparada = true;
            babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
            babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            babosaInstanciada.GetComponent<BoxCollider>().enabled = true;
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

    private bool sacarBabosa(){
        GameObject Babosa;
        Babosa = listaDeBabosasScript.obtenerBabosaInventario();

        if (listaDeBabosasScript.BuscarBabosas())
        {
            int numeroEstado = listaDeBabosasScript.obtenerEstadoBabosaInventario();
            if (listaDeBabosasScript.SacarBabosaInventario())
            {
                babosaInstanciada = Instantiate(Babosa, pickableZone.position, pickableZone.rotation);
                babosaInstanciada.GetComponent<Item>().Estado = numeroEstado;
                babosaInstanciada.GetComponent<Animator>().SetBool("Disparando", true);
                babosaInstanciada.transform.SetParent(pickableZone, true);
                babosaInstanciada.GetComponent<BoxCollider>().enabled = false;
                babosaInstanciada.GetComponent<IABabosaMovimiento>().enabled = false;
                babosaInstanciada.GetComponent<PickableObject>().isPickable = false;
                babosaInstanciada.GetComponent<Rigidbody>().useGravity = false;

                listaDeBabosasScript.ActualizarInventarioBabosas();
            }
        }
        else
        {
            Debug.Log("Debes tener babosas");
        }

        listaDeBabosasScript.ActualizarMochila();
    }

    IEnumerator esperaRecarga()
    {
        yield return new WaitForSeconds(0.5f);
        cargado = false;
    }*/
}