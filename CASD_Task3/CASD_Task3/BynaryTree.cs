using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace BinaryTree;
using MyArrayList;
internal class BynaryTree
{
    public class BinaryNode
    {
        public int Value;
        public BinaryNode Left = null;
        public BinaryNode Right = null;

        public BinaryNode(int value)
        {
            Value = value;
        }
    }

    private BinaryNode Root = null;
    public BynaryTree(int[] array)
    {
        foreach (int element in array) Add(element);
    }

    public void Add(int value) {
        if (Root == null) Root = new BinaryNode(value);
        else SnapNode(Root, new BinaryNode(value));
    }

    private void SnapNode(BinaryNode parentNode, BinaryNode newNode)
    {
        if (newNode.Value > parentNode.Value)
        {
            if (parentNode.Right == null) parentNode.Right = newNode;
            else SnapNode(parentNode.Right, newNode);
        }
        else
        {
            if (parentNode.Left == null) parentNode.Left = newNode;
            else SnapNode(parentNode.Left, newNode);
        }
    }

    public int[] ToArray()
    {
        MyArrayList<int> arrayList = new MyArrayList<int>();
        Bypass(Root, arrayList);
        return arrayList.ToArray();
    }

    private void Bypass(BinaryNode root, MyArrayList<int> arrayList)
    {
        if(root != null)
        {
            if(root.Left != null) Bypass(root.Left, arrayList);
            arrayList.Add(root.Value);
            if (root.Right != null) Bypass(root.Right, arrayList);
        }
    }
}
