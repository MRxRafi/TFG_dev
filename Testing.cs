using System;
using System.Collections;
using System.Collections.Generic;


public class Testing
{
    private static StateMachineEngine testMachine;
    private static BehaviourTreeEngine BTMachine;
    private static UtilitySystemEngine utilEngine;
    private static Player p;
    private static Player p1;
    private static Factor lifeVariable1;
    private static Factor lifeVariable2;
    private static Factor maxFusionFactor;
    private static Factor expFactor1, expFactor2;
    private static Factor linearFactor;
    private static Factor weightFactor;
    private static UtilityAction u1, u2, uSub1, uSub2;

    static public void Main(String[] args)
    {
        testMachine = new StateMachineEngine(true);
        BTMachine = new BehaviourTreeEngine(true);
        utilEngine = new UtilitySystemEngine(false);

        float[] f = { 1.0f, 1.0f };

        p = new Player(2.0f, f);
        p1 = new Player(9.0f, f);

        CreateSubmachine();
        CreateSubmachine2();
        CreateMainMachine();


        // Update tick emulation (e.g like Unity)
        // ELAPSED crea hilos 
        System.Timers.Timer tmr = new System.Timers.Timer();
        tmr.Interval = 100;
        tmr.AutoReset = false;
        tmr.Elapsed += (s, e) => {
            testMachine.Update();
            BTMachine.Update();
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
                //Console.WriteLine("Utilidad de linear: " + u1.getUtility());
                //Console.WriteLine("Utilidad de exp: " + u2.getUtility());
            } else if (k.Key == ConsoleKey.DownArrow)
            {
                p.life -= 1.0f;
                Console.WriteLine("Vida de P: " + p.life);
                //Console.WriteLine("Utilidad de linear: " + u1.getUtility());
                //Console.WriteLine("Utilidad de exp: " + u2.getUtility());
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

    private static void CreateSubmachine2()
    {
        SequenceNode root = BTMachine.CreateSequenceNode("root", false);
        BTMachine.SetRootNode(root);

        Action a1 = new Action(() => {
            p1.modifyLife();
            Console.WriteLine("[BT_MACHINE] Vida p1 modificada: " + p1.life);
            Console.WriteLine("--------------------------------");
        });
        LeafNode lf1 = BTMachine.CreateLeafNode("leaf1", a1, () => ReturnValues.Succeed);

        LeafNode lf2 = BTMachine.CreateLeafNode("leaf2", () => Console.WriteLine("[BT_MACHINE]--------------------------------"), () => ReturnValues.Succeed);
        TimerDecoratorNode tdn = BTMachine.CreateTimerNode("timer", lf2, 2);

        root.AddChild(lf1);
        root.AddChild(tdn);

        BTMachine.CreateExitTransition("Exit_Transition", root.StateNode,
            new BehaviourTreeStatusPerception(BTMachine, ReturnValues.Succeed, utilEngine), utilEngine);
    }

    private static void CreateMainMachine()
    {
        //FACTORS
        // Leafs
        lifeVariable1 = new LeafVariable(() => { return p.life; }, 10.0f, 0.0f);
        lifeVariable2 = new LeafVariable(() => { return p1.life; }, 10.0f, 0.0f);

        List<Factor> lifeFactors = new List<Factor>();
        lifeFactors.Add(lifeVariable1);
        lifeFactors.Add(lifeVariable2);

        // Min Fusion
        maxFusionFactor = new MaxFusion(lifeFactors);

        // Linear by parts
        List<Point2D> points = new List<Point2D>();
        points.Add(new Point2D(0, 0));
        points.Add(new Point2D(0.2f, 0.4f));
        points.Add(new Point2D(0.6f, 0.8f));
        points.Add(new Point2D(1, 1));
        linearFactor = new LinearPartsCurve(lifeVariable1, points);

        // Exponential Factors
        expFactor1 = new ExpCurve(linearFactor, 0.6f);
        expFactor2 = new ExpCurve(lifeVariable2, 0.7f);

        // Weight Factor
        List<Factor> wFactors = new List<Factor>();
        wFactors.Add(expFactor1);
        wFactors.Add(maxFusionFactor);
        weightFactor = new WeightedSumFusion(wFactors);

        //ACTIONS

        //Basic actions
        Action a1 = new Action(() => {
            p1.modifyLife();
            Console.WriteLine("[BASICO1] Vida p1 modificada: " + p1.life);
            Console.WriteLine("--------------------------------");
        });
        Action a2 = new Action(() => {
            p.modifyLife();
            Console.WriteLine("[BASICO2] Vida p modificada: " + p.life);
            Console.WriteLine("--------------------------------");
        });

        //Utility Actions
        u1 = utilEngine.CreateUtilityAction("basico1", a1, maxFusionFactor);
        u2 = utilEngine.CreateUtilityAction("basico2", a2, linearFactor);
        uSub1 = utilEngine.CreateSubBehaviour("subMachine1", weightFactor, testMachine);
        uSub2 = utilEngine.CreateSubBehaviour("subMachine2", expFactor2, BTMachine);
    }
    
}