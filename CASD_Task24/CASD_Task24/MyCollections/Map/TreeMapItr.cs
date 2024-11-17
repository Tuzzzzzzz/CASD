namespace MyCollections.Map;

using RBTree;

internal class TreeMapItr<K, V> : Iterator<(K, V?)>
    where K : IComparable<K>
{
    private AbstractTreeItr<K, V> _treeItr;

    public TreeMapItr(AbstractTreeItr<K, V> treeItr)
    {
        _treeItr = treeItr;
    }
    public bool HasNext() => _treeItr.HasNext();

    public (K, V?) Next()
    {
        var node = _treeItr.Next();
        return (node.Key!, node.Value);
    }

    public (K, V?) Current()
    {
        var node = _treeItr.Current();
        return (node.Key!, node.Value);
    }

    public void Remove() => _treeItr.Remove();
}

