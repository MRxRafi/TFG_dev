using System;
using System.Collections;
using System.Collections.Generic;


public class Testing
{

    private static UtilityCurvesEngine engine;
    private static Player p;
    private static Player p1;
    private static UtilityPerception perception;
    private static UtilityPerception perception2;

    static public void Main(String[] args)
    {
        engine = new UtilityCurvesEngine();

        float[] f = new float[2];
        f[0] = 1.0f;
        f[1] = 1.0f;
        p = new Player(2.0f, f);
        p1 = new Player(3.0f, f);

        perception = new PlayerPerception(p);
        perception2 = new PlayerPerception(p1);

        ExpFunc func = new ExpFunc(perception, 2);
        LinearFunc func2 = new LinearFunc(perception2, 1, 2);

        engine.CreateUtilityAction("bolo", () => Console.WriteLine("Se ha entrado en bolo"), func);
        engine.CreateUtilityAction("bolo2", () => Console.WriteLine("Se ha entrado en bolo2"), func2);

        // Update tick emulation (e.g like Unity)
        // ELAPSED crea hilos 
        System.Timers.Timer tmr = new System.Timers.Timer();
        tmr.Interval = 100;
        tmr.AutoReset = false;
        tmr.Elapsed += (s, e) => {
            engine.Update();
            tmr.Enabled = true;
        };
        tmr.Enabled = true;

        
        // To prevent the app closing
        // Si se usa EspacioPerception hay que pulsar dos veces espacio para que haga caso
        while (true)
        {
            ConsoleKeyInfo k = System.Console.ReadKey();
            if(k.Key == ConsoleKey.UpArrow)
            {
                p.life += 1.0f;
                Console.WriteLine("Vida de P: " + p.life);
            } else if (k.Key == ConsoleKey.DownArrow)
            {
                p.life -= 1.0f;
                Console.WriteLine("Vida de P: " + p.life);
            }
        };


    }
    /*
    private static void CreateMainMachine()
    {
        testMachine = new StateMachineEngine(false);

        testMachine.CreateEntryState("test", () => {
            Console.WriteLine("Se acaba de entrar al estado de entrada" +
                              " de la máquina principal.\n");
        });

        State st2 = testMachine.CreateState("test2", () => { Console.WriteLine("Se acaba de entrar al estado 2\n"); });
        State st3 = testMachine.CreateState("test3", () => { Console.WriteLine("Se acaba de entrar al estado 3\n"); });
        State stSub = testMachine.CreateSubStateMachine("subMachine", subMachine);

        p = testMachine.CreatePerception<TimerPerception>(3);

        testMachine.CreateTransition("e_st2", testMachine.GetEntryState(), p, st2);
        testMachine.CreateTransition("e_st3", st2, p, st3);
        testMachine.CreateTransition("e_sub", st3, p, stSub);
        subMachine.CreateExitTransition("e_sub_exit", stSub, pQ, testMachine.GetEntryState());
    }
    */
}