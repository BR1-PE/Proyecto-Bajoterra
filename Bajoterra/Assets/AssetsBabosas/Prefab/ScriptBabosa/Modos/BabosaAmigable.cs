using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaAmigable : IModoBabosa
{
    private CerebroBabosa babosa;
    private StateMachine subMaquina;

    public BabosaAmigable(CerebroBabosa babosa)
    {
        this.babosa = babosa;
        subMaquina = new StateMachine();
    }

    public void EntrarModo()
    {
        Debug.Log("Modo: Amigable");
        babosa.ComprobarEstado(3);
        subMaquina.ChangeState(new EstadoGeneracion(subMaquina, babosa));
    }

    public void Update()
    {
        subMaquina.Update();
    }

    public void SalirModo()
    {
        Debug.Log("Saliendo del modo Amigable");
    }

    //------------------------------
    // SUBESTADOS DE COMPORTAMIENTO
    //------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoPatrulla : IState
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoPatrulla(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }
        public void Enter()
        {
            Debug.Log("Estado: Patrulla");
            babosa.StartCoroutine(babosa.Mueve());
        }

        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoObtenida(maquina, babosa));
            if (babosa.transform.position.y < -0.2f) babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
        }

        public void Exit()
        {
            babosa.StopAllCoroutines();
        }
    }
    private class EstadoObtenida : IState //La babosa ejecuta su defensa
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoObtenida(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }

        public void Enter()
        {
            Debug.Log("Estado: Agarrada");
            babosa.EliminarBot();
            babosa.DesactivarFisicas();
            babosa.Animar("Existiendo", 0.05f);
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                babosa.sujetada = false;
                babosa.ActivarFisicas();
                babosa.CambiarModo(new BabosaDomesticada(babosa));
            }
            if (!babosa.sujetada) maquina.ChangeState(new EstadoGeneracion(maquina, babosa));
        }
        public void Exit()
        {
            babosa.ActivarFisicas();
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
            if (babosa.sujetada) maquina.ChangeState(new EstadoObtenida(maquina, babosa));
            if (babosa.neoBot != null) maquina.ChangeState(new EstadoPatrulla(maquina, babosa));
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
        }
    }
}