using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pelota : MonoBehaviour
{
    public float fuerza;
    public float largo;
    public float velocidad;
    public Rigidbody rb;
    //public LayerMask capas;
    public bool salto;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    //Ray nombre = Ray(-transform.up, 1f);
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0f, 0f, 1f), Time.deltaTime * velocidad);
        Ray ray = new Ray(transform.position, transform.position - (transform.position + new Vector3(0f, 1f, 0f)));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, largo/*, ~0, QueryTriggerInteraction.Ignore*/) && !salto)
        {
            salto = true;
            rb.velocity = new Vector3(0f, 0f, 0f);
            rb.AddForce(fuerza * transform.up, ForceMode.Impulse);
        }
        else if (salto && rb.velocity.y < 1f && !Physics.Raycast(ray, out hit, largo/*, ~0, QueryTriggerInteraction.Ignore*/))
        {
            salto = false;
        }
        Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red);
    }
}
