using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaArmada : IModoBabosa
{
    private CerebroBabosa babosa;
    private StateMachine subMaquina;

    public BabosaArmada(CerebroBabosa babosa)
    {
        this.babosa = babosa;
        subMaquina = new StateMachine();
    }

    public void EntrarModo()
    {
        Debug.Log("Modo: Armado");
        babosa.ejecutadoBabosa = false;
        subMaquina.ChangeState(new EstadoRecargada(subMaquina, babosa));
    }

    public void Update()
    {
        subMaquina.Update();
    }

    public void SalirModo()
    {
        babosa.arrojada = false;
        babosa.disparada = false;
        babosa.ejecutado = false;
        Debug.Log("Saliendo del modo Armado");
    }

    //------------------------------
    // SUBESTADOS DE COMPORTAMIENTO
    //------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoRecargada : IState //La babosa se encuentra en la recamara de la lanzadora
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoRecargada(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Recargada");
            babosa.alternarRb(true);
            if (babosa.destino != null)
            {
                babosa.colocar(babosa.destino, true, false);
                babosa.destino = null;
            }
            babosa.animar("Volando", 0.0f);
        }

        public void Update()
        {
            if (babosa.ejecutadoBabosa)
            {
                babosa.arrojada = true;
                maquina.ChangeState(new EstadoDescargada(maquina, babosa));
            }
            else if (Input.GetMouseButtonDown(0))
            {
                babosa.disparada = true;
                maquina.ChangeState(new EstadoDisparada(maquina, babosa));
            }
        }

        public void Exit()
        {
            babosa.contacto = false;
        }
    }
    private class EstadoDisparada : IState //La babosa es disparada
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoDisparada(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Disparada");
            babosa.alternarRb(false);
            babosa.liberaTransform();
            babosa.GetComponent<Rigidbody>().useGravity = false;
            babosa.animar("Volando", 0f);
            babosa.darFuerza(babosa.velocidadDisparo, 2);
            babosa.tiempo = false;
            babosa.StartCoroutine(babosa.temporizador(0.1f, 0));
            babosa.GetComponent<PickableObject>().isPickable = false;
            babosa.ejecutado = false;
        }

        public void Update()
        {
            if (babosa.finAnimar("GolpeFinal"))
            {
                babosa.GetComponent<PickableObject>().isPickable = true;
                switch (babosa.estadoItem(null))
                {
                    case 0: babosa.CambiarModo(new BabosaSalvaje(babosa)); break;
                    case 1: babosa.CambiarModo(new BabosaInteresada(babosa)); break;
                    case 2: babosa.CambiarModo(new BabosaDomesticada(babosa)); break;
                    case 3: babosa.CambiarModo(new BabosaAmigable(babosa)); break;
                }
            }
            if (babosa.Rb.useGravity)
            {
                if (Mathf.Abs(babosa.Rb.velocity.y) < 0.1f)
                {
                    babosa.animar("GolpeFinal", 0f);
                }
            }
            if (babosa.contacto)
            {
                babosa.Rb.useGravity = true;
                babosa.Rb.velocity *= 0.75f;
                babosa.transform.rotation = Quaternion.Euler(0, babosa.transform.eulerAngles.y, 0);
                babosa.Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                babosa.ejecutado = true;
                if (babosa.anguloContacto < 30f)
                {
                    babosa.animar("GolpeFinal", 0f);
                }
                else
                {
                    babosa.animar("CayendoMal", 0f);
                }
            }
            else if (babosa.tiempo && !babosa.Rb.useGravity)
            {
                if (babosa.Velociforma != null)
                {
                    Vector3 direccion = babosa.Rb.velocity.normalized;
                    babosa.transform.rotation = Quaternion.LookRotation(direccion);
                    babosa.transformacion = babosa.instanciar(babosa.Velociforma, babosa.transform.position, babosa.transform.rotation);
                    babosa.destino = babosa.transformacion.transform;
                    babosa.colocar(babosa.transform, false, true);
                    babosa.destino = null;
                    babosa.transform.localScale = new Vector3(1f, 1f, 1f);
                    babosa.transformacion.transform.localScale = new Vector3(1f, 1f, 1f);
                    babosa.cambiarTag("Velociforma");
                    babosa.GetComponent<SphereCollider>().enabled = true;
                    babosa.GetComponent<Rigidbody>().mass = 100f;

                    babosa.CambiarModo(new BabosaDisparada(babosa));
                }
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoDescargada : IState //La babosa sale de la recamara sin ser disparada
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoDescargada(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Descargada");
            babosa.alternarRb(false);
            babosa.animar("Cayendo", 0f);
            babosa.darFuerza(new Vector3(3f, 0f, 0f), 1);
            babosa.GetComponent<PickableObject>().isPickable = false;
        }

        public void Update()
        {
            babosa.transform.rotation = Quaternion.Euler(0, babosa.transform.eulerAngles.y, 0);
            if (babosa.contacto)
            {
                if (babosa.anguloContacto < 30f)
                {
                    babosa.GetComponent<PickableObject>().isPickable = true;
                    switch (babosa.estadoItem(null))
                    {
                        case 0: babosa.CambiarModo(new BabosaSalvaje(babosa)); break;
                        case 1: babosa.CambiarModo(new BabosaInteresada(babosa)); break;
                        case 2: babosa.CambiarModo(new BabosaDomesticada(babosa)); break;
                        case 3: babosa.CambiarModo(new BabosaAmigable(babosa)); break;
                    }
                }
            }
        }

        public void Exit()
        {
            
        }
    }
}