using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaInteresada : IModoBabosa
{
    private CerebroBabosa babosa;
    private StateMachine subMaquina;

    public BabosaInteresada(CerebroBabosa babosa)
    {
        this.babosa = babosa;
        subMaquina = new StateMachine();
    }

    public void EntrarModo()
    {

    }

    public void Update()
    {

    }

    public void SalirModo()
    {

    }/*

    //------------------------------
    // SUBESTADOS DE COMPORTAMIENTO
    //------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoPerseguir : IState //La babosa perseguira al jugador
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
            if (babosa.neoBot != null)
            {
                if (babosa.player != null)
                {
                    if (babosa.playerMuyCerca)
                    {
                        if (babosa.playerDemasiadoCerca)
                        {
                            babosa.ir(babosa.player, 0);    
                        }
                        else
                        {
                            babosa.ir(babosa.player, 1);
                        }
                    }
                    else
                    {
                        babosa.ir(babosa.player, 3);
                        babosa.Salta();
                    }
                }
                else
                {
                    babosa.ir(babosa.transform, 0);
                }
                if (babosa.comida != null)
                {
                    maquina.ChangeState(new EstadoHambreado(maquina, babosa));
                }   
            }
        }

        public void Exit()
        {
            
        }
    }
    private class EstadoRecogida : IState //La babosa esta en manos del jugador
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
            babosa.liberaTransform();
            babosa.sujetada = true;
            Debug.Log("Estado: Recogida");
            babosa.destruir(false, true, false);
            babosa.alternarRb(babosa.sujetada);
        }

        public void Update()
        {
            babosa.Animar("Existiendo", 0.05f);
            if (!babosa.sujetada)
            {
                maquina.ChangeState(new EstadoPerseguir(maquina, babosa));
            }

        }

        public void Exit()
        {
            babosa.ejecutadoBabosa = false;
            babosa.sujetada = false;
            babosa.alternarRb(babosa.sujetada);
        }
    }
    private class EstadoHambreado : IState //La babosa detecta comida
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
            babosa.Ir(babosa.comida, 2);
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
    private class EstadoDesinteresado : IState //Si no se le ha alimentado entonces la babosa se torna salvaje
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
            babosa.ejecutado1 = true;
        }

        public void Update()
        {
            if (babosa.tiempo)
            {
                babosa.CambiarModo(new BabosaSalvaje(babosa));
            }
        }
        
        public void Exit()
        {

        }
    }*/
}