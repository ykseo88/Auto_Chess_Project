using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitState
{
    
    public void Enter();

    public void Update();

    public void Exit();
}
