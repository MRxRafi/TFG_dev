using System;
using System.Collections.Generic;

public class ExpFunc: MathFunc
{
    #region variables

    private float k, c, b;

    #endregion

    #region constructors

    public ExpFunc(UtilityPerception perception, float exp = 1, float despX = 0, float despY = 0)
    {
        this.k = exp;
        this.c = despX;
        this.b = despY;
        this.p = perception;
    }

    #endregion

    public override float getImage()
    {
        return (float) (Math.Pow(p.getValue() - c, k) + b);
    }
}