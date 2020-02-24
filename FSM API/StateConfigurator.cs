using System;
using System.Collections;
using System.Collections.Generic;

public class StateConfigurator{
    #region variables

    private enum STATE_TYPE {EMPTY, WITHOUT_PERCEPTION, WITH_PERCEPTION, SUBMACHINE};
    private STATE_TYPE stateType; // Maybe this isn't necessary
    private Action exit, entry;

    #endregion variables

    #region methods
    public StateConfigurator OnEntry(Action method){
        entry = method;
        return this;
    }

    public StateConfigurator OnExit(Action method){
        exit = method;
        return this;
    }

    public StateConfigurator PermitReentry(Perception perception){
        /* TODO Transition to the same State */
        return this;
    }

    public StateConfigurator Permit(Perception perception, State state){
        /* TODO Transition to State of the same machine */
        return this;
    }

    public StateConfigurator InternalTransition(Perception perception, Action method){
        /* TODO Transition to State of other machine */
    }
    #endregion methods
}