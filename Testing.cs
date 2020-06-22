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
    private static Factor lFactor;
    private static UtilityAction u1, u2, uSub;
    private static bool lPressed = false;

    static public void Main(String[] args)
    {
        testMachine = new StateMachineEngine(true);
        utilEngine = new UtilityCurvesEngine(false);

        float[] f = { 1.0f, 1.0f };

        p = new Player(2.0f, f);
        p1 = new Player(9.0f, f);

        CreateSubmachine();
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
            } else if(k.Key == ConsoleKey.L)
            {
                if (lPressed) { lPressed = false; } else { lPressed = true; };
                Console.WriteLine("Estado L: " + lPressed);
                Console.WriteLine("Utilidad L: " + uSub.getUtility());
            }
        };


    }
    
    private static void CreateSubmachine()
    {
        State st1 = testMachine.CreateEntryState("st1", () => Console.WriteLine("Se acaba de entrar al estado 1\n"));
        State st2 = testMachine.CreateState("st2", () => Console.WriteLine("Se acaba de entrar al estado 2\n"));
        State st3 = testMachine.CreateState("st3", () => Console.WriteLine("Se acaba de entrar al estado 3\n"));

        TimerPerception tp1 = testMachine.CreatePerception<TimerPerception>(1.0f);
        TimerPerception tp2 = testMachine.CreatePerception<TimerPerception>(2.0f);
        TimerPerception tp3 = testMachine.CreatePerception<TimerPerception>(2.0f);

        testMachine.CreateTransition("toSt2", st1, tp1, st2);
        testMachine.CreateTransition("toSt3", st2, tp2, st3);
        testMachine.CreateExitTransition("Exit_Transition", st3, tp3, utilEngine);
    }

    private static void CreateMainMachine()
    {
        lifeVariable1 = new LeafVariable(() => { return p.life; }, 10.0f, 0.0f);
        lifeVariable2 = new LeafVariable(() => { return p1.life; }, 10.0f, 0.0f);
        linearFactor = new LinearFunc(lifeVariable1);
        expFactor = new ExpFunc(lifeVariable2, 2);
        lFactor = new LeafVariable(() => {
            if (lPressed) return 1.0f;
            return 0.0f;
        }, 1.0f, 0.0f);

        u1 = utilEngine.CreateUtilityAction("bolo", () => Console.WriteLine("Se ha entrado en bolo"), linearFactor);
        u2 = utilEngine.CreateUtilityAction("bolo2", () => Console.WriteLine("Se ha entrado en bolo2"), expFactor);
        uSub = utilEngine.CreateSubBehaviour("subMachine", lFactor, testMachine);
    }
    
}