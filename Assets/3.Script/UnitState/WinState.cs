using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WinState : IUnitState
{
    private static readonly int WIN = Animator.StringToHash("Win");
    private UnitController unitController;
    private UnitData unitData;
    public WinState(UnitController unitController) => this.unitController = unitController;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    
    
    public void Enter()
    {
        animator = unitController.animator;
        unitController.transform.TryGetComponent(out unitData);
        animator.SetTrigger(WIN);
        navMeshAgent = unitController.agent;
        navMeshAgent.enabled = false;
        if(unitData.Team.teamName == "Player")GameManager.Instance.isPlayerWin = true;
        else GameManager.Instance.isEnemyWin = true;
        
    }

    public void Update()
    {
        
    }

    public void Exit()
    {

    }
}
