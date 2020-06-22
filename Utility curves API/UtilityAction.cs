using System;
using System.Collections.Generic;
using System.Linq;

public class UtilityAction
{
    #region variables

    public State utilityState;
    public bool HasSubmachine;
    public BehaviourEngine subMachine;
    private Factor factor;
    private UtilityCurvesEngine uCurvesEngine;

    #endregion

    //Accion normal
    public UtilityAction(string name, Action action, Factor factor, UtilityCurvesEngine utilityCurvesEngine)
    {
        this.HasSubmachine = false;
        this.factor = factor;
        this.utilityState = new State(name, action, utilityCurvesEngine);
        this.uCurvesEngine = utilityCurvesEngine;
    }

    //Acción con submáquina
    public UtilityAction(State utilState, Factor factor, BehaviourEngine behaviourEngine, BehaviourEngine subMachine)
    {
        this.HasSubmachine = true;
        this.utilityState = utilState;
        this.factor = factor;
        this.uCurvesEngine = behaviourEngine as UtilityCurvesEngine;
        this.subMachine = subMachine;
    }

    
    //Acción de salida para árboles de comportamiento (sale a nodo hoja)
    public UtilityAction(Factor factor, ReturnValues valueReturned, UtilityCurvesEngine utilityCurvesEngine, BehaviourTreeEngine behaviourTreeEngine)
    {
        this.HasSubmachine = false;
        
        Action action = () =>
        {
            new Transition("Exit_Action_Transition", this.utilityState, new PushPerception(this.uCurvesEngine), this.uCurvesEngine.NodeToReturn,
                            valueReturned, behaviourTreeEngine, this.uCurvesEngine)
                            .FireTransition();
        };
        this.utilityState = new State("Exit_Action", action, utilityCurvesEngine);
        this.factor = factor;
        this.uCurvesEngine = utilityCurvesEngine;
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
