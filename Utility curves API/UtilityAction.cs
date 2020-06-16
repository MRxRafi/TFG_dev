using System;
using System.Collections.Generic;
using System.Linq;

public class UtilityAction
{
    #region variables

    public State utilityState;
    public bool HasSubmachine;
    private Factor factor;
    private Action utilityAction;
    private UtilityCurvesEngine uCurvesEngine;

    #endregion

    public UtilityAction(string name, Action action, Factor factor, UtilityCurvesEngine utilityCurvesEngine)
    {
        this.HasSubmachine = false;
        this.utilityAction = action;
        this.factor = factor;
        this.utilityState = new State(name, action, utilityCurvesEngine);
        this.uCurvesEngine = utilityCurvesEngine;
    }

    public UtilityAction(string name, State utilState, Factor factor, BehaviourEngine behaviourEngine)
    {
        this.HasSubmachine = true;
        this.utilityState = utilState;
        this.factor = factor;
        this.uCurvesEngine = behaviourEngine as UtilityCurvesEngine;

    }

    public float getUtility()
    {
        float utilityValue = factor.getValue();
        if (utilityValue > 1.0f) return 1.0f;
        if (utilityValue < 0.0f) return 0.0f;
        return utilityValue;
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
