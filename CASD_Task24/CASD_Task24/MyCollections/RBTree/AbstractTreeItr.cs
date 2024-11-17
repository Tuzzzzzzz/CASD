namespace MyCollections.RBTree;

using MyStack;

public abstract class AbstractTreeItr<K, V> : Iterator<Node<K, V>>
    where K : IComparable<K>
{
    protected RBTree<K, V> _tree;
    protected Node<K, V>? _currentNode;
    protected Node<K, V>? _returnedNode = null;
    protected MyStack<Node<K, V>> _stack = new MyStack<Node<K, V>>();
    protected bool _deletionBefore = false;

    protected AbstractTreeItr(RBTree<K, V>? tree)
    {
        if (tree == null)
            throw new ArgumentNullException();
        _tree = tree;
        _currentNode = tree.Root;
    }

    public abstract bool HasNext();

    public abstract Node<K, V> Next();

    public abstract void Remove();

    public abstract Node<K, V> Current();
}
