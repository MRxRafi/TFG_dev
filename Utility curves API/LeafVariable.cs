using System;
using System.Collections.Generic;

public class LeafVariable : Factor
{
    #region variables

    private float maxValue, minValue;
    private Func<float> getFactor;

    #endregion

    #region constructors

    /// <summary>
    /// Variable factor
    /// </summary>
    /// <param name="getVariable">The function that returns the variable</param>
    /// <param name="maxValue">The maximum value that the variable can reach</param>
    /// <param name="minValue">The minimum value that the variable can reach</param>
    public LeafVariable(Func<float> getVariable, float maxValue, float minValue)
    {
        this.getFactor = getVariable;
        this.maxValue = maxValue;
        this.minValue = minValue;
    }

    #endregion

    #region methods

    public override float getValue()
    {
        float factor = (getFactor() - minValue) / (maxValue - minValue);
        if (factor < 0) factor = 0; if (factor > 1) factor = 1;

        return factor;
    }

    #endregion
}
