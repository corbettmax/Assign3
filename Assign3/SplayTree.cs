//COIS-2020H Assignment 3
//By:
//Max Corbett (0787791)
//Nick Fraga (0765518)
//Drew Goettsche (0767601)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assign3
{
    // Interfaces used for a Splay Tree

    public interface IContainer<T>
    {
        void MakeEmpty();
        bool Empty();
        int Size();
    }

    //-------------------------------------------------------------------------

    public interface ISearchable<T> : IContainer<T>
    {
        void Insert(T item);
        bool Remove(T item);
        bool Contains(T item);
    }

    //-------------------------------------------------------------------------

    // Common generic node class for a splay tree
    // Same data members as a binary search tree

    public class Node<T> where T : IComparable
    {
        // Read/write properties

        public T Item { get; set; }
        public Node<T>? Left { get; set; }
        public Node<T>? Right { get; set; }

        public Node(T item)
        {
            Item = item;
            Left = Right = null;
        }
    }

    //-------------------------------------------------------------------------

    // Implementation:  Splay Tree

    // Note:
    // If item is not found in the tree then the last node (item) visited is splayed to the given root

    class SplayTree<T> : ISearchable<T> where T : IComparable
    {
        private Node<T>? root;                // Reference to the root of a splay tree

        // Constructor
        // Initializes an empty splay tree
        // Time complexity:  O(1)

        public SplayTree()
        {
            root = null;                     // Empty splay tree
        }

        // RightRotate
        // Rotates the splay tree to the right around Node p
        // Returns the new root of the (sub)tree
        // Worst case time complexity:  O(1)

        private Node<T> RightRotate(Node<T> p)
        {
            Node<T> q = p.Left;

            p.Left = q.Right;
            q.Right = p;

            return q;
        }

        // RotateLeft
        // Rotates the splay tree to the left around Node p
        // Returns the new root of the (sub)tree
        // Worst case time complexity:  O(1)

        private Node<T> LeftRotate(Node<T> p)
        {
            Node<T> q = p.Right;

            p.Right = q.Left;
            q.Left = p;

            return q;
        }

        // Splay
        // Splays last node accessed to the root of the tree using stack S. The stack is
        // used to retrace the access path and determine the types of rotations.
        // Worst case time complexity:  

        private void Splay(Stack<Node<T>> S)
        {
            if (S.Count == 0)
                throw new Exception("Empty tree");

            // target lowest, middle is above target, top is above middle
            Node<T> top;
            Node<T> middle;
            Node<T> target;


            target = S.Pop();
            if (S.Count % 2 == 1) //Even start requires single rotation
            {
                middle = S.Pop();

                if (target.Item.CompareTo(middle.Item) > 0) //Lower node to right of upper node
                {
                    target = LeftRotate(middle);
                }
                else if (target.Item.CompareTo(middle.Item) < 0) //Lower node to left of upper node
                {
                    target = RightRotate(middle);
                }
            }


            while (S.Count >= 2)
            {

                middle = S.Pop();

                // Connect newly rotated node to the node above it
                if (target.Item.CompareTo(middle.Item) < 0)
                    middle.Left = target;
                else if (target.Item.CompareTo(middle.Item) > 0)
                    middle.Right = target;

                top = S.Pop();


                if (middle.Item.CompareTo(top.Item) < 0) //Middle is left of top
                {
                    if (target.Item.CompareTo(middle.Item) < 0) //Target is left of middle is left of top
                    {
                        //Right-Right
                        target = RightRotate(RightRotate(top));
                    }
                    else //Target is right of middle is left of top
                    {
                        //Left-Right
                        target = LeftRotate(middle);
                        top.Left = target;
                        target = RightRotate(top);
                    }

                }
                else if (middle.Item.CompareTo(top.Item) > 0) //Middle is right of top
                {
                    if (target.Item.CompareTo(middle.Item) > 0) //Target is right of middle is right of top
                    {
                        //Left-Left
                        target = LeftRotate(middle);
                        top.Right = target;
                        target = LeftRotate(top);
                    }
                    else //Target is left of middle is right of top
                    {
                        //Right-Left
                        target = RightRotate(middle);
                        top.Right = target;
                        target = LeftRotate(top);
                    }
                }

            }

            root = target;
        }

        // Public Insert

        // When an item is successfully inserted, it is splayed to the root. If a duplicate item is
        // found, the duplicate item is splayed to the root.
        // Amortized time complexity:  

        public void Insert(T item)
        {
            Node<T> p = new Node<T>(item);

            if (root == null)                          // If the tree is empty      
                root = p;                              // Create a new root at p
            else
            {
                Stack<Node<T>> S = Access(item); //Search for item
                Node<T>top = S.Peek();
                if (p.Item.CompareTo(top.Item) != 0) //Not a duplicate item
                {
                    if (p.Item.CompareTo(top.Item) < 0) // Item is less than root
                    {
                        top.Left = p;
                    }
                    else if (p.Item.CompareTo(top.Item) > 0) // Item is greater than root
                    {
                        top.Right = p;
                    }
                    
                    // add P to stack
                    S.Push(p);

                }
                Splay(S); //Splay node

            }
        }

        // Remove an item from a splay tree
        // Nothing is performed if the item is not found or the tree is empty
        // Amortized time complexity:  O(log n)

        public bool Remove(T item)
        {
            Node<T> temp;

            if (root != null)                          // Tree not empty (else do nothing)
            {
                Stack<Node<T>> S = Access(item); //Search for item
                Splay(S);                        //Splay last node accessed to root

                if (item.CompareTo(root.Item) == 0)    // Item found at root (else do nothing)
                {
                    if (root.Left == null)             // No left child
                        root = root.Right;             // New root is its right child           
                    else
                    {
                        temp = root;                   // Store the old root

                        Node<T> p = root.Left;
                        S.Push(p);
                        while (p.Right != null) //Keep going down the right of the left subtree
                        {
                            p = p.Right;
                            S.Push(p);
                        }
                        Splay(S); // New root is the maximum child of left subtree
                        root.Right = temp.Right;       // Connect new root with the right subtree
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        // Public Contains
        // Returns true if the item is found in an AVL Tree; false otherwise
        // Amortized time complexity:  O(log n)

        public bool Contains(T item)
        {
            if (root == null)   // Empty splay tree
                return false;
            else
            {
                Stack<Node<T>> S = Access(item); //Search for item
                Splay(S); //Splay last node accessed to root
                return item.CompareTo(root.Item) == 0; // Compare item with that at the root
            }
        }

        // MakeEmpty
        // Resets the splay tree to empty
        // Time complexity:  O(1)

        public void MakeEmpty()
        {
            root = null;
        }

        // Empty
        // Returns true if the splay tree is empty; false otherwise
        // Time complexity:  O(1)

        public bool Empty()
        {
            return root == null;
        }

        // Public Size
        // Returns the number of items in an splay tree
        // Time complexity:  O(n)

        public int Size()
        {
            return Size(root);          // Calls the private, recursive Size
        }

        // Size
        // Calculates the size of the tree rooted at node
        // Time complexity:  O(n)

        private int Size(Node<T> node)
        {
            if (node == null)
                return 0;
            else
                return 1 + Size(node.Left) + Size(node.Right);
        }

        // Public Print
        // Outputs the items of an splay tree in sorted order
        // Time complexity:  O(n)

        public void Print()
        {
            int indent = 0;

            Print(root, indent);  // Calls private, recursive Print
            Console.WriteLine();
        }

        // Private Print
        // Outputs items using an inorder traversal
        // Time complexity:  O(n)

        private void Print(Node<T> node, int indent)
        {
            if (node != null)
            {
                Print(node.Right, indent + 3);
                Console.WriteLine(new String(' ', indent) + node.Item.ToString());
                Print(node.Left, indent + 3);
            }
        }

        // Private Access
        // Returns the nodes in reverse order along the access path from the root to the last node
        // accessed. In the case of a successful insertion, the last node contains the inserted item.
        private Stack<Node<T>> Access(T item)
        {
            Stack<Node<T>> stack = new Stack<Node<T>>();

            if (root != null) // Tree not empty
            {
                Node<T> curr = root;
                stack.Push(curr); // Add root to stack

                if (root.Left == null && root.Right == null)
                    return stack;

                while (item.CompareTo(curr.Item) != 0 || !(curr.Left == null && curr.Right == null)) // Repeat until item found or empty node
                {

                    if (curr.Left != null && curr.Item.CompareTo(item) > 0) // Item is less than current node
                    {
                        curr = curr.Left;
                        stack.Push(curr);
                    }
                    else if (curr.Right != null && curr.Item.CompareTo(item) < 0) //Item is greater than current node
                    {
                        curr = curr.Right;
                        stack.Push(curr);
                    }
                    else
                    {
                        return stack;
                    }
                }
            }
            return stack;
        }

        // Returns a deep copy of the current splay tree using preorder traversal.
        public SplayTree<T> Clone()
        {
            SplayTree<T> clone = new SplayTree<T>();

            clone.root = root;

            //Visit root
            Preorder(root);

            return clone;

        }


        //Preorder traversal
        private static void Preorder(Node<T>? node)
        {
            if (node == null)
                return;

            Preorder(node.Left);
            Preorder(node.Right);
        }

        //Preorder traversal
        private static bool BoolPreorder(Node<T>? node1, Node<T>? node2)
        {

            if (node1 != node2)
                return false;

            if (node1 == null || node2 == null)
                return true;

            if (BoolPreorder(node1.Left, node2.Left) == false)
                return false;
            if (BoolPreorder(node1.Right, node2.Right) == false)
                return false;
            return true;
        }


        //Returns true if t is an exact copy of the current splay tree; false otherwise.
        public override bool Equals(object? t)
        {
            if (t.GetType() != typeof(SplayTree<T>))
                return false;

            SplayTree<T>? tree = t as SplayTree<T>;


            if (BoolPreorder(tree.root, root) == false)
            {
                return false;
            }
            else
                return true;
        }


        public SplayTree<T> Undo()
        {
            //
            if (root == null){
                return null;
            }

            SplayTree<T> original = Clone();
            SplayTree<T> temp = null;

            while (root.Left != null || root.Right != null){
                temp = Clone();

                if (root.Left != null) {
                    RightRotate(root);
                    if (root.Left == null && root.Right == null){
                        break;
                    }
                }
                if (root.Right != null) {
                    LeftRotate(root);
                    if (root.Left == null && root.Right == null){
                        break;
                    }
                }
            }

            if (root.Left == null && root.Right == null){
                root = null;
            } else {
                root = original.root;
            }

            return this;
        }

    }

}