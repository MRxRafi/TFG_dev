using System;
using System.Collections.Generic;

public class LinearFunc: MathFunc
{
    #region variables

    private float m, c;

    #endregion

    #region constructors

    public LinearFunc(UtilityPerception perception, float pend = 1, float ind = 0)
    {
        this.m = pend;
        this.c = ind;
        this.p = perception;
    }

    #endregion

    public override float getImage()
    {
        return (m*p.getValue() + c);
    }
}