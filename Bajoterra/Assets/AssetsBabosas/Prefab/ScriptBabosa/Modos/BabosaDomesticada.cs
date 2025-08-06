using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabosaDomesticada : IModoBabosa
{
    private CerebroBabosa babosa;
    private StateMachine subMaquina;

    public BabosaDomesticada(CerebroBabosa babosa)
    {
        this.babosa = babosa;
        subMaquina = new StateMachine();
    }

    public void EntrarModo()
    {
        Debug.Log("Modo: Domesticado");
        babosa.ComprobarEstado(2);
        subMaquina.ChangeState(new EstadoGeneracion(subMaquina, babosa));
    }

    public void Update()
    {
        subMaquina.Update();
    }

    public void SalirModo()
    {
        Debug.Log("Saliendo del modo Domesticado");
    }

    //------------------------------
    // SUBESTADOS DE COMPORTAMIENTO
    //------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoSeguir : IState
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoSeguir(StateMachine maquina, CerebroBabosa babosa)
        {
            this.maquina = maquina;
            this.babosa = babosa;
        }
        private bool salto;
        public void Enter()
        {
            Debug.Log("Estado: Seguir");
        }

        public void Update()
        {
            if (babosa.sujetada) maquina.ChangeState(new EstadoAgarrada(maquina, babosa));
            else if (babosa.player != null)
            {
                if (babosa.playerCerca)
                {
                    if (salto)
                    {
                        babosa.StopAllCoroutines();
                        salto = false;
                    }
                    if (babosa.playerDemasiadoCerca) babosa.Ir(babosa.player.position, 0);
                    else babosa.Ir(babosa.player.position, 1);
                }
                else
                {
                    babosa.Ir(babosa.player.position, 3);
                    if (!salto)
                    {
                        babosa.StartCoroutine(babosa.Salta());
                        salto = true;
                    }
                }
            }
            if (babosa.transform.position.y < -0.2f) babosa.transform.position = new Vector3(babosa.neoBot.transform.position.x, 0.1f, babosa.neoBot.transform.position.z);
        }

        public void Exit()
        {
            babosa.StopAllCoroutines();
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------
    private class EstadoAgarrada : IState //La babosa ejecuta su defensa
    {
        private StateMachine maquina;
        private CerebroBabosa babosa;

        public EstadoAgarrada(StateMachine maquina, CerebroBabosa babosa)
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
                babosa.CambiarModo(new BabosaAmigable(babosa));
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
            if (babosa.sujetada) maquina.ChangeState(new EstadoAgarrada(maquina, babosa));
            if (babosa.neoBot != null) maquina.ChangeState(new EstadoSeguir(maquina, babosa));
        }
        public void Exit()
        {
            babosa.StopAllCoroutines();
        }
    }
}