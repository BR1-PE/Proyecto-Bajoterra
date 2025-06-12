using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoProcedural : MonoBehaviour
{
    [SerializeField] private Transform[] posNeutro;
    [SerializeField] private Transform[] pata;
    [SerializeField] private float distDetector;
    [SerializeField] private float velPata;
    [SerializeField] private float distPata;
    [SerializeField] private float fuerzaCuerpo;
    [SerializeField] private float fuerzaPata;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * fuerzaCuerpo, ForceMode.Force);
        for (int i = 0; i < posNeutro.Length; i++)
        {
            detectarPiso(posNeutro[i], pata[i]);
        }
    }

    void detectarPiso(Transform puntoInicio, Transform posPata)
    {
        Ray ray = new Ray(puntoInicio.position, -puntoInicio.up);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * distDetector, Color.red);
        if (Physics.Raycast(ray, out hit, distDetector))
        {
            moverPata(hit.point, posPata);    
        }
    }

    void moverPata(Vector3 impacto, Transform posPata)
    {
        Vector3 direccion = impacto - posPata.position;
        float distancia = direccion.magnitude;
        if (distancia > distPata)
        {
            posPata.gameObject.GetComponent<Rigidbody>().AddForce(direccion.normalized * fuerzaPata, ForceMode.Force);
        }
    }
}