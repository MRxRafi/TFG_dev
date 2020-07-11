using System;
using System.Collections;
using System.Collections.Generic;


public class Testing
{
    private static UtilitySystemEngine subUtilMachine;
    private static UtilitySystemEngine utilEngine;
    private static Player p, p1, p2, p3;
    private static Factor lifeVariable1, lifeVariable2;
    private static Factor positionVariable1, positionVariable2;
    
    static public void Main(String[] args)
    {
        subUtilMachine = new UtilitySystemEngine(true, 1.15f);
        utilEngine = new UtilitySystemEngine(false);

        float[] f = { 1.0f, 1.0f };

        p = new Player(2.0f, f);
        p1 = new Player(5.0f, f);
        p2 = new Player(9.0f, new float[] { 3.0f, 0.0f });
        p3 = new Player(9.0f, new float[] { 4.0f, 0.0f });

        CreateSubmachine();
        CreateMainMachine();


        // Update tick emulation (e.g like Unity)
        // ELAPSED crea hilos 
        System.Timers.Timer tmr = new System.Timers.Timer();
        tmr.Interval = 100;
        tmr.AutoReset = false;
        tmr.Elapsed += (s, e) => {
            subUtilMachine.Update();
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
            } else if (k.Key == ConsoleKey.DownArrow)
            {
                p.life -= 1.0f;
                Console.WriteLine("Vida de P: " + p.life);
            } else if(k.Key == ConsoleKey.LeftArrow)
            {
                p1.life -= 1.0f;
                Console.WriteLine("Vida de P1: " + p1.life);
            } else if(k.Key == ConsoleKey.RightArrow)
            {
                p1.life += 1.0f;
                Console.WriteLine("Vida de P1: " + p1.life);
            } else if(k.Key == ConsoleKey.O)
            {
                p2.position[0]++;
                Console.WriteLine("Posicion P2: " + p2.position[0]);
            } else if(k.Key == ConsoleKey.L)
            {
                p2.position[0]--;
                Console.WriteLine("Posicion P2: " + p2.position[0]);
            }
        };


    }
    
    private static void CreateSubmachine()
    {
        //FACTORS
        // Leafs
        positionVariable1 = new LeafVariable(() => p2.position[0], 10.0f, 0.0f);
        positionVariable2 = new LeafVariable(() => p3.position[0], 10.0f, 0.0f);

        //ACTIONS
        // Basic actions
        Action sub1 = new Action(() => {
            Console.WriteLine("[BUCKET_1] Bucket. Acción 1");
            Console.WriteLine("--------------------------------");
        });

        Action sub2 = new Action(() => {
            Console.WriteLine("[BUCKET_2] Bucket. Acción 2.");
            Console.WriteLine("--------------------------------");
            p.life++;
        });

        // Utility Actions
        subUtilMachine.CreateUtilityAction("bucket_1", sub1, positionVariable1);
        subUtilMachine.CreateUtilityAction("bucket_2", sub2, positionVariable2);
    }
   
    private static void CreateMainMachine()
    {
        //FACTORS
        // Leafs
        lifeVariable1 = new LeafVariable(() => p.life, 10.0f, 0.0f);
        lifeVariable2 = new LeafVariable(() => p1.life, 10.0f, 0.0f);


        //ACTIONS

        //Basic actions
        Action a1 = new Action(() => {
            Console.WriteLine("[BASICO1] Se ha entrado al estado básico 1.");
            Console.WriteLine("--------------------------------");
        });

        //Utility Actions
        utilEngine.CreateUtilityAction("basico1", a1, lifeVariable1);
        utilEngine.CreateSubBehaviour("subBucket", lifeVariable2, subUtilMachine);
    }
    
}