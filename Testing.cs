using System;
using System.Collections;
using System.Collections.Generic;


public class Testing{

    static public void Main(String[] args)
    {
        Console.WriteLine("Hola amiguis");

        // State Machines
        StateMachineEngine subMachine = new StateMachineEngine(true);
        StateMachineEngine testMachine = new StateMachineEngine(false);

        // States
        testMachine.CreateEntryState("test", () => { Console.WriteLine("Se acaba de entrar al estado de entrada\n"); });
        subMachine.CreateEntryState("subTest1", () => Console.WriteLine("[SUBMAQUINA] Estado 1 activado."));

        State st2 = testMachine.CreateState("test2", () => { Console.WriteLine("Se acaba de entrar al estado 2\n"); });
        State st3 = testMachine.CreateState("test3", () => { Console.WriteLine("Se acaba de entrar al estado 3\n"); });
        State stSub = testMachine.CreateSubStateMachine("subMachine", subMachine);
        State sub_2 = subMachine.CreateState("subTest2", () => Console.WriteLine("[SUBMAQUINA] Estado 2 activado."));


        // Perceptions
        TimerPerception p = testMachine.CreatePerception<TimerPerception>(2);
        TimerPerception pSub = subMachine.CreatePerception<TimerPerception>(1);
        
        // Transitions
        testMachine.CreateTransition("e_st2", testMachine.GetEntryState(), p, st2);
        testMachine.CreateTransition("e_st3", st2, p, st3);
        testMachine.CreateTransition("e_sub", st3, p, stSub);
        
        subMachine.CreateTransition("e_sTest1", subMachine.GetEntryState(), pSub, sub_2);
        subMachine.CreateTransition("e_sTest2", sub_2, pSub, subMachine.GetEntryState());
        subMachine.CreateExitTransition("e_sTest3", stSub, p, testMachine.GetEntryState());


        // Update tick emulation (e.g like Unity)
        //Timer timerUpdate = new Timer((e) => { testMachine.Update(); subMachine.Update(); }, null, 70, 70);
        // ELAPSED crea hilos 
        System.Timers.Timer tmr = new System.Timers.Timer();
        tmr.Interval = 100;
        tmr.AutoReset = false;
        tmr.Elapsed += (s, e) => { testMachine.Update(); subMachine.Update(); tmr.Enabled = true; };
        tmr.Enabled = true;
        Console.ReadKey();

    }
}