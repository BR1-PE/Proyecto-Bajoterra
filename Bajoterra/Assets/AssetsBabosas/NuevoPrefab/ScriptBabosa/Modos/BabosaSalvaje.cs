using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaSalvaje : IModoBabosa //Aplica para babosas recien instanciadas
{
    private CerebroBabosa babosa;
    private StateMachine subMaquina;

    public BabosaSalvaje(CerebroBabosa babosa)
    {
        this.babosa = babosa;
        subMaquina = new StateMachine();
    }

    public void EntrarModo()
    {
        Debug.Log("Modo: Salvaje");
        subMaquina.ChangeState(new EstadoTranquilo(subMaquina, babosa));
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
        if (babosa.enMano && !babosa.ejecutadoBabosa)
        {
            babosa.ejecutadoBabosa = true;
            subMaquina.ChangeState(new EstadoDefendiendo(subMaquina, babosa));
        }
    }

    public void SalirModo()
    {
        Debug.Log("Saliendo del modo Salvaje");
        babosa.comida = null;
    }

    //------------------------------
    // SUBESTADOS DE COMPORTAMIENTO
    //------------------------------
//--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoTranquilo : IState //La babosa esta quieta o deambulando
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoTranquilo(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Tranquilo");
            babosa.matarCoroutine = false;

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
                if (!babosa.agente.pathPending && babosa.agente.remainingDistance <= babosa.agente.stoppingDistance && !babosa.ejecutado)
                {
                    babosa.ejecutado = true;
                    babosa.StartCoroutine(babosa.espera());
                }

                if (babosa.playerCerca)
                {
                    maquina.ChangeState(new EstadoCorriendo(maquina, babosa));
                }
                if (babosa.comida != null)
                {
                    maquina.ChangeState(new EstadoInteresado(maquina, babosa));
                }
            }
        }
        public void Exit()
        {
            babosa.matarCoroutine = true;
            babosa.ejecutado = false;
        }
    }
//--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoCorriendo : IState //La babosa esta corriendo en direccion contraria a la del jugador
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoCorriendo(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Corriendo");
        }
        public void Update()
        {
            if (babosa.playerCerca)
            {
                babosa.corre();
            }
            if (!babosa.agente.pathPending && babosa.agente.remainingDistance <= babosa.agente.stoppingDistance)
            {
                maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
            }
            if (babosa.playerMuyCerca)
            {
                maquina.ChangeState(new EstadoSaltando(maquina, babosa));
            }
        }
        public void Exit()
        {

        }
    }
//--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoSaltando : IState //La babosa esta asustada
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoSaltando(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Saltando");
            babosa.ejecutado1 = false;
        }
        public void Update()
        {
            if (!babosa.ejecutado)
            {
                babosa.ejecutado = true;
                babosa.huye();
            }
            babosa.salta();
            if (babosa.ejecutado && !babosa.agente.pathPending && babosa.agente.remainingDistance <= babosa.agente.stoppingDistance)
            {
                babosa.ejecutado = false;
                if (!babosa.playerCerca)
                {
                    maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
                }
            }
            if (babosa.destino != null)
            {
                if (babosa.camino(babosa.destino) > 0f && babosa.camino(babosa.destino) < 15f)
                {
                    maquina.ChangeState(new EstadoEscondiendo(maquina, babosa));
                }
            }
        }
        public void Exit()
        {
            babosa.ejecutado = false;
            babosa.ejecutado1 = false;
        }
    }
//--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoEscondiendo : IState //La babosa busca un escondite
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoEscondiendo(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Escondiendo");
            babosa.matarCoroutine = false;
        }
        public void Update()
        {
            babosa.salta();
            babosa.ir(babosa.destino, 3);
            if (!babosa.agente.pathPending && babosa.agente.remainingDistance <= babosa.agente.stoppingDistance)
            {
                Debug.Log("Escondido");
                babosa.destruir(true, true);
            }
        }
        public void Exit()
        {
            babosa.matarCoroutine = true;
        }
    }
//--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoDefendiendo : IState //La babosa ejecuta su defensa
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoDefendiendo(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            babosa.animar("Existiendo", 0.05f);
            Debug.Log("Estado: Defendiendo");
            babosa.destruir(false, true);
            babosa.alternarRb(babosa.enMano);
        }
        public void Update()
        {
            if (!babosa.enMano)
            {
                maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
            }
        }
        public void Exit()
        {
            babosa.ejecutadoBabosa = false;
            babosa.alternarRb(babosa.enMano);
        }
    }
//--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoInteresado : IState //La babosa ve comida
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoInteresado(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Interesado");
        }
        public void Update()
        {
            if (babosa.comida != null)
            {
                babosa.ir(babosa.comida, 1);
            }
            if (!babosa.agente.pathPending && babosa.agente.remainingDistance <= babosa.agente.stoppingDistance)
            {
                Debug.Log("Comiendo");
                babosa.CambiarModo(new BabosaInteresada(babosa));

            }
            else if (babosa.comida == null || babosa.comida.parent != null)
            {
                maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
            }
            if (babosa.playerCerca)
            {
                maquina.ChangeState(new EstadoCorriendo(maquina, babosa));
            }
        }
        public void Exit()
        {
            babosa.comida = null;
            babosa.matarCoroutine = true;
        }
    }
}