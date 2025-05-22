using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject ObjectToPickUp;
    public GameObject PickedObject;
    public GameObject objetoComida;
    public Transform interactionZone;
    public Inventario listaDeBabosasScript;
    public bool controladorArmaMano = false;
    public bool soltar = false;
    public bool soltarBabosa = false;

    void Update()
    {
        if(ObjectToPickUp != null && ObjectToPickUp.GetComponent<PickableObject>().isPickable == true && PickedObject == null && !controladorArmaMano)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickedObject = ObjectToPickUp;

                if (PickedObject.GetComponent<CerebroBabosa>() != null)
                {
                    PickedObject.GetComponent<CerebroBabosa>().enMano = true;

                    PickedObject.GetComponent<Animator>().SetBool("Caminando", false);
                    PickedObject.GetComponent<Animator>().SetBool("Corriendo", false);
                    PickedObject.GetComponent<Animator>().SetBool("Saltando", false);
                    PickedObject.GetComponent<Animator>().SetBool("Flotando", true);
                    PickedObject.GetComponent<Animator>().SetBool("Cayendo1", true);
                }
                else
                {
                    PickedObject.GetComponent<Rigidbody>().useGravity = false;
                    PickedObject.GetComponent<Rigidbody>().isKinematic = true;

                    if (PickedObject.GetComponent<BoxCollider>() != null)
                    {
                        PickedObject.GetComponent<BoxCollider>().enabled = false;
                    }
                    if (PickedObject.GetComponent<SphereCollider>() != null)
                    {
                        PickedObject.GetComponent<SphereCollider>().enabled = false;
                    }
                    if (PickedObject.GetComponent<CapsuleCollider>() != null)
                    {
                        PickedObject.GetComponent<CapsuleCollider>().enabled = false;
                    }
                }
                
                PickedObject.GetComponent<PickableObject>().isPickable = false;
                PickedObject.transform.SetParent(interactionZone, true);
                
                PickedObject.transform.position = interactionZone.position;
                PickedObject.transform.rotation = interactionZone.rotation;

            }
        }
        else if (PickedObject != null)
        {
            if (PickedObject.GetComponent<CerebroBabosa>() != null)
            {
                soltarBabosa = PickedObject.GetComponent<CerebroBabosa>().enMano;
            }
            else
            {
                soltarBabosa = true;
            }

            if (Input.GetKeyDown(KeyCode.F) || controladorArmaMano || soltar || !soltarBabosa)
            {
                PickedObject.transform.SetParent(null, true);

                if (PickedObject.GetComponent<CerebroBabosa>() != null)
                {
                    PickedObject.GetComponent<CerebroBabosa>().enMano = false;
                }
                else
                {
                    PickedObject.GetComponent<Rigidbody>().useGravity = true;
                    PickedObject.GetComponent<Rigidbody>().isKinematic = false;

                    if (PickedObject.GetComponent<BoxCollider>() != null)
                    {
                        PickedObject.GetComponent<BoxCollider>().enabled = true;
                    }
                    if (PickedObject.GetComponent<SphereCollider>() != null)
                    {
                        PickedObject.GetComponent<SphereCollider>().enabled = true;
                    }
                    if (PickedObject.GetComponent<CapsuleCollider>() != null)
                    {
                        PickedObject.GetComponent<CapsuleCollider>().enabled = true;
                    }
                }

                PickedObject.GetComponent<PickableObject>().isPickable = true;
                PickedObject.transform.SetParent(null, true);

                PickedObject = null;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Item nuevoItem = PickedObject.GetComponent<Item>();

                string NombreItem = PickedObject.transform.name.Replace("(Clone)", "");

                if (nuevoItem != null)
                {
                    if (PickedObject.GetComponent<Item>().esBabosa)
                    {
                        Debug.Log("Intentando guardar");
                        bool ApilableItem = PickedObject.GetComponent<Item>().Apilable;
                        RenderTexture TexturaItem = PickedObject.GetComponent<Item>().Textura;
                        int EstadoItem = PickedObject.GetComponent<Item>().Estado;

                        nuevoItem.Nombre = NombreItem;
                        nuevoItem.Apilable = ApilableItem;
                        nuevoItem.Textura = TexturaItem;
                        nuevoItem.Estado = EstadoItem;

                        if (listaDeBabosasScript.AgregarItemBabosa(nuevoItem))
                        {
                            Destroy(PickedObject);
                            PickedObject = null;
                        }

                        listaDeBabosasScript.ActualizarInventarioBabosas();
                    }
                    else
                    {
                        bool ApilableItem = PickedObject.GetComponent<Item>().Apilable;
                        RenderTexture TexturaItem = PickedObject.GetComponent<Item>().Textura;
                        int CantidadItem = PickedObject.GetComponent<Item>().Cantidad;

                        nuevoItem.Nombre = NombreItem;
                        nuevoItem.Apilable = ApilableItem;
                        nuevoItem.Textura = TexturaItem;

                        if (listaDeBabosasScript.AgregarItemObjeto(nuevoItem))
                        {
                            Destroy(PickedObject);
                            PickedObject = null;
                        }
                    }

                    listaDeBabosasScript.ActualizarMochila();
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && PickedObject == null && !controladorArmaMano)
        {
            if (listaDeBabosasScript.SacarComidaInventario())
            {
                PickedObject = Instantiate(objetoComida, transform.position, transform.rotation);

                PickedObject.GetComponent<PickableObject>().isPickable = false;

                if (PickedObject.transform.parent != null)
                {
                    if (PickedObject.transform.parent.gameObject != null)
                    {
                        Destroy(PickedObject.transform.parent.gameObject);
                    }
                }

                PickedObject.transform.SetParent(interactionZone, true);
                PickedObject.GetComponent<Rigidbody>().useGravity = false;
                PickedObject.GetComponent<Rigidbody>().isKinematic = true;

                if (PickedObject.GetComponent<BoxCollider>() != null)
                {
                    PickedObject.GetComponent<BoxCollider>().enabled = false;
                }
                if (PickedObject.GetComponent<SphereCollider>() != null)
                {
                    PickedObject.GetComponent<SphereCollider>().enabled = false;
                }
                if (PickedObject.GetComponent<CapsuleCollider>() != null)
                {
                    PickedObject.GetComponent<CapsuleCollider>().enabled = false;
                }

                PickedObject.transform.position = interactionZone.position;
                PickedObject.transform.rotation = interactionZone.rotation;
            }

            listaDeBabosasScript.ActualizarMochila();
        }
        soltar = false;
    }
}