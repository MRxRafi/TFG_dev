using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;


public class Testing{

    static public void Main(String[] args)
    {
        Console.WriteLine("Hola amiguis");

        StateMachineEngine testMachine = new StateMachineEngine(false);
        testMachine.CreateEntryState("test", () => { Console.WriteLine("Se acaba de entrar al estado de entrada\n"); });
        State st2 = testMachine.CreateState("test2", () => { Console.WriteLine("Se acaba de entrar al estado 2\n"); });
        State st3 = testMachine.CreateState("test3", () => { Console.WriteLine("Se acaba de entrar al estado 3\n"); });
        TimerPerception p = testMachine.CreatePerception<TimerPerception>(2);
        //TimerPerception p2 = testMachine.CreatePerception<TimerPerception>(1);
        //TimerPerception p3 = testMachine.CreatePerception<TimerPerception>(3);



        testMachine.CreateTransition("e_st2", testMachine.GetEntryState(), p, st2);
        testMachine.CreateTransition("e_st3", st2, p, st3);
        testMachine.CreateTransition("e_stEntry", st3, p, testMachine.GetEntryState());

        Timer timerUpdate = new Timer(70);
        timerUpdate.Elapsed += (s, e) => { testMachine.Update(); };
        timerUpdate.Enabled = true;
        timerUpdate.Start();
        while (true) ;

    }
}