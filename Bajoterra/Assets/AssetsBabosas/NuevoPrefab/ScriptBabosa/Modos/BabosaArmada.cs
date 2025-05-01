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
        Debug.Log("Modo: Interesado");
        subMaquina.ChangeState(new EstadoPerseguir(subMaquina, babosa));
    }

    public void Update()
    {
        subMaquina.Update();
        if (babosa.ejecutado)
        {
            subMaquina.ChangeState(new EstadoDesinteresado(subMaquina, babosa));
        }
    }

    public void SalirModo()
    {
        Debug.Log("Saliendo del modo Interesado");
    }

    //------------------------------
    // SUBESTADOS DE COMPORTAMIENTO
    //------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoPerseguir : IState
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoPerseguir(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Persiguiendo");
        }

        public void Update()
        {
            if (babosa.player != null)
            {
                babosa.ir(babosa.player, 2);
            }
            if (babosa.comida != null)
            {
                maquina.ChangeState(new EstadoHambreado(maquina, babosa));
            }

        }

        public void Exit()
        {
            
        }
    }
    private class EstadoRecogida : IState
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoRecogida(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Recogida");
        }

        public void Update()
        {
            if (babosa.player != null)
            {
                babosa.ir(babosa.player, 2);
            }

        }

        public void Exit()
        {
            
        }
    }
    private class EstadoHambreado : IState
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoHambreado(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Hambreado");
        }

        public void Update()
        {
            if (babosa.comida != null)
            {
                babosa.ir(babosa.comida, 2);
            }
            if (!babosa.agente.pathPending && babosa.agente.remainingDistance <= babosa.agente.stoppingDistance)
            {
                Debug.Log("Comiendo");
                babosa.CambiarModo(new BabosaDomesticada(babosa));

            }
            else if (babosa.comida == null)
            {
                maquina.ChangeState(new EstadoPerseguir(maquina, babosa));
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoDesinteresado : IState
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoDesinteresado(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Desinteresado");
        }

        public void Update()
        {
            if (babosa.player != null)
            {
                babosa.ir(babosa.player, 2);   
            }

        }

        public void Exit()
        {
            
        }
    }
}