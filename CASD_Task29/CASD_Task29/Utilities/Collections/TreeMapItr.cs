namespace Utilities.Collections;

using CollectionInterfaces;
using RBTree;

internal class TreeMapItr<K, V> : Iterator<(K, V)>
    where K : IComparable<K>
{
    private AbstractTreeItr<K, V> _treeItr;

    public TreeMapItr(AbstractTreeItr<K, V> treeItr)
    {
        _treeItr = treeItr;
    }

    public bool HasNext() => _treeItr.HasNext();

    public (K, V) Next()
    {
        if (!_treeItr.HasNext())
            throw new InvalidOperationException();

        var node = _treeItr.Next();
        return (node.Key, node.Value)!;
    }

    public void Remove() => _treeItr.Remove();
}

