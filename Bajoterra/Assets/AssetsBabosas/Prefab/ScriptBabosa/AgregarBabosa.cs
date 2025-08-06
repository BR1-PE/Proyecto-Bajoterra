using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgregarBabosa : MonoBehaviour
{
    public GameObject babosa;

    void Update()
    {
        if (babosa != null)
        {
            var scripts = gameObject.GetComponents<MonoBehaviour>();

            foreach (var script in scripts)
            {
                var tipo = script.GetType();

                var campo = tipo.GetField("babosa");

                if (campo != null)
                {
                    var valorAnterior = campo.GetValue(script);
                    campo.SetValue(script, babosa);
                }
            }
            babosa = null;
        }
    }
}
