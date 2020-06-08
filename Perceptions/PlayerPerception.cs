using System;
using System.Collections.Generic;

public class PlayerPerception : UtilityPerception
{
    Player p;

    public PlayerPerception(Player _p)
    {
        p = _p;
    }

    public override float getValue()
    {
        return p.life;
    }
}
