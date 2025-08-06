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
        babosa.Pick.isPickable = false;
        babosa.DesactivarFisicas();
        subMaquina.ChangeState(new EstadoRecargada(subMaquina, babosa));
    }

    public void Update()
    {
        subMaquina.Update();
    }

    public void SalirModo()
    {
        babosa.soltada = false;
        babosa.disparada = false;
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
            babosa.Animar("Volando", 0f);
        }

        public void Update()
        {
            if (babosa.soltada) maquina.ChangeState(new EstadoCayendo(maquina, babosa));
            if (babosa.disparada) maquina.ChangeState(new EstadoDisparada(maquina, babosa));
        }

        public void Exit()
        {

        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoCayendo : IState //La babosa sale de la recamara sin ser disparada
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoCayendo(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Cayendo");
            babosa.Pick.isPickable = true;
            babosa.ActivarFisicas();
            babosa.Animar("Cayendo", 0f);
        }

        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoSostenida(maquina, babosa));
            if (babosa.DetectarEntornoVuelo())
            {
                if (Vector3.Angle(babosa.normal, Vector3.up) < 30f)
                {
                    babosa.RestringeMovimiento();
                    babosa.transform.rotation = Quaternion.Euler(0, babosa.transform.eulerAngles.y, 0);
                    babosa.Animar("Aterrizaje1", 0f);
                }
            }
            if (babosa.FinAnimar("Aterrizaje1")) babosa.CambiarMaquinaEstados(babosa.ComprobarEstado(null));
        }
        public void Exit()
        {

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
            babosa.ActivarFisicas();
            babosa.Animar("Volando", 0f);
            babosa.StartCoroutine(babosa.TemporizadorTransformar(0.1f));
        }

        public void Update()
        {
            if (babosa.DetectarEntornoVuelo())
            {
                if (Vector3.Angle(babosa.normal, Vector3.up) < 30f)
                {
                    babosa.RestringeMovimiento();
                    babosa.transform.rotation = Quaternion.Euler(0, babosa.transform.eulerAngles.y, 0);
                    babosa.Animar("GolpeFinal", 0f);
                }
                maquina.ChangeState(new EstadoCayendoMal(maquina, babosa));
            }
            if (babosa.transformar)
            {
                if (babosa.Transformar())
                {
                    babosa.transformar = false;
                    babosa.StopAllCoroutines();
                    babosa.CambiarModo(new BabosaDisparada(babosa));
                }
            }
        }

        public void Exit()
        {

        }
    }
    private class EstadoCayendoMal : IState //La babosa sale de la recamara sin ser disparada
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoCayendoMal(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: CayendoMal");
            babosa.Pick.isPickable = true;
        }

        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoSostenida(maquina, babosa));
            if (babosa.DetectarEntornoVuelo())
            {
                if (Vector3.Angle(babosa.normal, Vector3.up) < 30f)
                {
                    babosa.RestringeMovimiento();
                    babosa.transform.rotation = Quaternion.Euler(0, babosa.transform.eulerAngles.y, 0);
                    babosa.Animar("GolpeFinal", 0f);
                }
            }
            if (babosa.FinAnimar("GolpeFinal")) babosa.CambiarMaquinaEstados(babosa.ComprobarEstado(null));
        }

        public void Exit()
        {

        }
    }
    private class EstadoSostenida : IState //La babosa ejecuta su defensa
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoSostenida(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Sostenida");
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