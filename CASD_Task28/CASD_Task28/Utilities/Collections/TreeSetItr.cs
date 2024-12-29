namespace Utilities.Collections;

using CollectionInterfaces;
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

    public K Next() {
        if (!_treeItr.HasNext())
            throw new InvalidOperationException();

        return _treeItr.Next().Key!;
    }

    public void Remove() => _treeItr.Remove();
}
