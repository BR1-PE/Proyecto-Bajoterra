using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsignarNavMesh : MonoBehaviour
{
    public GameObject BabosaBot;

    GameObject BabosaBotInstanciada = null;

    void Start()
    {
        if (transform.parent == null)
        {
            StartCoroutine(coldownBabosa());
            CrearNavMesh();
        }
    }

    IEnumerator coldownBabosa()
    {
        yield return new WaitForSeconds(2);
        GetComponent<PickableObject>().isPickable = true;
    }

    public void CrearNavMesh()
    {
        if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out UnityEngine.AI.NavMeshHit hit, 100f, UnityEngine.AI.NavMesh.AllAreas))
        {
            StartCoroutine(coldownBabosa());
            
            BabosaBotInstanciada = Instantiate(BabosaBot, hit.position, Quaternion.identity);
            Vector3 nuevaPosicion = new Vector3(transform.position.x, BabosaBotInstanciada.transform.position.y, transform.position.z);
            BabosaBotInstanciada.transform.position = nuevaPosicion;
            BabosaBotInstanciada.transform.rotation = transform.rotation;

            transform.SetParent(BabosaBotInstanciada.transform, true);
            transform.localPosition = new Vector3(0, transform.position.y, 0);

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            GetComponent<IABabosaMovimiento>().enabled = true;

            this.enabled = false;
        }
    }
}