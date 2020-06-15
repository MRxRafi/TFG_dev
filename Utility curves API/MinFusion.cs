using System;
using System.Collections.Generic;
using System.Linq;

public class MinFusion : Fusion
{
    #region constructors
    public MinFusion(List<Factor> factors) : base(factors) { }
    #endregion

    #region methods
    public override float getValue()
    {
        return factors.Min(f => f.getValue());
    }
    #endregion
}
