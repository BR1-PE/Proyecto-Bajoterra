using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaDisparada : IModoBabosa
{
    private CerebroBabosa babosa;
    private StateMachine subMaquina;

    public BabosaDisparada(CerebroBabosa babosa)
    {
        this.babosa = babosa;
        subMaquina = new StateMachine();
    }

    public void EntrarModo()
    {
        Debug.Log("Modo: Disparada");
        babosa.ejecutado1 = false;
        babosa.contadorAtaques = 0;
        babosa.tiempo = false;
        babosa.ejecutadoBabosa = false;
        babosa.StartCoroutine(babosa.temporizador(babosa.tiempoTransformacion, 0));
        subMaquina.ChangeState(new EstadoVolando(subMaquina, babosa));
    }

    public void Update()
    {
        if (babosa.transformacion != null)
        {
            babosa.transformacion.transform.position = babosa.transform.position;
            babosa.transformacion.transform.rotation = babosa.transform.rotation;
        }
        if (babosa.tiempo && !babosa.ejecutadoBabosa && babosa.contadorAtaques < 5)
        {
            if (babosa.Natural != null)
            {
                subMaquina.ChangeState(new EstadoNatural(subMaquina, babosa));
            }
            else
            {
                babosa.ejecutadoBabosa = true;
                subMaquina.ChangeState(new EstadoDestransformada(subMaquina, babosa));
            }
        }
        else
        {
            subMaquina.Update();
        }
    }

    public void SalirModo()
    {
        babosa.ejecutadoBabosa = false;
        babosa.ejecutado1 = false;
        babosa.contadorAtaques = 0;
        Debug.Log("Saliendo del modo Disparado");
    }

    //------------------------------
    // SUBESTADOS DE COMPORTAMIENTO
    //------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoVolando : IState //La babosa esta transformada y en el aire
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoVolando(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Volando");
            babosa.ejecutadoBabosa = false;
        }

        public void Update()
        {
            babosa.detectarObjetivo(15f, 15f, 0.5f);
            if (babosa.distanciaObjetivo < 15f && babosa.contadorAtaques < 5)
            {
                if (babosa.distanciaObjetivo < 1.5f && babosa.Impactar != null && babosa.ataqueImpactar)
                {
                    Debug.Log("Estado: Intento impactar");
                    babosa.ataqueImpactar = false;
                    maquina.ChangeState(new EstadoImpactar(maquina, babosa));
                }
                else if (babosa.distanciaObjetivo < 5f && babosa.Golpear != null && babosa.ataqueGolpear)
                {
                    Debug.Log("Estado: Intento golpear");
                    babosa.ataqueGolpear = false;
                    maquina.ChangeState(new EstadoGolpear(maquina, babosa));
                }
                else if (babosa.distanciaObjetivo > 10f && babosa.Apuntar != null && babosa.ataqueApuntar)
                {
                    Debug.Log("Estado: Intento apuntar");
                    babosa.ataqueApuntar = false;
                    maquina.ChangeState(new EstadoApuntar(maquina, babosa));
                }
            }
            else if (babosa.contadorAtaques >= 5 && !babosa.ejecutado1)
            {
                babosa.ejecutado1 = true;
                maquina.ChangeState(new EstadoDestransformada(maquina, babosa));
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoApuntar : IState //La babosa esta a punto de disparar
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoApuntar(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Apuntar");
            babosa.ejecutadoBabosa = true;
            GameObject Apuntar = babosa.instanciar(babosa.Apuntar, babosa.transform.position, babosa.transform.rotation);
            babosa.pasarDatos(Apuntar);
            babosa.ataqueApuntar = false;
        }

        public void Update()
        {
            if (babosa.ataqueApuntar)
            {
                maquina.ChangeState(new EstadoVolando(maquina, babosa));
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoGolpear : IState //La babosa esta a punto de golpear
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoGolpear(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Golpear");
            babosa.ejecutadoBabosa = true;
            GameObject Golpear = babosa.instanciar(babosa.Golpear, babosa.transform.position, babosa.transform.rotation);
            babosa.pasarDatos(Golpear);
            babosa.ataqueGolpear = false;
        }

        public void Update()
        {
            if (babosa.ataqueGolpear)
            {
                maquina.ChangeState(new EstadoVolando(maquina, babosa));
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoImpactar : IState //La babosa colisiona con algo que no puede golpear
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoImpactar(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Impactar");
            babosa.ejecutadoBabosa = true;
            GameObject Impactar = babosa.instanciar(babosa.Impactar, babosa.transform.position, babosa.transform.rotation);
            babosa.pasarDatos(Impactar);
            babosa.ataqueImpactar = false;
        }

        public void Update()
        {
            if (babosa.ataqueImpactar)
            {
                maquina.ChangeState(new EstadoVolando(maquina, babosa));
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoNatural : IState //La babosa no realizo ningun ataque
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoNatural(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Natural");
            babosa.ejecutadoBabosa = true;
            GameObject Natural = babosa.instanciar(babosa.Natural, babosa.transform.position, babosa.transform.rotation);
            babosa.pasarDatos(Natural);
            babosa.ataqueNatural = false;      
        }

        public void Update()
        {
            if (babosa.ataqueNatural)
            {
                maquina.ChangeState(new EstadoVolando(maquina, babosa));
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoDestransformada : IState //La babosa se destranforma
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoDestransformada(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Destranformada");
            babosa.GetComponent<SphereCollider>().enabled = false;
            babosa.destruir(false, false, true);
            babosa.animar("Cayendo", 0f);
            babosa.GetComponent<PickableObject>().isPickable = true;
            babosa.Rb.useGravity = true;
            babosa.arrojada = true;
        }

        public void Update()
        {
            babosa.transform.rotation = Quaternion.Euler(0, babosa.transform.eulerAngles.y, 0);
            if (babosa.contacto)
            {
                if (babosa.anguloContacto < 30f)
                {
                    babosa.arrojada = false;
                    babosa.contacto = false;
                    babosa.cambiarTag("Protoforma");
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