using System;
using System.Collections.Generic;
using System.Linq;

public class MaxFusion : Fusion
{
    #region constructors
    public MaxFusion(List<Factor> factors) : base(factors) { }
    #endregion

    #region methods

    public override float getValue()
    {
        return factors.Max(f => f.getValue());
    }

    #endregion
}
