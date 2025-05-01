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
        Debug.Log("Modo: Interesado");
        subMaquina.ChangeState(new EstadoPerseguir(subMaquina, babosa));
        babosa.StartCoroutine(babosa.temporizador(30f));
        babosa.ejecutado1 = false;
    }

    public void Update()
    {
        subMaquina.Update();
        if (babosa.neoBot != null)
        {
            if (babosa.transform.position.y < -0.2f)
            {
                babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
            }
        }
        if (babosa.tiempo && !babosa.ejecutado1)
        {
            subMaquina.ChangeState(new EstadoDesinteresado(subMaquina, babosa));
        }
        else
        {
            if (babosa.enMano && !babosa.ejecutadoBabosa)
            {
                babosa.ejecutadoBabosa = true;
                subMaquina.ChangeState(new EstadoRecogida(subMaquina, babosa));
            }   
        }
    }

    public void SalirModo()
    {
        Debug.Log("Saliendo del modo Interesado");
        babosa.tiempo = false;
        babosa.comida = null;
    }

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
            if (babosa.neoBot != null)
            {
                babosa.restablecerRuta();
            }
        }

        public void Update()
        {
            if (babosa.neoBot == null)
            {
                babosa.neoBot = babosa.bot();
            }
            else
            {
                if (babosa.player != null)
                {
                    if (babosa.playerMuyCerca)
                    {
                        babosa.ir(babosa.player, 1);
                    }
                    else
                    {
                        babosa.ir(babosa.player, 3);
                        babosa.salta();
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
            babosa.enMano = true;
            babosa.animar("Existiendo", 0.05f);
            Debug.Log("Estado: Recogida");
            babosa.destruir(false, true);
            babosa.alternarRb(babosa.enMano);
        }

        public void Update()
        {
            if (!babosa.enMano)
            {
                maquina.ChangeState(new EstadoPerseguir(maquina, babosa));
            }

        }

        public void Exit()
        {
            babosa.ejecutadoBabosa = false;
            babosa.enMano = false;
            babosa.alternarRb(babosa.enMano);
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
    }
}