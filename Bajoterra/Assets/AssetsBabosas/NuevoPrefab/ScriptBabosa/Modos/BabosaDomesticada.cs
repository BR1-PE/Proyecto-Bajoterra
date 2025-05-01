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
        subMaquina.ChangeState(new EstadoSeguir(subMaquina, babosa));
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
            subMaquina.ChangeState(new EstadoAgarrada(subMaquina, babosa));
        }
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

        public void Enter()
        {
            Debug.Log("Estado: Seguir");
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
                    if (babosa.playerCerca)
                    {
                        babosa.ir(babosa.player, 2);
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
            }
        }

        public void Exit()
        {
            
        }
    }
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
                babosa.CambiarModo(new BabosaAmigable(babosa));
            }
        }
        public void Exit()
        {
            babosa.ejecutadoBabosa = false;
            babosa.alternarRb(babosa.enMano);
        }
    }
}