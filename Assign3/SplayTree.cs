//Splay tree Assignment
//COIS 2020

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        // Splays node p to the root of the tree using stack S. The stack is used to retrace the
        // access path and determine the types of rotations.
        // Worst case time complexity:  

        private void Splay(Node<T> p, Stack<Node<T>> S)
        {

            // bottom is lowest, middle is above bottom, top is above middle
            Node<T> top;
            Node<T> middle;
            Node<T> bottom;



            if (S.Count != 0)
            {
                bottom = S.Pop();

                if (S.Count % 2 == 1) //Odd start requires single rotation
                {
                    middle = S.Pop();

                    if (bottom.Item.CompareTo(middle.Item) > 0) //Lower node to right of upper node
                    {
                        bottom = LeftRotate(middle);
                    }
                    else if (bottom.Item.CompareTo(middle.Item) < 0) //Lower node to left of upper node
                    {
                        bottom = RightRotate(middle);
                    }
                }


                while (S.Count >= 2)
                {

                    middle = S.Pop();

                    // Connect newly rotated node to the node above it
                    if (bottom.Item.CompareTo(middle.Item) < 0)
                        middle.Left = bottom;
                    else if (bottom.Item.CompareTo(middle.Item) > 0)
                        middle.Left = bottom;

                    top = S.Pop();


                    if (middle.Item.CompareTo(top.Item) < 0) //Middle is left of top
                    {
                        if (bottom.Item.CompareTo(middle.Item) < 0) //Bottom is left of middle is left of top
                        {
                            //Right-Right
                            bottom = RightRotate(RightRotate(top));
                        }
                        else //Bottom is right of middle is left of top
                        {
                            //Left-Right
                            bottom = LeftRotate(middle);
                            top.Left = bottom;
                            bottom = RightRotate(top);
                        }

                    }
                    else if (middle.Item.CompareTo(top.Item) > 0) //Middle is right of top
                    {
                        if (bottom.Item.CompareTo(middle.Item) > 0) //Bottom is right of middle is right of top
                        {
                            //Left-Left
                            bottom = LeftRotate(LeftRotate(top));
                        }
                        else //Bottom is left of middle is right of top
                        {
                            //Right-Left
                            bottom = RightRotate(middle);
                            top.Right = bottom;
                            bottom = LeftRotate(top);
                        }
                    }

                }

                root = bottom;
            }
            else
                throw new Exception("Empty tree");
        }

        // Public Insert
        // adapted from https://www.geeksforgeeks.org/splay-tree-set-2-insert-delete/?ref=rp

        // Inserts an item into a splay tree
        // An exception is throw if the item is already in the tree
        // Amortized time complexity:  O(log n)

        public void Insert(T item)
        {
            Node<T> p = new Node<T>(item);

            if (root == null)                          // If the tree is empty      
                root = p;                              // Create a new root at p
            else
            {
                Access(item); //Search for item
                Stac
                
            }
        }

        // Public Remove
        // adapted from https://www.geeksforgeeks.org/splay-tree-set-3-delete/?ref=rp

        // Remove an item from a splay tree
        // Nothing is performed if the item is not found or the tree is empty
        // Amortized time complexity:  O(log n)

        public bool Remove(T item)
        {
            Node<T> temp;

            if (root != null)                          // Tree not empty (else do nothing)
            {
                root = Splay(item, root);              // Splay item to the root
                if (item.CompareTo(root.Item) == 0)    // Item found at root (else do nothing)
                {
                    if (root.Left == null)             // No left child
                        root = root.Right;             // New root is its right child           
                    else
                    {
                        temp = root;                   // Store the old root
                        root = Splay(item, root.Left); // New root is the maximum child of left subtree
                                                       // Note that the last item visited is the maximum item
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
                root = Splay(item, root);              // Splay item to the root
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

            Print(root, indent);             // Calls private, recursive Print
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

                while (item.CompareTo(curr.Item) != 0 || (curr.Left == null && curr.Right == null)) // Repeat until item found or empty node
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
                }
            }
            return stack;
        }

    }

}