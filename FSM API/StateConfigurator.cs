using System;
using System.Collections;
using System.Collections.Generic;

public class StateConfigurator {
    #region variables

    public enum STATE_TYPE {EMPTY, NOT_EMPTY};
    public STATE_TYPE stateType { get; set; } // Maybe this isn't necessary
    public Action entry { get; }

    // A Queue is needed as there could be multiple Actions to execute when we exit 
    // (the normal Exit + the InternalTransition)
    public Queue<Action> exit;

    #endregion variables

    // ¿No constructors?
    public StateConfigurator(STATE_TYPE st){
        this.stateType = st;
        exit = new Queue<Action>();
    }

    #region methods
    public StateConfigurator OnEntry(Action method){
        entry = method;
        this.stateType = this.STATE_TYPE.NOT_EMPTY;
        return this;
    }

    public StateConfigurator OnExit(Action method){
        exit.Enqueue(method);
        return this;
    }

    public StateConfigurator InternalTransition(Perception perception, Action method){
        exit.Enqueue(method);
        return this;
    }
    #endregion methods
}