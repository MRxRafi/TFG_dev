using System;
using System.Collections;
using System.Collections.Generic;


public class Testing{
    //private static StateMachineEngine testMachine;
    //private static StateMachineEngine subMachine;
    private static BehaviourTreeEngine subBTMachine;
    /*
    private static TimerPerception p;
    private static TimerPerception pSub;
    private static KeyPerception pQ;
    private static KeyPerception pW;
    */

    private static bool lockedDoor = true;
    private static bool key = true;

    static public void Main(String[] args)
    {
        CreateSubBehaviourMachine();
        //CreateSubMachine();
        //CreateMainMachine();

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
        tmr.Elapsed += (s, e) => {
            //testMachine.Update();
            //subMachine.Update();
            subBTMachine.Update();
            tmr.Enabled = true;
        };
        tmr.Enabled = true;

        // To prevent the app closing
        // Si se usa EspacioPerception hay que pulsar dos veces espacio para que haga caso
        while (true)
        {
            System.Console.ReadKey();
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
    */

    private static void CreateSubBehaviourMachine()
    {
        subBTMachine = new BehaviourTreeEngine(false);

        SequenceNode root = subBTMachine.CreateSequenceNode("root", false);
        SelectorNode select = subBTMachine.CreateSelectorNode("select");
        SequenceNode unlock = subBTMachine.CreateSequenceNode("unlock", false);

        LeafNode walkToDoor1 = subBTMachine.CreateLeafNode("walk_door1",
            () => Console.WriteLine("[WALK_TO] Andando a la puerta."),
            () => { return ReturnValues.Succeed; });
        LeafNode walkToDoor2 = subBTMachine.CreateLeafNode("walk_door2",
            () => Console.WriteLine("[WALK_TO] Andando a la puerta."),
            () => { return ReturnValues.Failed; });
        LeafNode enterHouse = subBTMachine.CreateLeafNode("enter_house",
            () => Console.WriteLine("[ROOT] Entrando a la casa."),
            () => { return ReturnValues.Succeed; });
        LeafNode openDoor1 = subBTMachine.CreateLeafNode("oDoor1",
            () => Console.WriteLine("[UNLOCKED_DOOR] Abriendo puerta"),
            () => {
                if (!lockedDoor) return ReturnValues.Succeed;
                Console.WriteLine("Ha retornado failed.");
                return ReturnValues.Failed;
            });
        LeafNode openDoor2 = subBTMachine.CreateLeafNode("oDoor2",
            () => Console.WriteLine("[LOCKED_DOOR] Abriendo puerta"),
            () => {
                if (!lockedDoor) return ReturnValues.Succeed;
                return ReturnValues.Failed;
            });

        LeafNode unlockDoor = subBTMachine.CreateLeafNode("uDoor",
            () => {

                if (key)
                {
                    Console.WriteLine("[COLLECT_KEY] Recogiendo llave.");
                    lockedDoor = false;
                } else
                {
                    Console.WriteLine("[COLLECT_KEY] No hay llave que recoger.");
                }
            },
            () => { 
                if(key) return ReturnValues.Succeed;
                return ReturnValues.Failed;
            });

        LeafNode explodeDoor = subBTMachine.CreateLeafNode("eDoor",
            () => Console.WriteLine("[CLOSED_DOOR] No hay llave. No queda otra que reventar la puerta."),
            () => { return ReturnValues.Succeed; });

        TimerDecoratorNode test = subBTMachine.CreateTimerNode("timer", walkToDoor2, 1);
        LoopUntilFailDecoratorNode test2 = subBTMachine.CreateLoopUntilFailNode("loop", test);

        unlock.AddChild(unlockDoor);
        unlock.AddChild(walkToDoor1);
        unlock.AddChild(openDoor1);

        select.AddChild(openDoor2);
        select.AddChild(unlock);
        select.AddChild(explodeDoor);

        root.AddChild(test2);
        root.AddChild(select);
        root.AddChild(enterHouse);

        subBTMachine.SetRootNode(root);
    }
}