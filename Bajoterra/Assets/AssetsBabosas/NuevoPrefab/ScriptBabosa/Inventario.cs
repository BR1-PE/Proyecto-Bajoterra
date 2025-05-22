using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventario : MonoBehaviour
{
    public List<Item> listaDeBabosas = new List<Item>();
    public List<Item> listaDeObjetos = new List<Item>();
    public GameObject contenedorInventarioBabosas;
    public GameObject contenedorInventarioTubos;
    public GameObject[] contenedorBabosas;
    public GameObject[] contenedorObjetos;
    int largo = 9;

    public TextMeshProUGUI contadorTubos;
    public TextMeshProUGUI contadorComida;
    public TextMeshProUGUI contadorMonedas;

    public void ActualizarMochila()
    {
        if (listaDeObjetos.Count > 0)
        {
            int i = 0;
            foreach (var item in listaDeObjetos)
            {
                if (item.Nombre == "Tubo")
                {
                    contadorTubos.text = $"{item.Cantidad}";
                    break;
                }
                i++;
            }

            int j = 0;
            foreach (var item in listaDeObjetos)
            {
                if (item.Nombre == "Comida")
                {
                    contadorComida.text = $"{item.Cantidad}";
                    break;
                }
                j++;
            }
        }
    }

    //.....................................................................................................................................

    public bool AgregarItemBabosa(Item nuevoItem)
    {
        int i = 0;
        bool encontrado = false;
        foreach (var item in listaDeObjetos)
        {
            if (item.Nombre == "Tubo")
            {
                encontrado = true;
                break;
            }
            i++;
        }

        if (encontrado)
        {
            if (listaDeBabosas.Count < listaDeObjetos[i].Cantidad)
            {
                if (listaDeBabosas.Count < largo)
                {
                    listaDeBabosas.Add(nuevoItem);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;    
            }
        }
        else
        {
            return false;
        }
    }

    public bool BuscarBabosas()
    {
        if (listaDeBabosas.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ActualizarInventarioBabosas()
    {
        for(int k = 0; k < 9; k++)
        {
            Transform hijo = contenedorInventarioBabosas.transform.GetChild(k);

            Color colorActual = hijo.gameObject.GetComponent<RawImage>().color;
            colorActual.a = 0f;
            hijo.gameObject.GetComponent<RawImage>().color = colorActual;
            hijo.gameObject.GetComponent<RawImage>().texture = null;
        }

        for (int i = 0; i < listaDeBabosas.Count; i++)
        {
            Transform hijo = contenedorInventarioBabosas.transform.GetChild(i);

            if (hijo.gameObject.GetComponent<RawImage>().texture == null)
            {
                RenderTexture RenderActual = listaDeBabosas[i].Textura;
                hijo.gameObject.GetComponent<RawImage>().texture = RenderActual;

                Color colorActual = hijo.gameObject.GetComponent<RawImage>().color;
                colorActual.a = 1f;
                hijo.gameObject.GetComponent<RawImage>().color = colorActual;
            }
        }
    }

    public int obtenerEstadoBabosaInventario()
    {
        return listaDeBabosas[0].Estado;
    }

    public bool SacarBabosaInventario()
    {
        for (int j = 0; j < listaDeBabosas.Count ; j++)
        {
            Transform hijo = contenedorInventarioBabosas.transform.GetChild(j);

            if (hijo.gameObject.GetComponent<RawImage>().texture != null)
            {
                Color colorActual = hijo.gameObject.GetComponent<RawImage>().color;
                colorActual.a = 0f;
                hijo.gameObject.GetComponent<RawImage>().color = colorActual;
                hijo.gameObject.GetComponent<RawImage>().texture = null;
                listaDeBabosas.RemoveAt(j);
                return true;
            }
        }
        return false;
    }

    public GameObject obtenerBabosaInventario()
    {
        if (listaDeBabosas.Count >= 1)
        {
            Transform nombreHijo = contenedorInventarioBabosas.transform.GetChild(0);
            string nameHijo = nombreHijo.gameObject.GetComponent<RawImage>().texture.name;

            foreach (GameObject hijo in contenedorBabosas)
            {
                if (hijo.name == nameHijo)
                {
                    return hijo;
                }
            }
        }
        return null;
    }

    //.....................................................................................................................................

    public bool AgregarItemObjeto(Item nuevoItem)
    {
        if(listaDeObjetos.Count == 0)
        {
            listaDeObjetos.Add(nuevoItem);
        }

        if(nuevoItem.Apilable)
        {
            if (nuevoItem.Nombre == "Tubo")
            {
                bool encontrado = false;
                int i = 0;

                foreach (var item in listaDeObjetos)
                {
                    if (item.Nombre == "Tubo")
                    {
                        encontrado = true;
                        break;
                    }
                    i++;
                }
                if (encontrado)
                {
                    listaDeObjetos[i].Cantidad++;
                }
                else
                {
                    listaDeObjetos.Add(nuevoItem);
                    
                    i = 0;
                    foreach (var item in listaDeObjetos)
                    {
                        if (item.Nombre == "Tubo")
                        {
                            listaDeObjetos[i].Cantidad++;
                            break;
                        }
                        i++;
                    }
                }

                ActualizarInventarioTubos();
            }

            if (nuevoItem.Nombre == "Comida")
            {
                bool encontrado = false;
                int j = 0;

                foreach (var item in listaDeObjetos)
                {
                    if (item.Nombre == "Comida")
                    {
                        encontrado = true;
                        break;
                    }
                    j++;
                }
                if (encontrado)
                {
                    listaDeObjetos[j].Cantidad++;
                }
                else
                {
                    listaDeObjetos.Add(nuevoItem);

                    j = 0;
                    foreach (var item in listaDeObjetos)
                    {
                        if (item.Nombre == "Comida")
                        {
                            listaDeObjetos[j].Cantidad++;
                            break;
                        }
                        j++;
                    }
                }
            }
        }
        return true;
    }

    public void ActualizarInventarioTubos()
    {
        for(int k = 0; k < 9; k++)
        {
            Transform hijo = contenedorInventarioTubos.transform.GetChild(k);

            Color colorActual = hijo.gameObject.GetComponent<RawImage>().color;
            colorActual.a = 0f;
            hijo.gameObject.GetComponent<RawImage>().color = colorActual;
            hijo.gameObject.GetComponent<RawImage>().texture = null;
        }

        int i = 0;
        foreach (var item in listaDeObjetos)
        {
            if (item.Nombre == "Tubo")
            {
                break;
            }
            i++;
        }

        int cantidadTubos = listaDeObjetos[i].Cantidad;

        for (int k = 0; k < 9; k++)
        {
            if (k + 1 <= cantidadTubos)
            {
                Transform hijo = contenedorInventarioTubos.transform.GetChild(k);

                if (hijo.gameObject.GetComponent<RawImage>().texture == null)
                {
                    RenderTexture RenderActual = listaDeObjetos[i].Textura;
                    hijo.gameObject.GetComponent<RawImage>().texture = RenderActual;

                    Color colorActual = hijo.gameObject.GetComponent<RawImage>().color;
                    colorActual.a = 1f;
                    hijo.gameObject.GetComponent<RawImage>().color = colorActual;
                }
            }
        }
    }

    public bool BuscarTubos()
    {
        int i = 0;
        bool encontrado = false;
        foreach (var item in listaDeObjetos)
        {
            if (item.Nombre == "Tubo")
            {
                encontrado = true;
                break;
            }
            i++;
        }

        if (encontrado)
        {
            if (listaDeObjetos[i].Cantidad != 0)
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public bool SacarTuboInventario()
    {
        int i = 0;
        foreach (var item in listaDeObjetos)
        {
            if (item.Nombre == "Tubo")
            {
                break;
            }
            i++;
        }

        for (int j = 0; j < listaDeObjetos[i].Cantidad; j++)
        {
            Transform hijo = contenedorInventarioTubos.transform.GetChild(j);

            if (hijo.gameObject.GetComponent<RawImage>().texture != null)
            {
                Color colorActual = hijo.gameObject.GetComponent<RawImage>().color;
                colorActual.a = 0f;
                hijo.gameObject.GetComponent<RawImage>().color = colorActual;
                hijo.gameObject.GetComponent<RawImage>().texture = null;
                listaDeObjetos[i].Cantidad--;
                return true;
            }
        }
        return false;
    }

    public bool SacarComidaInventario()
    {
        if (listaDeObjetos.Count > 0)
        {
            int i = 0;
            foreach (var item in listaDeObjetos)
            {
                if (item.Nombre == "Comida")
                {
                    if (listaDeObjetos[i].Cantidad > 0)
                    {
                        listaDeObjetos[i].Cantidad--;
                        return true;
                    }
                    break;
                }
                i++;
            }
        }
        return false;
    }
}