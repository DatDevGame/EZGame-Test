using System.Collections;
using System.Collections.Generic;
using Premium.EditableStateMachine;
using UnityEngine;

public class BaseBoxerStateSO : StateSO
{
    public List<TransitionSO> TransitionSO => transitions;

    public override void SetupState(object[] parameters = null){}

    protected override void StateDisable(){}

    protected override void StateEnable(){}

    protected override void StateUpdate(){}
}
