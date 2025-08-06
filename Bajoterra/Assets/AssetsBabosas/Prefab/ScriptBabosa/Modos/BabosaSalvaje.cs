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
        babosa.ComprobarEstado(0);
        subMaquina.ChangeState(new EstadoGeneracion(subMaquina, babosa));
    }

    public void Update()
    {
        subMaquina.Update();
    }

    public void SalirModo()
    {
        Debug.Log("Saliendo del modo Salvaje");
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
            babosa.StartCoroutine(babosa.Mueve());
        }
        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoDefendiendo(maquina, babosa));
            if (babosa.playerCerca) maquina.ChangeState(new EstadoCorriendo(maquina, babosa));
            if (babosa.BuscarComida(60f)) maquina.ChangeState(new EstadoInteresado(maquina, babosa));
            if (babosa.transform.position.y < -0.2f) babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
            if (babosa.IntercambiarBot()) maquina.ChangeState(new EstadoTrepando(maquina, babosa));
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
        }
    }
    private class EstadoTrepando : IState //La babosa esta quieta o deambulando en paredes o techos
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoTrepando(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Trepando");
            babosa.StartCoroutine(babosa.Mueve());
        }
        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoDefendiendo(maquina, babosa));
            if (babosa.soltada)
            {
                babosa.transform.SetParent(null, true);
                babosa.EliminarBot();
                babosa.StopAllCoroutines();
                babosa.CambiarModo(new BabosaArmada(babosa));
            }
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
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
            babosa.StartCoroutine(babosa.Corre());
        }
        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoDefendiendo(maquina, babosa));
            if (babosa.playerMuyCerca) maquina.ChangeState(new EstadoSaltando(maquina, babosa));
            if (!babosa.playerCerca && babosa.RutaCompletada()) maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
            if (babosa.transform.position.y < -0.2f) babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
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
            babosa.StartCoroutine(babosa.Huye());
            babosa.StartCoroutine(babosa.Salta());
        }
        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoDefendiendo(maquina, babosa));
            if (!babosa.playerCerca && babosa.RutaCompletada()) maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
            if (babosa.BuscarEscondite(15f)) maquina.ChangeState(new EstadoEscondiendo(maquina, babosa));
            if (babosa.transform.position.y < -0.2f) babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
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
            babosa.StartCoroutine(babosa.Salta());
        }
        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoDefendiendo(maquina, babosa));
            if (!babosa.BuscarEscondite(15f)) maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
            if (babosa.transform.position.y < -0.2f) babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
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
            if (babosa.sujetada) maquina.ChangeState(new EstadoDefendiendo(maquina, babosa));
            if (!babosa.BuscarComida(60f)) maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
            if (babosa.LlegoComida()) babosa.CambiarModo(new BabosaDomesticada(babosa));
            if (babosa.playerCerca) maquina.ChangeState(new EstadoCorriendo(maquina, babosa));
            if (babosa.transform.position.y < -0.2f) babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
        }
        public void Exit()
        {
            babosa.comida = null;
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
            Debug.Log("Estado: Defendiendo");
            babosa.EliminarBot();
            babosa.DesactivarFisicas();
            babosa.Animar("Existiendo", 0.05f);
        }
        public void Update()
        {
            if (!babosa.sujetada) maquina.ChangeState(new EstadoGeneracion(maquina, babosa));
        }
        public void Exit()
        {
            babosa.ActivarFisicas();
            babosa.soltada = false;
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoGeneracion : IState //La babosa decide si va al piso o sube a una pared o techo
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoGeneracion(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Buscando terreno apropiado");
            babosa.StartCoroutine(babosa.GenerarBot());
        }
        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoDefendiendo(maquina, babosa));
            if (babosa.neoBot != null)
            {
                if (babosa.navMeshAgente != null) maquina.ChangeState(new EstadoTranquilo(maquina, babosa));
                else maquina.ChangeState(new EstadoTrepando(maquina, babosa));
            }
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
        }
    }
}