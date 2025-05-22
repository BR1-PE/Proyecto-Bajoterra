using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    public Transform Cartucho;
    public Transform objetivo;
    public float fuerzaDisparo = 500.0f;
    GameObject babosaInstanciada = null;
    public GameObject[] Babosa;
    bool disparo = false;
    public GameObject Audio;
    GameObject reproductorAudio;
    void Update()
    {
        if (!disparo)
        {
            StartCoroutine(apuntaDispara());
        }
        transform.LookAt(objetivo);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    IEnumerator apuntaDispara()
    {
        int randomBabosa = Random.Range(0, Babosa.Length);
        disparo = true;
        int random = Random.Range(2,5);
        yield return new WaitForSeconds(random);

        babosaInstanciada = Instantiate(Babosa[randomBabosa], Cartucho.position, Cartucho.rotation);
        babosaInstanciada.GetComponent<Animator>().SetBool("Disparando", true);
        babosaInstanciada.transform.SetParent(transform, true);
        babosaInstanciada.GetComponent<BoxCollider>().enabled = false;
        babosaInstanciada.GetComponent<PickableObject>().isPickable = false;
        babosaInstanciada.GetComponent<Rigidbody>().useGravity = false;

        yield return new WaitForSeconds(3);
        
        reproductorAudio = Instantiate(Audio, transform.position, transform.rotation);
        reproductorAudio.GetComponent<ReproductorAudio>().disparar();
        reproductorAudio = null;
        
        babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
        babosaInstanciada.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        babosaInstanciada.GetComponent<BoxCollider>().enabled = true;
        babosaInstanciada.GetComponent<Rigidbody>().AddForce(Cartucho.up*fuerzaDisparo);
        babosaInstanciada.transform.SetParent(null, true);

        disparo = false;

    }
}