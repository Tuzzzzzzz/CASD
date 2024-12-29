namespace Utilities.Collections.RBTree;

internal class ReverseTreeItr<K, V> : AbstractTreeItr<K, V>
    where K : IComparable<K>
{
    public ReverseTreeItr(RBTree<K, V>? tree) : base(tree) { }

    public override bool HasNext()
        => (_stack.Size() != 0 || _currentNode != RBTree<K, V>.Nil || _deletionBefore) && _tree.Root != null;

    public override Node<K, V> Next()
    {
        if (!HasNext())
            throw new InvalidOperationException();

        if (_deletionBefore)
        {
            _deletionBefore = false;
            return _returnedNode!;
        }

        if ((_stack.Size() != 0 || _currentNode != RBTree<K, V>.Nil) && _tree.Root != null)
        {
            while (_currentNode != RBTree<K, V>.Nil)
            {
                _stack.Push(_currentNode!);
                _currentNode = _currentNode!.Right;
            }
            _currentNode = _stack.Pop();
            _returnedNode = _currentNode;
            _currentNode = _currentNode.Left;
            return _returnedNode;
        }

        throw new InvalidOperationException();
    }

    public override void Remove()
    {
        if (_returnedNode == null)
            throw new InvalidOperationException();

        var delNode = _returnedNode;
        _tree.Remove(delNode!.Key);

        if (_tree.Root is null)
            return;

        _currentNode = _tree.Root;
        _stack.Clear();
        _returnedNode = null;

        while ((_stack.Size() != 0 || _currentNode != RBTree<K, V>.Nil) && _currentNode != null)
        {
            var nextNode = Next();
            if (_tree.Comparer.Compare(nextNode.Key, delNode.Key) > 0)
                break;
        }
        _deletionBefore = true;
    }

    public override Node<K, V> Current()
    {
        if (_returnedNode == null)
            throw new InvalidOperationException();

        return _returnedNode;
    }
}