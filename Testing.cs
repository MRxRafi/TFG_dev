using System;
using System.Collections;
using System.Collections.Generic;


public class Testing
{
    private static BehaviourTreeEngine testMachine;
    private static UtilityCurvesEngine utilEngine;
    private static Player p;
    private static Player p1;
    private static Factor lifeVariable1;
    private static Factor lifeVariable2;
    private static Factor linearFactor;
    private static Factor expFactor;
    private static Factor lFactor;
    private static UtilityAction u1, u2, uExit;
    private static bool lPressed = false;

    static public void Main(String[] args)
    {
        testMachine = new BehaviourTreeEngine(false);
        utilEngine = new UtilityCurvesEngine(true);

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
                Console.WriteLine("Utilidad L: " + uExit.getUtility());
            }
        };


    }
    
    private static void CreateMainMachine()
    {
        SequenceNode sn = testMachine.CreateSequenceNode("root", false);
        LoopDecoratorNode ldp = testMachine.CreateLoopNode("loop", sn);   
        testMachine.SetRootNode(ldp);

        LeafNode n1 = testMachine.CreateLeafNode("test1", () => { Console.WriteLine("Se acaba de entrar al nodo 1\n"); }, () => ReturnValues.Succeed);
        LeafNode n2 = testMachine.CreateLeafNode("test2", () => { Console.WriteLine("Se acaba de entrar al nodo 2\n"); }, () => ReturnValues.Succeed);
        LeafNode nSub = testMachine.CreateSubBehaviour("sub", utilEngine);

        TimerDecoratorNode tdn = testMachine.CreateTimerNode("timer", n2, 2);

        sn.AddChild(n1);
        sn.AddChild(tdn);
        sn.AddChild(nSub);  
    }

    private static void CreateSubmachine()
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
        uExit = utilEngine.CreateUtilityAction(lFactor, ReturnValues.Succeed, testMachine);
    }
    
}