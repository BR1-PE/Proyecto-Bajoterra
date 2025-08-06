using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Busca babosas para asignarle alimento
/// </summary>
public class DetectorComida : MonoBehaviour
{
    public GameObject babosa;
    private CerebroBabosa CB;
    public float tiempo = 5f;
    public float rango = 3f;
    public LayerMask capas;
    private bool suelto = false;
    private bool objetivo = false;

    void Start()
    {
        StartCoroutine(BuscarBabosa());
    }

    void Update()
    {
        suelto = GetComponent<PickableObject>().isPickable;

        if (objetivo && !suelto)
        {
            if (CB != null && CB.comida == transform)
            {
                CB.comida = null;
                objetivo = false;
            }
        }
        if (suelto && objetivo)
        {
            if (CB.comida != transform)
            {
                objetivo = false;
            }
        }
    }
    /// <summary>
    /// Se buscan posibles babosas dentro de un volumen y se le asigna la comida actual como destino si es que no tiene comida asignada o si está suelta
    /// </summary>
    private IEnumerator BuscarBabosa()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempo);
            if (suelto && !objetivo)
            {
                Collider[] Col = Physics.OverlapSphere(transform.position, rango, capas);

                if (Col.Length > 0)
                {
                    foreach (Collider c in Col)
                    {
                        babosa = c.gameObject;
                        CB = babosa.GetComponent<CerebroBabosa>();
                        if (CB.comida != null) continue;
                        if (suelto)
                        {
                            CB.comida = transform;
                            objetivo = true;
                        }
                        else
                        {
                            if (CB.comida == transform)
                            {
                                CB.comida = null;
                            }
                            objetivo = false;
                        }
                        break;
                    }
                }
            }
        }
    }
}