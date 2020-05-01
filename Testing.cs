using System;
using System.Collections;
using System.Collections.Generic;


public class Testing{
    //private static StateMachineEngine testMachine;
    private static StateMachineEngine subMachine;
    private static BehaviourTreeEngine BTMachine;
    
    private static Perception massPutted;
    private static Perception tomatoPutted;
    private static Perception nextTopping;
    private static Perception allToppings;
    private static LeafNode subFSM;

    private static int actualIngredient = 0;

    static public void Main(String[] args)
    {
        BTMachine = new BehaviourTreeEngine(BehaviourEngine.IsNotASubmachine);
        subMachine = new StateMachineEngine(BehaviourEngine.IsASubmachine);

        CreateSubMachine();
        CreateSubBehaviourMachine();
        //CreateMainMachine();


        // Update tick emulation (e.g like Unity)
        //Timer timerUpdate = new Timer((e) => { testMachine.Update(); subMachine.Update(); }, null, 70, 70);
        // ELAPSED crea hilos 
        System.Timers.Timer tmr = new System.Timers.Timer();
        tmr.Interval = 100;
        tmr.AutoReset = false;
        tmr.Elapsed += (s, e) => {
            //testMachine.Update();
            subMachine.Update();
            BTMachine.Update();
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
    */
    private static void CreateSubMachine()
    {
        //Perceptions
        massPutted = subMachine.CreatePerception<TimerPerception>(1);
        tomatoPutted = subMachine.CreatePerception<TimerPerception>(1);
        nextTopping = subMachine.CreatePerception<TimerPerception>(1);
        allToppings = subMachine.CreatePerception<PushPerception>();

        //States
        State putMass = subMachine.CreateEntryState("put_mass",
                        () => Console.WriteLine("[SUBFSM] Putting mass."));
        State putTomato = subMachine.CreateState("put_tomato",
                        () => Console.WriteLine("[SUBFSM] Putting tomato."));
        State putTopping = subMachine.CreateState("put_topping", PutTopping);

        //Transitions
        subMachine.CreateTransition("mass_putted", putMass, massPutted, putTomato);
        subMachine.CreateTransition("tomato_putted", putTomato, tomatoPutted, putTopping);
        subMachine.CreateTransition("next_topping", putTopping, nextTopping, putTopping);

        //Super nodo del árbol de comportamientos y la transición de salida
        subFSM = BTMachine.CreateSubBehaviour("subFSM", subMachine, putMass);
        subMachine.CreateExitTransition("back_bt", putTopping, allToppings, ReturnValues.Succeed);
    }

    private static void CreateSubBehaviourMachine()
    {
        SequenceNode sequence = BTMachine.CreateSequenceNode("makePizza", false);

        LeafNode lookRecipe = BTMachine.CreateLeafNode("look_recipe",
            () => Console.WriteLine("[SEQUENCE] Mirando receta."),
            () => { return ReturnValues.Succeed; });

        LeafNode bake = BTMachine.CreateLeafNode("bake",
            () => Console.WriteLine("[SEQUENCE] Haciendo pizza. \n ----------------"),
            () => { return ReturnValues.Succeed; });


        sequence.AddChild(lookRecipe);
        sequence.AddChild(subFSM);
        sequence.AddChild(bake);

        LoopDecoratorNode loop = BTMachine.CreateLoopNode("root", sequence);

        BTMachine.SetRootNode(loop);
    }

    private static void PutTopping()
    {
        switch (actualIngredient)
        {
            case 0:
                Console.WriteLine("[SUBFSM] Poniendo ingrediente 0.");
                break;
            case 1:
                Console.WriteLine("[SUBFSM] Poniendo ingrediente 1.");
                break;
            case 2:
                Console.WriteLine("[SUBFSM] Poniendo ingrediente 2.");
                break;
        }
        actualIngredient++;

        if(actualIngredient == 3)
        {
            allToppings.Fire();
            actualIngredient = 0;
        }
    }
}