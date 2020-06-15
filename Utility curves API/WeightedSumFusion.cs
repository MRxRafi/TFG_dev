using System;
using System.Collections.Generic;
using System.Linq;

public class WeightedSumFusion : Fusion
{
    #region constructors
    public WeightedSumFusion(List<Factor> factors) : base(factors) { }
    #endregion

    #region methods

    public override float getValue()
    {
        return factors.Sum(f => f.getValue()) / factors.Count;
    }

    #endregion
}
