using System;
using Assign3;

public class Test
{
    public static void Main()
    {
        SplayTree<int> tree = new SplayTree<int>();
        tree.Insert(50);
        tree.Insert(20);
        tree.Insert(65);
        tree.Insert(60);
        tree.Insert(10);
        tree.Insert(45);
        tree.Insert(40);
        tree.Insert(5);
        tree.Insert(35);
        tree.Insert(55);
        tree.Insert(70);
        tree.Insert(15);
        tree.Insert(30);
        tree.Insert(75);
        tree.Insert(8);
        tree.Insert(25);
        Console.WriteLine(tree.Contains(50));
        tree.Print();
        tree.Remove(50);
        tree.Remove(20);
        tree.Print();
        Console.WriteLine(tree.Contains(50));

        SplayTree<int> tree2 = new SplayTree<int>();
        tree2.Insert(50);
        tree2.Insert(20);
        tree2.Insert(65);
        tree2.Insert(60);
        tree2.Insert(10);

        SplayTree<int> treeclone = tree.Clone();
        treeclone.Print();
        Console.WriteLine(treeclone.Equals(tree));
        Console.WriteLine(treeclone.Equals(tree2));


        /*
              75
   70
            65
                  60
               55
                     50
                        45
                  40
         35
      30
25
   20
         15
            10
      8
         5
        */
    }
}
