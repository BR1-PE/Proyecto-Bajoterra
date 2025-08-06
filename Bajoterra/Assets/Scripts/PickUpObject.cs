using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Permite agarrar objetos o babosas
/// </summary>
public class PickUpObject : MonoBehaviour
{
    public GameObject ObjectToPickUp;
    public GameObject PickedObject;
    public GameObject objetoComida;
    public Transform interactionZone;
    public Inventario listaDeBabosasScript;
    public bool armaActiva = false;
    public float velocidad;

    private CerebroBabosa CB;
    private Rigidbody Rb;
    private Collider[] Col;
    private bool hold;
    private bool estaPresionando;
    private float tiempoPresionado;

    void Update()
    {
        if (ObjectToPickUp != null && ObjectToPickUp.GetComponent<PickableObject>().isPickable == true && PickedObject == null && !armaActiva)
        {
            if (Input.GetMouseButtonUp(0))
            {
                AgarrarObjeto();
            }
        }
        else if (PickedObject != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                estaPresionando = true;
                hold = false;
                tiempoPresionado = 0f;
            }
            if (estaPresionando)
            {
                tiempoPresionado += Time.deltaTime;
                if (tiempoPresionado >= 0.3f && !hold) hold = true;
            }
            if (Input.GetMouseButtonUp(0) || armaActiva)
            {
                if (hold) LanzarObjeto();
                else SoltarObjeto();

                PickedObject = null;
                estaPresionando = false;
                tiempoPresionado = 0f;
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (GuardarObjeto())
                {
                    LimpiarDatos();
                    Destroy(PickedObject);
                    PickedObject = null;
                    estaPresionando = false;
                    tiempoPresionado = 0f;
                }
            }

            if (CB != null)
            {
                if (!CB.sujetada)
                {
                    SoltarObjeto();
                    PickedObject = null;
                    estaPresionando = false;
                    tiempoPresionado = 0f;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) && PickedObject == null && !armaActiva)
        {
            if (listaDeBabosasScript.SacarComidaInventario())
            {
                ObjectToPickUp = Instantiate(objetoComida, transform.position, transform.rotation);
                AgarrarObjeto();
            }

            listaDeBabosasScript.ActualizarMochila();
        }
    }
    /// <summary>
    /// Posiciona el objeto en la mano del jugador
    /// </summary>
    private void AgarrarObjeto()
    {
        Vector3 offset = Vector3.zero;
        PickedObject = ObjectToPickUp;
        CB = PickedObject.GetComponent<CerebroBabosa>();

        if (CB != null)
        {
            CB.sujetada = true;
            offset = new Vector3(0f, 0.2f, 0f);
        }
        else
        {
            Rb = PickedObject.GetComponent<Rigidbody>();
            Col = PickedObject.GetComponents<Collider>();

            Rb.useGravity = false;
            Rb.isKinematic = true;

            foreach (Collider c in Col) c.enabled = false;
        }
        PickedObject.GetComponent<PickableObject>().isPickable = false;
        PickedObject.transform.SetParent(interactionZone, true);

        PickedObject.transform.position = interactionZone.position - offset;
        PickedObject.transform.rotation = interactionZone.rotation;
    }
    /// <summary>
    /// Deja caer el objeto o babosa que tenía en mano
    /// </summary>
    private void SoltarObjeto()
    {
        if (CB != null)
        {
            CB.sujetada = false;
        }
        else
        {
            Rb.useGravity = true;
            Rb.isKinematic = false;
            foreach (Collider c in Col) c.enabled = true;
        }
        PickedObject.GetComponent<PickableObject>().isPickable = true;
        PickedObject.transform.SetParent(null, true);
        LimpiarDatos();
    }
    /// <summary>
    /// Lanza el objeto o babosa que tenía en mano
    /// </summary>
    private void LanzarObjeto()
    {
        if (CB != null)
        {
            CB.sujetada = false;
            CB.ActivarFisicas();
            Rb = PickedObject.GetComponent<Rigidbody>();
        }
        else
        {
            Rb.useGravity = true;
            Rb.isKinematic = false;
            foreach (Collider c in Col) c.enabled = true;
        }
        PickedObject.GetComponent<PickableObject>().isPickable = true;
        PickedObject.transform.SetParent(null, true);
        Rb.AddForce(-interactionZone.forward * velocidad, ForceMode.VelocityChange);
        LimpiarDatos();
    }
    /// <summary>
    /// Guarda el objeto o babosa como información en el inventario
    /// </summary>
    /// <returns>Devuelve verdadero si se logró guardar con éxito</returns>
    private bool GuardarObjeto()
    {
        Item nuevoItem = PickedObject.GetComponent<Item>();
        string NombreItem = PickedObject.transform.name.Replace("(Clone)", "");
        nuevoItem.Nombre = NombreItem;

        if (nuevoItem != null)
        {
            if (PickedObject.GetComponent<Item>().esBabosa)
            {
                if (listaDeBabosasScript.AgregarItemBabosa(nuevoItem))
                {
                    listaDeBabosasScript.ActualizarInventarioBabosas();
                    return true;
                }
            }
            else if (listaDeBabosasScript.AgregarItemObjeto(nuevoItem))
            {
                listaDeBabosasScript.ActualizarMochila();
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Limpia los datos sobre el objeto o babosa
    /// </summary>
    private void LimpiarDatos()
    {
        CB = null;
        Rb = null;
        Col = null;
    }
}