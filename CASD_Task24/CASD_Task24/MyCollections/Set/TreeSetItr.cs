namespace MyCollections.Set;

using RBTree;

internal class TreeSetItr<K> : Iterator<K>
    where K : IComparable<K>
{
    private AbstractTreeItr<K, object> _treeItr;

    public TreeSetItr(AbstractTreeItr<K, object> treeItr)
    {
        _treeItr = treeItr;
    }
    public bool HasNext() => _treeItr.HasNext();

    public K Next() => _treeItr.Next().Key!;

    public K Current() => _treeItr.Current().Key!;

    public void Remove() => _treeItr.Remove();
}
