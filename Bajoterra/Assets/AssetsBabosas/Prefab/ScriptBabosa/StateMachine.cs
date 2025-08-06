using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.Exit();     // Sale del estado actual si existe
        currentState = newState;  // Cambia al nuevo estado
        currentState.Enter();     // Entra al nuevo estado
    }

    public void Update()
    {
        currentState?.Update();   // Actualiza el estado actual
    }
}