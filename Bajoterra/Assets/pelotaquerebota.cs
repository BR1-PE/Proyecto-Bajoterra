using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pelotaquerebota : MonoBehaviour
{
    public float fuerza;
    public float tiempo;
    public Rigidbody rb;
    
    Coroutine a;
    public bool pausa = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        a = StartCoroutine(pausaysalto());
    }

    // Update is called once per frame
    void Update()
    {
        if (pausa) StopCoroutine(a);
    }

    public IEnumerator pausaysalto(){
        while (true){
            rb.AddForce(fuerza * transform.up, ForceMode.Impulse);
            /*
            ForceMode.Impulse
            ForceMode.VelocityChange
            ForceMode.AcelerationChange
            */
            yield return new WaitForSeconds(tiempo);
        }        
    }
}
