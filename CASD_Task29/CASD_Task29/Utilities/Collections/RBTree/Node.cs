namespace Utilities.Collections.RBTree;

internal enum Color : byte
{
    RED, BLACK
}

internal class Node<K, V> where K : IComparable<K>
{
    public K? Key { get; set; }
    public V? Value { get; set; }
    public Color Color { get; set; }
    public Node<K, V>? Parent { get; set; } = null;
    public Node<K, V>? Left { get; set; } = null;
    public Node<K, V>? Right { get; set; } = null;

    public Node(Color color, K? key = default, V? value = default)
    {
        Key = key;
        Value = value;
        Color = color;
    }

    public void CopyKeyValueOf(Node<K, V>? other)
    {
        if (other == null)
            throw new ArgumentNullException();

        Key = other.Key;
        Value = other.Value;
    }
}