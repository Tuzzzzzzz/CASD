
namespace MyCollections.RBTree;

using MyVector;

public class RBTree<K, V> where K : IComparable<K>
{
    private Node<K, V>? _root = null;

    private IComparer<K> _comparer;

    public static Node<K, V> Nil = new Node<K, V>(Color.BLACK);


    public Node<K, V>? Root => _root;

    public IComparer<K> Comparer => _comparer;


    public RBTree() => _comparer = Comparer<K>.Default;

    public RBTree(IComparer<K>? comparer)
    {
        if (comparer == null)
            throw new ArgumentNullException();
        _comparer = comparer;

    }

    public AbstractTreeItr<K, V> Iterator()
        => new TreeItr<K, V>(this);

    public AbstractTreeItr<K, V> ReverseIterator()
        => new ReverseTreeItr<K, V>(this);


    public Node<K, V>[][] Ribs()
    {
        var ribs = new MyVector<Node<K, V>[]>();
        RecursiveRibs(_root);
        return ribs.ToArray();

        void RecursiveRibs(Node<K, V>? node)
        {
            if (node == null) return;
            if (node.Left != null)
            {
                ribs.Add([node, node.Left]);
                RecursiveRibs(node.Left);
            }
            if (node.Right != null)
            {
                ribs.Add([node, node.Right]);
                RecursiveRibs(node.Right);
            }
        }
    }


    public void Insert(K? key, V? value)
    {
        if (key == null)
            throw new ArgumentNullException();

        var newNode = new Node<K, V>(Color.RED, key, value);
        newNode.Left = Nil;
        newNode.Right = Nil;

        if (_root is null) //empty tree
        {
            _root = newNode;
            Balance(newNode);
            return;
        }

        var parent = SearchParent(key);

        if (parent is null) { _root.Value = value; return; }

        if (_comparer.Compare(key, parent!.Key) > 0)
        {
            if (parent.Right == Nil)
            {
                parent.Right = newNode;
                newNode.Parent = parent;
                Balance(newNode);
            }
            else
            //identical keys
            {
                parent.Right!.Value = value;
            }
            return;
        }

        //key < parent.Key
        if (parent.Left == Nil)
        {
            parent.Left = newNode;
            newNode.Parent = parent;
            Balance(newNode);
        }
        else
        //identical keys
        {
            parent.Left!.Value = value;
        }
        return;
    }


    private void Balance(Node<K, V> node)
    {
        while (node.Parent is not null && node.Parent.Color == Color.RED)
        {
            if (node.Parent == node.Parent.Parent!.Left)
            {
                var uncle = node.Parent.Parent.Right;
                if (uncle!.Color == Color.RED)
                {
                    node.Parent.Color = Color.BLACK;
                    uncle.Color = Color.BLACK;
                    node.Parent.Parent.Color = Color.RED;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Right)
                    {
                        LeftRotate(node.Parent);
                        node = node.Parent;
                    }
                    node.Parent!.Color = Color.BLACK;
                    node.Parent.Parent!.Color = Color.RED;
                    RightRotate(node.Parent.Parent);
                    node = node.Parent;
                }
            }
            else
            {
                var uncle = node.Parent.Parent.Left;
                if (uncle!.Color == Color.RED)
                {
                    node.Parent.Color = Color.BLACK;
                    uncle.Color = Color.BLACK;
                    node.Parent.Parent.Color = Color.RED;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Left)
                    {
                        RightRotate(node.Parent);
                        node = node.Parent;
                    }
                    node.Parent!.Color = Color.BLACK;
                    node.Parent.Parent!.Color = Color.RED;
                    LeftRotate(node.Parent.Parent);
                    node = node.Parent;
                }
            }
        }
        _root!.Color = Color.BLACK;
    }


    private void RightRotate(Node<K, V> node)
    {
        if (node.Parent is null)
            _root = node.Left;
        else
        {
            if (node.Parent.Left == node)
                node.Parent.Left = node.Left;
            else
                node.Parent.Right = node.Left;
        }

        var new_root = node.Left;
        new_root!.Parent = node.Parent;

        var copy = new_root.Right;

        new_root.Right = node;
        node.Parent = new_root;

        node.Left = copy;
        copy!.Parent = node;
    }


    private void LeftRotate(Node<K, V> node)
    {
        if (node.Parent is null)
            _root = node.Right;
        else
        {
            if (node.Parent.Right == node)
                node.Parent.Right = node.Right;
            else
                node.Parent.Left = node.Right;
        }

        var new_root = node.Right;
        new_root!.Parent = node.Parent;

        var copy = new_root.Left;

        new_root.Left = node;
        node.Parent = new_root;

        node.Right = copy;
        copy!.Parent = node;
    }


    public bool ContainsKey(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        return Search(key) != null;
    }


    public Node<K, V>? Search(K? key)
    {
        if (key == null)
            throw new ArgumentNullException();

        var ptr = _root;

        while (ptr is not null && ptr != Nil)
        {
            if (_comparer.Compare(key, ptr.Key) < 0)
                ptr = ptr.Left;
            else if (_comparer.Compare(key, ptr.Key) > 0)
                ptr = ptr.Right;
            else
                return ptr;
        }
        return null;
    }


    public bool ContainsValue(V? value)
    {
        if (value is null)
            throw new ArgumentNullException();

        var it = Iterator();
        while (it.HasNext())
        {
            if (it.Next().Equals(value))
                return true;
        }
        return false;
    }


    private Node<K, V>? SearchParent(K key)
    {
        var ptr = _root;
        Node<K, V>? parent = null;
        while (ptr is not null && ptr != Nil)
        {
            parent = ptr;
            if (_comparer.Compare(key, ptr.Key) < 0)
                ptr = ptr.Left;
            else if (_comparer.Compare(key, ptr.Key) > 0)
                ptr = ptr.Right;
            else
                return parent.Parent;
        }
        return parent;
    }


    private static Node<K, V>? LocalMinNode(Node<K, V>? localRoot)
    {
        if (localRoot is null || localRoot == Nil)
            return null;
        var min = localRoot;
        while (min!.Left != Nil)
            min = min.Left;
        return min;
    }


    private static Node<K, V>? LocalMaxNode(Node<K, V>? localRoot)
    {
        if (localRoot is null || localRoot == Nil)
            return null;
        var max = localRoot;
        while (max!.Right != Nil)
            max = max.Right;
        return max;
    }


    public Node<K, V>? MinNode()
    {
        if (_root is null)
            return null;
        var min = _root;
        while (min!.Left != Nil)
            min = min.Left;
        return min;
    }


    public Node<K, V>? MaxNode()
    {
        if (_root is null)
            return null;
        var max = _root;
        while (max!.Right != Nil)
            max = max.Right;
        return max;
    }


    public bool Remove(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        var delNode = Search(key);

        if (delNode is null)
            return false;

        while (true)
        {
            if (delNode!.Color == Color.RED && delNode.Left == Nil && delNode.Right == Nil)
            {
                if (delNode.Parent is null)
                    _root = Nil;
                else
                {
                    if (delNode.Parent.Right == delNode)
                        delNode.Parent.Right = Nil;
                    else
                        delNode.Parent.Left = Nil;
                }
                break;
            }

            else if (delNode.Left != Nil && delNode.Right != Nil)
            {
                var newDelNode = LocalMinNode(delNode.Right!);

                delNode.CopyKeyValueOf(newDelNode);

                delNode = newDelNode;
            }

            else if (delNode.Color == Color.BLACK && delNode.Right != Nil && delNode.Left == Nil)
            {
                delNode.CopyKeyValueOf(delNode.Right);

                delNode = delNode.Right;
            }

            else if (delNode.Color == Color.BLACK && delNode.Right == Nil && delNode.Left != Nil)
            {
                delNode.CopyKeyValueOf(delNode.Left);

                delNode = delNode.Left;
            }

            else
            {
                if (delNode.Parent is null)
                    _root = null;
                else
                {
                    bool isLeft = (delNode.Parent.Left == delNode);
                    BalanceAfterRemove(delNode);

                    var parent = delNode.Parent;
                    if (isLeft)
                        parent.Left = Nil;
                    else
                        parent.Right = Nil;
                }
                break;
            }
        }

        return true;
    }


    private void BalanceAfterRemove(Node<K, V> node)
    {
        while (node.Parent is not null && node.Color == Color.BLACK)
        {
            if (node.Parent.Left == node)
            {
                var brother = node.Parent.Right;
                if (brother!.Color == Color.BLACK)
                {
                    if (brother.Right!.Color == Color.RED)
                    {
                        brother.Color = brother.Parent!.Color;
                        brother.Parent.Color = Color.BLACK;
                        brother.Right.Color = Color.BLACK;
                        LeftRotate(brother.Parent);
                        break;
                    }
                    else if (brother.Left!.Color == Color.RED && brother.Right.Color == Color.BLACK)
                    {
                        brother.Color = Color.RED;
                        brother.Left.Color = Color.BLACK;
                        RightRotate(brother);
                    }
                    else if (brother.Left.Color == Color.BLACK && brother.Right.Color == Color.BLACK)
                    {
                        brother.Color = Color.RED;
                        if (brother.Parent!.Color == Color.RED)
                        {
                            brother.Parent.Color = Color.BLACK;
                            break;
                        }
                        else
                            node = node.Parent;
                    }
                }
                else
                {
                    brother.Parent!.Color = Color.RED;
                    brother.Color = Color.BLACK;
                    LeftRotate(brother.Parent);
                }
            }
            else
            {
                var brother = node.Parent.Left;
                if (brother!.Color == Color.BLACK)
                {
                    if (brother.Left!.Color == Color.RED)
                    {
                        brother.Color = brother.Parent!.Color;
                        brother.Parent.Color = Color.BLACK;
                        brother.Left.Color = Color.BLACK;
                        RightRotate(brother.Parent);
                        break;
                    }
                    else if (brother.Right!.Color == Color.RED && brother.Left.Color == Color.BLACK)
                    {
                        brother.Color = Color.RED;
                        brother.Right.Color = Color.BLACK;
                        LeftRotate(brother);
                    }
                    else if (brother.Right.Color == Color.BLACK && brother.Left.Color == Color.BLACK)
                    {
                        brother.Color = Color.RED;
                        if (brother.Parent!.Color == Color.RED)
                        {
                            brother.Parent.Color = Color.BLACK;
                            break;
                        }
                        else
                            node = node.Parent;
                    }
                }
                else
                {
                    brother.Parent!.Color = Color.RED;
                    brother.Color = Color.BLACK;
                    RightRotate(brother.Parent);
                }
            }
        }
    }

    public Node<K, V>? LowerEntry(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        if (_root is null) return null;

        var parent = SearchParent(key);

        if (parent is null)
        {
            return LocalMaxNode(_root.Left);
        }

        if (_comparer.Compare(parent.Key, key) < 0)
        {
            if (parent.Right!.Left is null || parent.Right!.Left == Nil)
                return parent;
            return LocalMaxNode(parent.Right.Left);
        }

        if (parent.Left!.Left != Nil) return LocalMaxNode(parent.Left.Left);

        var grandpa = parent.Parent;
        if (grandpa is null) return null;
        while (grandpa is not null)
        {
            if (grandpa.Right == parent)
                return grandpa;
            parent = grandpa;
            grandpa = grandpa.Parent;
        }
        return null;
    }

    public Node<K, V>? HigherEntry(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        if (_root is null) return null;

        var parent = SearchParent(key);

        if (parent is null)
        {
            return LocalMinNode(_root.Right);
        }

        if (_comparer.Compare(parent.Key, key) > 0)
        {
            if (parent.Left!.Right is null || parent.Left!.Right == Nil)
                return parent;
            return LocalMinNode(parent.Left.Right);
        }

        if (parent.Right!.Right != Nil) return LocalMinNode(parent.Right.Right);

        var grandpa = parent.Parent;
        if (grandpa is null) return null;
        while (grandpa is not null)
        {
            if (grandpa.Left == parent)
                return grandpa;
            parent = grandpa;
            grandpa = grandpa.Parent;
        }
        return null;
    }

    public Node<K, V>? FloorEntry(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        if (_root is null) return null;

        var parent = SearchParent(key);

        if (parent is null)
        {
            return _root;
        }

        if (_comparer.Compare(parent.Key, key) < 0)
        {
            if (parent.Right != Nil)
                return parent.Right;
            if (parent.Right!.Left is null || parent.Right!.Left == Nil)
                return parent;
            return LocalMaxNode(parent.Right.Left);
        }

        if (parent.Left != Nil) return parent.Left;

        var grandpa = parent.Parent;
        if (grandpa is null) return null;
        while (grandpa is not null)
        {
            if (grandpa.Right == parent)
                return grandpa;
            parent = grandpa;
            grandpa = grandpa.Parent;
        }
        return null;
    }

    public Node<K, V>? CeilingEntry(K? key)
    {
        if (key is null)
            throw new ArgumentNullException();

        if (_root is null) return null;

        var parent = SearchParent(key);

        if (parent is null)
        {
            return _root;
        }

        if (_comparer.Compare(parent.Key, key) > 0)
        {
            if (parent.Left != Nil)
                return parent.Left;
            if (parent.Left!.Right is null || parent.Left!.Right == Nil)
                return parent;
            return LocalMinNode(parent.Left.Right);
        }

        if (parent.Right != Nil) return parent.Right;

        var grandpa = parent.Parent;
        if (grandpa is null) return null;
        while (grandpa is not null)
        {
            if (grandpa.Left == parent)
                return grandpa;
            parent = grandpa;
            grandpa = grandpa.Parent;
        }
        return null;
    }

    public void Clear() => _root = null;

    public override string ToString()
    {
        var result = "[";
        var it = Iterator();
        while (it.HasNext())
        {
            var node = it.Next();
            var color = node.Color == Color.RED ? "RED" : "BLACK";
            result += $"[color: {color}, key: {node.Key}, value: {node.Value}],\n";
        }
        if (result.Length > 1) result = result.Substring(0, result.Length - 2);
        result += "]";
        return result;
    }
}