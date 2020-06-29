using System;
using System.Collections.Generic;

public class LinearCurve : Curve
{
    #region variables

    private float m, c;

    #endregion

    #region constructors

    public LinearCurve(Factor f, float pend = 1, float ind = 0) : base(f)
    {
        this.m = pend;
        this.c = ind;
    }

    #endregion

    public override float getValue()
    {
        return m*factor.getValue() + c;
    }

    public void setM(float _m)
    {
        this.m = _m;
    }

    public void setC(float _c)
    {
        this.c = _c;
    }
}