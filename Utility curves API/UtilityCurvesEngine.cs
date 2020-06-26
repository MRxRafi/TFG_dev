﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UtilityCurvesEngine : BehaviourEngine
{
    #region variables

    public UtilityAction ActiveAction;
    public List<UtilityAction> actions;

    private bool DEBUG = true;
    #endregion

    #region constructors

    /// <summary>
    /// Creates a UtilityCurvesEngine that CANNOT be a submachine
    /// </summary>
    /// <param name="isSubmachine"></param>
    public UtilityCurvesEngine()
    {
        base.transitions = new Dictionary<string, Transition>();
        base.states = new Dictionary<string, State>();
        this.actions = new List<UtilityAction>();
        base.IsSubMachine = false;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);

        Active = true;
    }

    /// <summary>
    /// Creates a UtilityCurvesEngine
    /// </summary>
    /// <param name="isSubmachine">Is this a submachine?</param>
    public UtilityCurvesEngine(bool isSubmachine)
    {
        base.transitions = new Dictionary<string, Transition>();
        base.states = new Dictionary<string, State>();
        this.actions = new List<UtilityAction>();
        base.IsSubMachine = isSubmachine;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);

        Active = (isSubmachine) ? false : true;
    }

    #endregion

    #region methods

    public void Update()
    {
        // TODO: ¿Problema de la inercia?
        if(actions.Count != 0)
        {
            if (ActiveAction != null)
            {
                if (!Active && !ActiveAction.HasSubmachine) return;

                // Para evitar problemas, si el estado actual es distinto a la acción, se realiza
                // un reseteo de la máquina. Esto ocurre al volver de un BT
                if (ActiveAction.utilityState != this.actualState)
                {
                    this.Reset();
                }

                int maxIndex = getMaxUtilityIndex();
                int activeActionIndex = this.actions.IndexOf(ActiveAction);

                if (maxIndex != activeActionIndex)
                {
                    ExitTransition(this.actions[maxIndex]);
                }

                ActiveAction.Update(); //En caso de necesitarse

            } else if(Active && ActiveAction == null) {
                int maxIndex = getMaxUtilityIndex();
                ExitTransition(this.actions[maxIndex]);
            }
        }
        
        
    }

    public override void Reset()
    {
        foreach (UtilityAction action in actions)
        {
            action.Reset();
        }
        this.ActiveAction = null;
    }

    private int getMaxUtilityIndex()
    {
        int actionsSize = this.actions.Count;
        List<float> utilities = new List<float>(actionsSize);

        for (int i = 0; i < actionsSize; i++)
        {
            utilities.Add(this.actions[i].getUtility());
        }

        // Si hay dos utilidades máximas iguales, se queda con la primera que encuentre
        return utilities.IndexOf(utilities.Max());
    }

    #endregion

    #region transitions

    // Exits from the actual utilityAction to go to another one
    public void ExitTransition(UtilityAction action)
    {
        string last = this.actualState.Name;
        if (ActiveAction != null)
        {
            if (ActiveAction.HasSubmachine)
            {
                this.ActiveAction.subMachine.ResetPerceptionsActiveState();
                new Transition("Max_Utility_Transition", this.ActiveAction.subMachine.actualState, new PushPerception(this),
                    this.GetEntryState(), this, this.ActiveAction.subMachine)
                    .FireTransition();
            } else
            {
                new Transition("Max_Utility_Transition", this.actualState, new PushPerception(this), action.utilityState, this)
                    .FireTransition();
            }
        } else
        {
            new Transition("Max_Utility_Transition", this.actualState, new PushPerception(this), action.utilityState, this)
                    .FireTransition();
        }

        this.ActiveAction = action;
        if (DEBUG)
        {
            //EXIT TRANSITION
            Console.WriteLine("[DEBUG] ExitTransition - New active action: " + action.utilityState.Name +
            ". Active State: " + this.actualState.Name + ". Last active action: " + last);
            Console.WriteLine("-------------------------");

            //UTILITIES
            Console.WriteLine("[DEBUG] Utilities: ");
            foreach(UtilityAction a in this.actions)
            {
                Console.WriteLine(a.utilityState.Name + " utility " + a.getUtility());
            }
            Console.WriteLine("[DEBUG] FINISHED UTILITIES DEBUG");
            Console.WriteLine("--------------------------------");
        }
    }

    #endregion

    #region create actions

    /// <summary>
    /// Creates a new <see cref="UtilityAction"/> in the utility curves engine
    /// </summary>
    /// <param name="name">The name of the utility action</param>
    /// <param name="action">The action the utility action will execute</param>
    /// <returns></returns>
    public UtilityAction CreateUtilityAction(string name, Action action, Factor factor)
    {
        if (!states.ContainsKey(name))
        {
            UtilityAction uAction = new UtilityAction(name, action, factor, this);
            actions.Add(uAction);
            states.Add(name, uAction.utilityState);

            return uAction;
        }
        else
        {
            throw new DuplicateWaitObjectException(name, "The utility action already exists in the utility engine");
        }
    }

    //Crea una UtilityAction que sirve para salir de la máquina al nodo hoja contenedor del UtilitySystem
    public UtilityAction CreateUtilityAction(Factor factor, ReturnValues valueReturned, BehaviourTreeEngine behaviourTreeEngine)
    {
        if (!states.ContainsKey("Exit_Action"))
        {
            UtilityAction uAction = new UtilityAction(factor, valueReturned, this, behaviourTreeEngine);
            actions.Add(uAction);
            states.Add("Exit_Action", uAction.utilityState);

            return uAction;
        }
        else
        {
            throw new DuplicateWaitObjectException("Exit_Action", "The utility action already exists in the utility engine");
        }
    }

    #endregion

    #region create sub-state machines

    /// <summary>
    /// Adds a type of <see cref="UtilityAction"/> with a sub-behaviour engine in it and its transition to the entry state
    /// </summary>
    /// <param name="actionName">The name of the action</param>
    /// <param name="factor">The factor that gives the utility value to the action</param>
    /// <param name="subBehaviourEngine">The sub-behaviour tree inside the </param>
    public UtilityAction CreateSubBehaviour(string actionName, Factor factor, BehaviourEngine subBehaviourEngine)
    {
        if (!states.ContainsKey(actionName))
        {
            State stateTo = subBehaviourEngine.GetEntryState();
            State state = new State(actionName, subBehaviourEngine.GetState("Entry_Machine"), stateTo, subBehaviourEngine, this);
            UtilityAction utilAction = new UtilityAction(state, factor, this, subBehaviourEngine);
            states.Add(utilAction.utilityState.Name, utilAction.utilityState);
            actions.Add(utilAction);

            return utilAction;
        } else
        {
            throw new DuplicateWaitObjectException(actionName, "The utility action already exists in the utility engine");
        }  
    }

    /// <summary>
    /// Adds a type of <see cref="UtilityAction"/> with a sub-behaviour engine in it and its transition to the entry state
    /// </summary>
    /// <param name="actionName">The name of the action</param>
    /// <param name="factor">The factor that gives the utility value to the action</param>
    /// <param name="subBehaviourEngine">The sub-behaviour tree inside the </param>
    /// <param name="stateTo">The name of the state where the sub-state machine will enter</param>
    public UtilityAction CreateSubBehaviour(string actionName, Factor factor, BehaviourEngine subBehaviourEngine, State stateTo)
    {
        if (!states.ContainsKey(actionName))
        {
            State state = new State(actionName, subBehaviourEngine.GetState("Entry_Machine"), stateTo, subBehaviourEngine, this);
            UtilityAction utilAction = new UtilityAction(state, factor, this, subBehaviourEngine);
            states.Add(utilAction.utilityState.Name, utilAction.utilityState);
            actions.Add(utilAction);

            return utilAction;
        }
        else
        {
            throw new DuplicateWaitObjectException(actionName, "The utility action already exists in the utility engine");
        }
    }

    #endregion create sub-state machines

}

