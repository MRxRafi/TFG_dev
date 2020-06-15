using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UtilityCurvesEngine : BehaviourEngine
{
    #region variables

    public UtilityAction ActiveAction;
    public List<UtilityAction> actions;

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
        if (!Active) return;

        int actionsSize = this.actions.Count;
        List<float> utilities = new List<float>(actionsSize);

        for (int i = 0; i < actionsSize; i++)
        {
            utilities.Add(this.actions[i].getUtility());
        }

        // Si hay dos utilidades máximas iguales, se queda con la primera que encuentre
        int maxIndex = utilities.IndexOf(utilities.Max());

        if (ActiveAction != null)
        {
            int activeActionIndex = this.actions.IndexOf(ActiveAction);

            if (maxIndex != activeActionIndex)
            {
                //Transicion a otra acción
                ExitTransition(this.actions[maxIndex]);
            }

            ActiveAction.Update(); //En caso de necesitarse

        } else if(actions.Count != 0)
        {
            ExitTransition(this.actions[maxIndex]);
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

    #endregion

    #region transitions

    public void ExitTransition(UtilityAction action)
    {
        string last = this.actualState.Name;
        new Transition("Max_Utility_Transition", this.actualState, new PushPerception(this), action.utilityState, this)
                    .FireTransition();

        this.ActiveAction = action;
        Console.WriteLine("[DEBUG] ExitTransition - New active action: " + action.utilityState.Name + 
            ". Active State: " + this.actualState.Name + ". Last active action: " + last);
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

    #endregion
}

