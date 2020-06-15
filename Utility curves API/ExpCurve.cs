using System;
using System.Collections.Generic;

public class ExpFunc: Curve
{
    #region variables

    private float k, c, b;

    #endregion

    #region constructors

    public ExpFunc(Factor f, float exp = 1, float despX = 0, float despY = 0) : base(f)
    {
        this.k = exp;
        this.c = despX;
        this.b = despY;
        this.factor = f;
    }

    #endregion

    public override float getValue()
    {
        return (float) (Math.Pow(factor.getValue() - c, k) + b);
    }

    public void setK(float _k)
    {
        this.k = _k;
    }

    public void setC(float _c)
    {
        this.c = _c;
    }

    public void setB(float _b)
    {
        this.b = _b;
    }
}