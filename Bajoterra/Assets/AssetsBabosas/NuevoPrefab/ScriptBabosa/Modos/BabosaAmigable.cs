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
        subMaquina.ChangeState(new EstadoPatrulla(subMaquina, babosa));
        babosa.ejecutadoBabosa = false;
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
            subMaquina.ChangeState(new EstadoObtenida(subMaquina, babosa));
        }
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
            }
        }

        public void Exit()
        {
            babosa.matarCoroutine = true;
            babosa.ejecutado = false;
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
            babosa.animar("Existiendo", 0.05f);
            Debug.Log("Estado: Agarrada");
            babosa.destruir(false, true);
            babosa.alternarRb(babosa.enMano);
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                babosa.enMano = false;
                babosa.alternarRb(babosa.enMano);
                babosa.CambiarModo(new BabosaDomesticada(babosa));
            }
        }
        public void Exit()
        {
            babosa.ejecutadoBabosa = false;
            babosa.alternarRb(babosa.enMano);
        }
    }
}