using System;
using System.Collections;
using System.Collections.Generic;


public class Testing{
    private static StateMachineEngine testMachine;
    private static StateMachineEngine subMachine;
    private static BehaviourTreeEngine subBTMachine;
    private static TimerPerception p;
    private static TimerPerception pSub;
    private static KeyPerception pQ;
    private static KeyPerception pW;

    static public void Main(String[] args)
    {
        CreateSubBehaviourMachine();
        CreateSubMachine();
        CreateMainMachine();

        /* ERRORES después de la primera iteracion al entrar en la submaquina pasa del estado de la 
           submaquina 1 al 2 sin que haga caso al timerperception, al entrar al estado de entrada de 
           la supermaquina entra 2 veces*/

        /* TRANSICION ENTRE ESTADOS
         *  - Para salir de la submáquina: Pulsar letra Q
         *  - Para salir del estado 1 de la submáquina: letra W
         * */

        // Update tick emulation (e.g like Unity)
        //Timer timerUpdate = new Timer((e) => { testMachine.Update(); subMachine.Update(); }, null, 70, 70);
        // ELAPSED crea hilos 
        System.Timers.Timer tmr = new System.Timers.Timer();
        tmr.Interval = 100;
        tmr.AutoReset = false;
        tmr.Elapsed += (s, e) => { testMachine.Update();
            subMachine.Update();
            subBTMachine.Update(); tmr.Enabled = true; };
        tmr.Enabled = true;

        // To prevent the app closing
        // Si se usa EspacioPerception hay que pulsar dos veces espacio para que haga caso
        while (true)
        {
            System.Console.ReadKey();
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
        State stSub = testMachine.CreateSubStateMachine("subMachine", subMachine);

        p = testMachine.CreatePerception<TimerPerception>(3);

        testMachine.CreateTransition("e_st2", testMachine.GetEntryState(), p, st2);
        testMachine.CreateTransition("e_st3", st2, p, st3);
        testMachine.CreateTransition("e_sub", st3, p, stSub);
        subMachine.CreateExitTransition("e_sub_exit", stSub, pQ, testMachine.GetEntryState());
    }

    private static void CreateSubMachine()
    {
        subMachine = new StateMachineEngine(true);

        subMachine.CreateEntryState("subTest1", () => Console.WriteLine("[SUBMAQUINA] Estado 1 activado."));

        State sub_2 = subMachine.CreateState("subTest2", () => Console.WriteLine("[SUBMAQUINA] Estado 2 activado."));
        State subBTState = subMachine.CreateSubStateMachine("subMachine", subBTMachine);

        pSub = subMachine.CreatePerception<TimerPerception>(2.5f);
        pQ = subMachine.CreatePerception<KeyPerception>(new KeyPerception(ConsoleKey.Q, subMachine));
        pW = subMachine.CreatePerception<KeyPerception>(new KeyPerception(ConsoleKey.W, subMachine));

        subMachine.CreateTransition("e_sTest1", subMachine.GetEntryState(), pW, sub_2);
        subMachine.CreateTransition("e_sTest2", sub_2, pSub, subBTState);

        BehaviourTreeStatusPerception succeed = subMachine.CreatePerception<BehaviourTreeStatusPerception>(subBTMachine, ReturnValues.Succeed);
        subBTMachine.CreateExitTransition("e_sTest3", subBTState, succeed, subMachine.GetEntryState());
    }

    private static void CreateSubBehaviourMachine()
    {
        subBTMachine = new BehaviourTreeEngine(true);

        SequenceNode sn1 = subBTMachine.CreateSequenceNode("s_1", false);

        LeafNode tn = subBTMachine.CreateLeafNode("ts_2_action",
            () => Console.WriteLine("[SUB_BT] Secuencia activada, nodo 1."),
            () => { return ReturnValues.Succeed; });
        LeafNode tn2 = subBTMachine.CreateLeafNode("s_3_action",
            () => Console.WriteLine("[SUB_BT] Secuencia activada, nodo 2."),
            () => { return ReturnValues.Succeed; });
        TimerDecoratorNode tdn = subBTMachine.CreateTimerNode("ts_2", tn, 1);
        sn1.AddChild(tdn);
        sn1.AddChild(tn2);

        subBTMachine.SetRootNode(sn1);
    }
}