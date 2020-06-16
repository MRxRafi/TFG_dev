using System;
using System.Collections;
using System.Collections.Generic;


public class Testing
{
    private static StateMachineEngine testMachine;
    private static UtilityCurvesEngine utilEngine;
    private static Player p;
    private static Player p1;
    private static Factor lifeVariable1;
    private static Factor lifeVariable2;
    private static Factor linearFactor;
    private static Factor expFactor;
    private static Perception tPerception;
    private static Perception pPerception;
    private static Perception isInPerception;

    static public void Main(String[] args)
    {
        utilEngine = new UtilityCurvesEngine(true);

        float[] f = new float[2];
        f[0] = 1.0f;
        f[1] = 1.0f;
        p = new Player(2.0f, f);
        p1 = new Player(9.0f, f);

        lifeVariable1 = new LeafVariable(() => { return p.life; }, 10.0f, 0.0f);
        lifeVariable2 = new LeafVariable(() => { return p1.life; }, 10.0f, 0.0f);
        linearFactor = new LinearFunc(lifeVariable1);
        expFactor = new ExpFunc(lifeVariable2, 2);

        UtilityAction u1 = utilEngine.CreateUtilityAction("bolo", () => Console.WriteLine("Se ha entrado en bolo"), linearFactor);
        UtilityAction u2 = utilEngine.CreateUtilityAction("bolo2", () => Console.WriteLine("Se ha entrado en bolo2"), expFactor);

        CreateMainMachine();

        // Update tick emulation (e.g like Unity)
        // ELAPSED crea hilos 
        System.Timers.Timer tmr = new System.Timers.Timer();
        tmr.Interval = 100;
        tmr.AutoReset = false;
        tmr.Elapsed += (s, e) => {
            testMachine.Update();
            utilEngine.Update();
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
                Console.WriteLine("Utilidad de linear: " + u1.getUtility());
                Console.WriteLine("Utilidad de exp: " + u2.getUtility());
            } else if (k.Key == ConsoleKey.DownArrow)
            {
                p.life -= 1.0f;
                Console.WriteLine("Vida de P: " + p.life);
                Console.WriteLine("Utilidad de linear: " + u1.getUtility());
                Console.WriteLine("Utilidad de exp: " + u2.getUtility());
            }
        };


    }
    
    private static void CreateMainMachine()
    {
        testMachine = new StateMachineEngine(false);

        testMachine.CreateEntryState("test", () => {
            Console.WriteLine("Se acaba de entrar al estado de entrada" +
                              " de la máquina principal.\n");
        });

        State st2 = testMachine.CreateState("test2", () => { Console.WriteLine("Se acaba de entrar al estado 2\n"); });
        State st3 = testMachine.CreateState("test3", () => { Console.WriteLine("Se acaba de entrar al estado 3\n"); });
        State stSub = testMachine.CreateSubStateMachine("subMachine", utilEngine);

        tPerception = testMachine.CreatePerception<TimerPerception>(3);

        testMachine.CreateTransition("e_st2", testMachine.GetEntryState(), tPerception, st2);
        testMachine.CreateTransition("e_st3", st2, tPerception, st3);
        testMachine.CreateTransition("e_sub", st3, tPerception, stSub);

        pPerception = new KeyPerception(ConsoleKey.P, utilEngine);
        utilEngine.CreateExitTransition("e_sub_exit", stSub, pPerception, testMachine.GetEntryState());

        isInPerception = new IsInStatePerception(utilEngine, "bolo2", testMachine);
        utilEngine.CreateExitTransition("e_sub_exit2", stSub, isInPerception, st2);
        
    }
    
}