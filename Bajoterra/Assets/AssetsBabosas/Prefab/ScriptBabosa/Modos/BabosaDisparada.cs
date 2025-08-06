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
        babosa.StartCoroutine(babosa.TemporizadorTransformar(babosa.tiempoTransformacion));
        subMaquina.ChangeState(new EstadoVolando(subMaquina, babosa));
    }

    public void Update()
    {
        subMaquina.Update();
        if (babosa.transformacion != null)
        {
            babosa.transformacion.transform.position = babosa.transform.position;
            babosa.transformacion.transform.rotation = babosa.transform.rotation;
        }
    }

    public void SalirModo()
    {
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
        }

        public void Update()
        {
            if (babosa.DesTransformar() || babosa.transformar) maquina.ChangeState(new EstadoDestransformada(maquina, babosa));
            babosa.detectarObjetivo(15f, 15f, 0.5f);
            if (babosa.distanciaObjetivo < 15f)
            {
                if (babosa.distanciaObjetivo < 1.5f && babosa.Impactar != null && babosa.ataqueImpactar)
                {
                    if (babosa.EjecutarAtaque(babosa.Impactar))
                    {
                        babosa.ataqueImpactar = false;
                        maquina.ChangeState(new EstadoImpactar(maquina, babosa));
                    }
                }
                else if (babosa.distanciaObjetivo < 5f && babosa.Golpear != null && babosa.ataqueGolpear)
                {
                    if (babosa.EjecutarAtaque(babosa.Golpear))
                    {
                        babosa.ataqueGolpear = false;
                        maquina.ChangeState(new EstadoGolpear(maquina, babosa));
                    }
                }
                else if (babosa.distanciaObjetivo > 10f && babosa.Apuntar != null && babosa.ataqueApuntar)
                {
                    if (babosa.EjecutarAtaque(babosa.Apuntar))
                    {
                        babosa.ataqueApuntar = false;
                        maquina.ChangeState(new EstadoApuntar(maquina, babosa));
                    }
                }
            }
            if (babosa.Natural != null && babosa.ataqueNatural)
            {
                if (babosa.EjecutarAtaque(babosa.Apuntar))
                {
                    babosa.ataqueApuntar = false;
                    maquina.ChangeState(new EstadoNatural(maquina, babosa));
                }
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
            babosa.Animar("Cayendo", 0f);
            babosa.ActivarFisicas();
            babosa.GetComponent<PickableObject>().isPickable = true;
        }

        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoSujetada(maquina, babosa));
            if (babosa.DetectarEntornoVuelo())
            {
                if (Vector3.Angle(babosa.normal, Vector3.up) < 30f)
                {
                    babosa.RestringeMovimiento();
                    babosa.cambiarTag("Protoforma");
                    babosa.transform.rotation = Quaternion.Euler(0, babosa.transform.eulerAngles.y, 0);
                    babosa.Animar("Aterrizaje2", 0f);
                }
            }
            if (babosa.FinAnimar("Aterrizaje2")) babosa.CambiarMaquinaEstados(babosa.ComprobarEstado(null));
        }

        public void Exit()
        {

        }
    }
    private class EstadoSujetada : IState //La babosa ejecuta su defensa
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoSujetada(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Sujetada");
            babosa.DesactivarFisicas();
            babosa.Animar("Existiendo", 0.05f);
        }
        public void Update()
        {
            if (!babosa.sujetada) babosa.CambiarMaquinaEstados(babosa.ComprobarEstado(null));
        }
        public void Exit()
        {
            babosa.ActivarFisicas();
        }
    }
}