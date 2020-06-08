using System;
using System.Collections.Generic;
using System.Linq;

public class UtilityAction
{
    #region variables

    public State utilityState;
    public bool HasSubmachine;
    public MathFunc function;
    private Action utilityAction;
    private UtilityCurvesEngine uCurvesEngine;

    #endregion

    public UtilityAction(string name, Action action, MathFunc func, UtilityCurvesEngine utilityCurvesEngine)
    {
        this.HasSubmachine = false;
        this.utilityAction = action;
        this.function = func;
        this.utilityState = new State(name, action, utilityCurvesEngine);
        this.uCurvesEngine = utilityCurvesEngine;
    }

    public UtilityAction(string name, State utilState, MathFunc func, BehaviourEngine behaviourEngine)
    {
        this.HasSubmachine = true;
        this.utilityState = utilState;
        this.function = func;
        this.uCurvesEngine = behaviourEngine as UtilityCurvesEngine;

    }

    public void Update()
    {
        //Action
        //Console.WriteLine("Estado " + utilityState.Name + " está siendo actualizado.");
    }

    public void Reset()
    {

    }

}
