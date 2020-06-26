using System;
using System.Collections.Generic;

public class Player
{
    public float life;
    public float[] position;
    private Random rand;

    public Player(float l, float[] pos)
    {
        life = l;
        position = pos;
        rand = new Random();
    }

    public void modifyLife()
    {
        int randomInt = rand.Next(50);
        float floated = (float)randomInt / 10;
        Console.Write("Value: " + floated);

        if (this.life > 5.0f) this.life -= floated;
        else if (this.life < 5.0f) this.life += floated;
        else if (this.life == 5.0f) this.life += floated - 0.5f;
        Console.Write(" Life: " + life);
    }
}

