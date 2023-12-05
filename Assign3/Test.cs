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
        tree.Print();

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
